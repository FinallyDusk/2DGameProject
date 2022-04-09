using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AStarUtil
{
    public class AStar
    {
        private const int LIST_COUNT = 100;

        /// <summary>
        /// 周围的坐标偏差值,暂时只管4个方向
        /// </summary>
        private Vector3Int[] m_AroundOffset;

        private Dictionary<Vector3Int, Point> m_AllPoint;
        private List<Point> m_OpenList;
        private List<Point> m_CloseList;
        private Point m_CurrentPoint;
        /// <summary>
        /// 起点
        /// </summary>
        private Point m_StartPoint;
        /// <summary>
        /// 结束点
        /// </summary>
        private Point m_EndPoint;
        private Point m_ResultPoint;

        private IAstarHelper m_Helper;

        public void Init()
        {
            m_AllPoint = new Dictionary<Vector3Int, Point>();
            m_OpenList = new List<Point>(LIST_COUNT);
            m_CloseList = new List<Point>(LIST_COUNT);
            m_AroundOffset = new Vector3Int[] { new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0) };
        }

        /// <summary>
        /// 开始A*寻路
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="allowClosestPoint">没有合适路径时是否运行返回最近的点</param>
        /// <returns></returns>
        public Stack<Vector3Int> StartAStar(Vector3Int startPoint, Vector3Int endPoint, bool allowClosestPoint, bool ingoreUnit, IAstarHelper helper)
        {
            if (startPoint == endPoint) return null;

            m_Helper = helper;

            foreach (var item in m_AllPoint)
            {
                ReferencePool.Release(item.Value);
            }
            m_AllPoint.Clear();
            m_OpenList.Clear();
            m_CloseList.Clear();

            m_StartPoint = Point.Create(startPoint, 0, 0);
            m_EndPoint = Point.Create(endPoint, 0, 0);
            m_StartPoint.CalcAndSetH(m_EndPoint);
            m_OpenList.Add(m_StartPoint);
            m_AllPoint.Add(startPoint, m_StartPoint);
            m_AllPoint.Add(endPoint, m_EndPoint);
            if (CalcPath(allowClosestPoint, ingoreUnit) == false)
            {
                Debug.LogError("查找失败");
                return null;
            }
            else
            {
                Debug.Log("查找成功");
                return GetPath();
            }
        }


        /// <summary>
        /// 查找距离不超过maxLength的圆形范围内的所有位置，不包括起始点
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="maxLength"></param>
        /// <param name="helper"></param>
        /// <param name="points"></param>
        public void FindAllPositionByCirle(Vector3Int startPoint, int maxLength, IAstarHelper helper, bool ingoreUnit, List<Vector3Int> points)
        {
            points.Clear();
            List<Vector3Int> edgePosition = new List<Vector3Int>();
            edgePosition.Add(startPoint);
            int currentLength = 1;
            while (currentLength <= maxLength)
            {
                int newPosIndex = points.Count;
                for (int i = 0; i < m_AroundOffset.Length; i++)
                {
                    foreach (var item in edgePosition)
                    {
                        Vector3Int offsetPos = item + m_AroundOffset[i];
                        if (helper.PositionIsValid(offsetPos, ingoreUnit))
                        {
                            if (points.Contains(offsetPos) == false)
                            {
                                points.Add(offsetPos);
                            }
                        }
                    }
                }
                edgePosition.Clear();
                for (int i = newPosIndex; i < points.Count; i++)
                {
                    edgePosition.Add(points[i]);
                }
                currentLength++;
            }
        }

        /// <summary>
        /// 查找x差和y差皆不大于maxLength的正方形范围内的所有位置，不包括起始点
        /// </summary>
        public void FindAllPositionBySquare(Vector3Int startPoint, int maxLength, IAstarHelper helper, bool ingoreUnit, List<Vector3Int> points)
        {
            points.Clear();
            for (int x = -maxLength; x <= maxLength; x++)
            {
                for (int y = -maxLength; y <= maxLength; y++)
                {
                    var pos = startPoint + new Vector3Int(x, y, 0);
                    if (helper.PositionIsValid(pos, ingoreUnit))
                    {
                        points.Add(pos);
                    }
                }
            }
            points.Remove(startPoint);
        }

        /// <summary>
        /// 查找一行距离小于等于maxLength所有位置，maxLength = -1时表示无视距离查找
        /// </summary>
        public void FindAllPositionByRow(Vector3Int startPoint, int maxLength, IAstarHelper helper, bool ingoreUnit, List<Vector3Int> points)
        {
            points.Clear();
            if (maxLength == -1)
            {
                //左右进行查找
                //左
                var pos = startPoint + Vector3Int.left;
                while (helper.EndPathForward(pos) == false)
                {
                    if (helper.PositionIsValid(pos, ingoreUnit))
                    {
                        points.Add(pos);
                    }
                    pos = pos + Vector3Int.left;
                }
                //右
                pos = startPoint + Vector3Int.right;
                while (helper.EndPathForward(pos) == false)
                {
                    if (helper.PositionIsValid(pos, ingoreUnit))
                    {
                        points.Add(pos);
                    }
                    pos = pos + Vector3Int.right;
                }
            }
            else
            {
                //按距离左右查找
                //左
                var pos = startPoint + Vector3Int.left;
                int distance = 1;
                while (distance <= maxLength)
                {
                    if (helper.PositionIsValid(pos, ingoreUnit))
                    {
                        points.Add(pos);
                    }
                    pos = pos + Vector3Int.left;
                }
                //右
                pos = startPoint + Vector3Int.right;
                distance = 1;
                while (distance <= maxLength)
                {
                    if (helper.PositionIsValid(pos, ingoreUnit))
                    {
                        points.Add(pos);
                    }
                    pos = pos + Vector3Int.right;
                }
            }
        }

        private Stack<Vector3Int> GetPath()
        {
            Stack<Vector3Int> path = new Stack<Vector3Int>();
            Point temp = m_ResultPoint;
            while (temp.Parent != null)
            {
                path.Push(temp.Position);
                temp = temp.Parent;
            }
            path.Push(temp.Position);
            return path;
        }

        /// <summary>
        /// 计算路径
        /// </summary>
        /// 1. 如果OpenList为空，则查找失败,如果OpenList中存在目标点，则查找成功
        /// 2. 先查找OpenList中F值最小的Point，把它变为CurrentPoint并且从OpenList中移除然后加入CloseList
        /// 3. 获得当前Point的有效周围节点，并对每个节点做如下操作：
        ///     3.1 如果该节点在CloseList中，则不作任何操作
        ///     3.2 如果该节点在OpenList中，则检查通过当前节点计算得到的F值是否更小，如果更小则更新其F值，并将该节点的父节点设置为当前节点，否则不作任何操作
        ///     3.3 如果该节点既不在CloseList中也不在OpenList中，则将其加入OpenList中，计算F值，设置其父节点为当前节点
        private bool CalcPath(bool allowClosestPoint, bool ingoreUnit)
        {
            List<Point> aroundPointList = new List<Point>(4);
            while (true)
            {
                if (m_OpenList.Count == 0)
                {
                    if (allowClosestPoint)
                    {
                        //从CloseList中找出H值最小的点，并返回路径
                        int minValue = m_CloseList[0].H;
                        int minIndex = 0;
                        for (int i = 1; i < m_CloseList.Count; i++)
                        {
                            if (minValue > m_CloseList[i].H)
                            {
                                minValue = m_CloseList[i].H;
                                minIndex = i;
                            }
                        }
                        m_ResultPoint = m_CloseList[minIndex];
                        return true;
                    }
                    return false;
                }
                if (m_OpenList.Contains(m_EndPoint))
                {
                    m_EndPoint.Parent = m_CurrentPoint;
                    m_ResultPoint = m_EndPoint;
                    return true;
                }

                m_CurrentPoint = FindOpenListMinValue();
                m_OpenList.Remove(m_CurrentPoint);
                m_CloseList.Add(m_CurrentPoint);

                FindAroundValidPoint(ingoreUnit, ref aroundPointList);
                for (int i = 0; i < aroundPointList.Count; i++)
                {
                    if (m_CloseList.Contains(aroundPointList[i]))
                    {
                        continue;
                    }
                    else if (m_OpenList.Contains(aroundPointList[i]))
                    {
                        //一般情况下H值不会进行改变，所以只需要比较G值即可
                        if (Point.CalcG(m_CurrentPoint) < aroundPointList[i].G)
                        {
                            aroundPointList[i].UpdateParent(m_CurrentPoint);
                        }
                    }
                    else
                    {
                        m_OpenList.Add(aroundPointList[i]);
                        aroundPointList[i].UpdateParent(m_CurrentPoint);
                    }
                }
            }
        }

        /// <summary>
        /// 计算open列表中f值最小的
        /// </summary>
        private Point FindOpenListMinValue()
        {
            int minValue = m_OpenList[0].F;
            int minIndex = 0;
            for (int i = 1; i < m_OpenList.Count; i++)
            {
                if (minValue > m_OpenList[i].F)
                {
                    minValue = m_OpenList[i].F;
                    minIndex = i;
                }
                else if (minValue == m_OpenList[i].F)
                {
                    //当f值相同时的操作，优先选取右方和下方的
                    if (m_OpenList[minIndex].Position.x < m_OpenList[i].Position.x)
                    {
                        minValue = m_OpenList[i].F;
                        minIndex = i;
                    }
                    else if (m_OpenList[minIndex].Position.x == m_OpenList[i].Position.x)
                    {
                        if (m_OpenList[minIndex].Position.y > m_OpenList[i].Position.y)
                        {
                            minValue = m_OpenList[i].F;
                            minIndex = i;
                        }
                    }
                }
            }
            return m_OpenList[minIndex];
        }

        /// <summary>
        /// 查找周围的有效节点（返回的节点内容不包括无法通行的地方）
        /// </summary>
        /// <param name="aroundPointList">周围节点在OpenList中的节点</param>
        private void FindAroundValidPoint(bool ingoreUnit, ref List<Point> aroundPointList)
        {
            aroundPointList.Clear();
            //此处只查找上下左右四个方向
            for (int i = 0; i < m_AroundOffset.Length; i++)
            {
                //todo，此处需要与地图系统进行确认该点是否可以通行
                var position = m_CurrentPoint.Position + m_AroundOffset[i];
                if (m_Helper.PositionIsValid(position, ingoreUnit) == false)
                {
                    continue;
                }
                if (!m_AllPoint.TryGetValue(position, out var result))
                {
                    result = CreatePoint(position);
                }
                aroundPointList.Add(result);
            }
        }

        private Point CreatePoint(Vector3Int position)
        {
            var result = Point.Create(position, Point.CalcG(m_CurrentPoint), Mathf.Abs(position.x - m_EndPoint.Position.x) + Mathf.Abs(position.y - m_EndPoint.Position.y));
            m_AllPoint.Add(position, result);
            return result;
        }
    }

    public class Point : IReference
    {
        public Point Parent;
        public Vector3Int Position;
        public int G;//从起始点一直往父物体的路径走的值
        public int H;//到目标点的值
        public int F;//等于g+h，为优先级（越低优先级越高）

        public void UpdateParent(Point parent)
        {
            Parent = parent;
            G = CalcG(parent);
            F = G + H;
        }

        public int CalcAndSetH(Point endPoint)
        {
            H = Mathf.Abs(Position.x - endPoint.Position.x) + Mathf.Abs(Position.y - endPoint.Position.y);
            F = G + H;
            return H;
        }

        public void Clear()
        {
            Parent = null;
            G = 0;
            H = 0;
            F = 0;
        }

        public static int CalcG(Point tempParent)
        {
            //此处可对上下左右的不同消耗进行计算
            return tempParent.G + 1;
        }

        public static Point Create(Vector3Int position, int g, int h)
        {
            Point result = ReferencePool.Acquire<Point>();
            result.Position = position;
            result.G = g;
            result.H = h;
            result.F = result.G + result.H;
            return result;
        }
    }

    /// <summary>
    /// Astar的帮助接口
    /// </summary>
    public interface IAstarHelper
    {
        /// <summary>
        /// 坐标位置是否有效（能否通行）,true表示可以通行，false为不能通行
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="ingoreUnit">是否忽略单位的影响，true为忽略，false为不忽略</param>
        /// <returns></returns>
        bool PositionIsValid(Vector3Int pos, bool ingoreUnit);
        /// <summary>
        /// 前方路径是否已经结束,true为结束，false为未结束
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool EndPathForward(Vector3Int pos);
    }
}
