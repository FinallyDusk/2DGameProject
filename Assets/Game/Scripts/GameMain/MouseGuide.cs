using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStarUtil;

namespace BoringWorld
{
	public class MouseGuide : MonoBehaviour
	{
        public Vector2 Offset;
        private Transform m_GuideObj;
        private Dictionary<string, Transform> m_AllGuideSprite;
        private Vector3Int m_OldEndPoint;

        private void Awake()
        {
            var allChilds = GetComponentsInChildren<Transform>(true);
            m_AllGuideSprite = new Dictionary<string, Transform>(allChilds.Length);
            for (int i = 1; i < allChilds.Length; i++)
            {
                m_AllGuideSprite.Add(allChilds[i].name, allChilds[i]);
            }
            m_GuideObj = allChilds[1];
            m_OldEndPoint = Vector3Int.zero;
        }

        public void ChangeSprite(TileTag tag)
        {
            string tagStr = tag.ToString();
            if (m_GuideObj.name  == tagStr) return;
            //m_SR.sprite = GameEntry.Sprite.GetSprite(GameEntry.Config.GetString(tag.ToString()), SpriteType.UnitPaint);
            if (m_AllGuideSprite.TryGetValue(tag.ToString(), out var guide))
            {
                m_GuideObj.SetActive(false);
                guide.SetActive(true);
                m_GuideObj = guide;
                return;
            }
            throw new System.NullReferenceException($"查找不到对应的类型SpriteRenderer， tag = {tag}");
        }

        /// <summary>
        /// 获得鼠标在地图上的位置
        /// </summary>
        /// <returns></returns>
        public Vector3Int GetMousePositionInMap()
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector3Int(Mathf.FloorToInt(worldPoint.x - Offset.x), Mathf.FloorToInt(worldPoint.y - Offset.y), 0);
        }

        public void ChangeStatus()
        {

        }

        private void Update()
        {
            transform.localPosition = GetMousePositionInMap();
            //关于切换鼠标引导器图标的问题
            //鼠标引导器只更换自己的图标，其他的问题不管（即鼠标移入敌人身上时显示敌方信息等）
            Vector3Int end = new Vector3Int((int)transform.localPosition.x, (int)transform.localPosition.y, 0);
            if (m_OldEndPoint != end)
            {
                ChangeSprite(GameEntry.Checkpoint.GetTileTag(end));
                m_OldEndPoint = end;
                //Debug.Log($"pos = {end}, Tag = {GameEntry.Checkpoint.GetTileTag(end)}");
            }
            if (GameMain.GameStatus == GameStatus.Normal && Input.GetMouseButtonDown(0))
            {
                var tempStartPos = GameMain.PlayerMapUnit.GetUnitPosition();
                Vector3Int start = new Vector3Int((int)tempStartPos.x, (int)tempStartPos.y, 0);
                var path = Util.AStar.StartAStar(start, end, true, false, GameEntry.Checkpoint.GetCurrentMap());
                GameMain.PlayerMapUnit.Move(path);
            }
        }

        private void ExecuteClickLogic()
        {

        }
    }

    /// <summary>
    /// 鼠标指引器状态
    /// </summary>
    public enum MouseGuideStatus
    {
        /// <summary>
        /// 地图寻路状态
        /// </summary>
        Map,
        /// <summary>
        /// 战斗寻路状态
        /// </summary>
        Combat,
        /// <summary>
        /// 禁用状态
        /// </summary>
        Disable,
    }
}