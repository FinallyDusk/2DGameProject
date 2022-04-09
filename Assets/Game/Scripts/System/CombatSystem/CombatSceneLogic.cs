using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using AStarUtil;

namespace BoringWorld.Combat
{
    /// <summary>
    /// 战斗地图逻辑
    /// </summary>
    public class CombatSceneLogic : MonoBehaviour
    {
        private System.Action m_InitFinshCallback;
        /// <summary>
        /// 战斗地图
        /// </summary>
        private Tilemap m_CombatMap;
        /// <summary>
        /// 范围选择视图地图
        /// </summary>
        private Tilemap m_RangeChooseMap;
        /// <summary>
        /// 战斗鼠标指引器绘制地图
        /// </summary>
        private Tilemap m_CombatMouseGuideMap;
        /// <summary>
        /// 所有Tile的数据
        /// </summary>
        private Dictionary<TileType, Tile> m_AllTileData;
        /// <summary>
        /// 显示范围的点（方便还原）
        /// </summary>
        private Vector3Int[] m_ChooseRangePointList;
        /// <summary>
        /// 战斗鼠标指引器上的点列表
        /// </summary>
        private List<Vector3Int> m_MouseGuidePointList;

        private int m_InitLoadTileCount;

        private bool m_EnabledChooseAtkTarget;
        private System.Action<Vector3Int> m_ChooseAtkTargetClickCallback;
        private Vector3Int m_OldMousePosition;

        private bool m_EnableCheckHasUnit;
        private System.Action<Vector3Int> m_CheckHasUnitCallback;

        //消息机制
        private System.Action<string> m_MsgEvent;

        public void OnInit(System.Action<string> msgEvent, System.Action callback)
        {
            m_MsgEvent = msgEvent;
            m_InitFinshCallback = callback;
            m_CombatMap = transform.Find("CombatMap").GetComponent<Tilemap>();
            m_RangeChooseMap = transform.Find("RangeChooseMap").GetComponent<Tilemap>();
            m_CombatMouseGuideMap = transform.Find("CombatMouseGuideMap").GetComponent<Tilemap>();

            m_MouseGuidePointList = new List<Vector3Int>();

            m_AllTileData = new Dictionary<TileType, Tile>();
            m_InitLoadTileCount = 2;
            var loadAssetCallback = new GameFramework.Resource.LoadAssetCallbacks(LoadAssetSuccessCallback);
            GameEntry.Resource.LoadAsset("Assets/Game/GameResource/Prefabs/Combat/Combat_ChooseRangeTile.asset", loadAssetCallback, TileType.RangeChooseTile);
            GameEntry.Resource.LoadAsset("Assets/Game/GameResource/Prefabs/Combat/Combat_ChooseRangeTile.asset", loadAssetCallback, TileType.AttackGuide);

            m_OldMousePosition = Vector3Int.zero;
            Close();
        }

        private void Update()
        {
            if (m_EnabledChooseAtkTarget)
            {
                var pos = GameMain.MouseGuide.GetMousePositionInMap();
                if (m_OldMousePosition != pos)
                {
                    ChangeMouseGuidePosition(m_OldMousePosition, pos, m_AllTileData[TileType.AttackGuide]);
                    m_OldMousePosition = pos;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    //判断是否在可以选择的范围内
                    if (m_ChooseRangePointList.Has(m_OldMousePosition))
                    {
                        m_ChooseAtkTargetClickCallback?.Invoke(m_OldMousePosition);
                    }
                    else
                    {
                        m_MsgEvent("请选择范围内目标");
                    }
                }
            }
            else
            {
                if (m_EnableCheckHasUnit)
                {
                    var pos = GameMain.MouseGuide.GetMousePositionInMap();
                    if (m_OldMousePosition != pos)
                    {
                        ChangeMouseGuidePosition(m_OldMousePosition, pos, m_AllTileData[TileType.AttackGuide]);
                        m_OldMousePosition = pos;
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        m_CheckHasUnitCallback?.Invoke(pos);
                    }
                }
            }
        }

        /// <summary>
        /// 开启
        /// </summary>
        public void Open()
        {
            gameObject.SetActive(true);
            transform.localPosition = Vector3.zero;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void LoadAssetSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            var tileType = (TileType)userData;
            var tile = asset as Tile;
            m_AllTileData[tileType] = tile;
            m_InitLoadTileCount--;
            if (m_InitLoadTileCount <= 0)
            {
                m_InitFinshCallback?.Invoke();
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 是否启用检测目标点的内容
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="clickCallback">点击之后触发的操作</param>
        public void EnableChoosePointEvent(bool enabled, System.Action<Vector3Int> clickCallback = null)
        {
            m_EnabledChooseAtkTarget = enabled;
            m_ChooseAtkTargetClickCallback = clickCallback;
            ResetCombatMouseGuideMap();
        }

        /// <summary>
        /// 是否启用检查显示目标单位的属性方法
        /// </summary>
        public void EnableCheckUnitInfoFunc(bool enabled, System.Action<Vector3Int> clickCallback = null)
        {
            m_EnableCheckHasUnit = enabled;
            m_CheckHasUnitCallback = clickCallback;
            m_CombatMouseGuideMap.SetTile(m_OldMousePosition, null);
        }

        /// <summary>
        /// 展示可以选择的范围
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="range">范围</param>
        /// <param name="includeCenter">是否包含中心点</param>
        public void DisplayChooseableRange(Vector3Int[] allPoint)
        {
            m_ChooseRangePointList = allPoint;
            //改变这些点的tile
            for (int i = 0; i < m_ChooseRangePointList.Length; i++)
            {
                m_RangeChooseMap.SetTile(m_ChooseRangePointList[i], m_AllTileData[TileType.RangeChooseTile]);
            }
        }

        /// <summary>
        /// 还原可以选择的范围为原来的状态
        /// </summary>
        public void HideChooseRange()
        {
            if (m_ChooseRangePointList.IsNullOrEmpty()) return;
            for (int i = 0; i < m_ChooseRangePointList.Length; i++)
            {
                m_RangeChooseMap.SetTile(m_ChooseRangePointList[i], null);
            }
        }

        private void ResetCombatMouseGuideMap()
        {
            for (int i = 0; i < m_MouseGuidePointList.Count; i++)
            {
                m_CombatMouseGuideMap.SetTile(m_MouseGuidePointList[i], null);
            }
            m_CombatMouseGuideMap.SetTile(m_OldMousePosition, null);
            m_MouseGuidePointList.Clear();
        }

        /// <summary>
        /// 改变鼠标指引器的Tile
        /// </summary>
        /// <param name="oldPos"></param>
        /// <param name="nowPos"></param>
        /// <param name="tileData"></param>
        private void ChangeMouseGuidePosition(Vector3Int oldPos, Vector3Int nowPos, Tile tileData)
        {
            m_CombatMouseGuideMap.SetTile(oldPos, null);
            m_CombatMouseGuideMap.SetTile(nowPos, tileData);
        }

        public bool EndPathForward(Vector3Int pos)
        {
            var tile = m_CombatMap.GetTile(pos);
            if (tile == null) return false;
            return tile.name == Tags.END_PATH_MAP_TILE_NAME;
        }

        private enum TileType
        {
            RangeChooseTile,
            /// <summary>
            /// 攻击指引
            /// </summary>
            AttackGuide,
        }
    }
}