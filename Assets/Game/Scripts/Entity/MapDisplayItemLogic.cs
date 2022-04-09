using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	/// <summary>
	/// 地图显示项目逻辑（如单位，道具之类的）
	/// </summary>
	public class MapDisplayItemLogic : EntityLogic
	{
        private UnitCamp m_Camp;
        public UnitCamp Camp
        {
            get
            {
                return m_Camp;
            }
            set
            {
                m_Camp = value;
            }
        }
        private MapUnitShowInfo m_UnitInfo;
        private SpriteRenderer m_SR;
        #region 移动相关，此处还需要修改，在Unit中也有相同的移动

        private Stack<Vector3Int> m_MovePathStack;
        private bool m_Moveing;
        private const float MOVE_INTERVAL = 0.15f;
        private float m_MoveTime;

        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_SR = GetComponent<SpriteRenderer>();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (m_Moveing)
            {
                m_MoveTime += Time.deltaTime;
                if (m_MoveTime >= MOVE_INTERVAL)
                {
                    m_MoveTime -= MOVE_INTERVAL;
                    TransformPosition(m_MovePathStack.Pop());
                    if (m_MovePathStack.Count <= 0)
                    {
                        m_Moveing = false;
                        m_MoveTime = 0;
                    }
                }
            }
        }

        public void SetSprite(string spriteName, SpriteType type)
        {
            m_SR.sprite = GameEntry.Sprite.GetSprite(spriteName, type);
        }

        public void TransformPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        /// <summary>
        /// 获得单位坐标
        /// </summary>
        /// <returns></returns>
        public Vector3 GetUnitPosition()
        {
            return transform.localPosition;
        }

        /// <summary>
        /// 按照路径移动
        /// </summary>
        /// <param name="path"></param>
        public void Move(Stack<Vector3Int> path)
        {
            m_MovePathStack = path;
            m_Moveing = true;
        }

        /// <summary>
        /// 注册实体显示信息
        /// </summary>
        public void RegisterShowInfo(MapUnitShowInfo unitInfo)
        {
            m_UnitInfo = unitInfo;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var hostilityEntityLogic = collision.gameObject.GetComponent<MapDisplayItemLogic>();
            if (hostilityEntityLogic != null && hostilityEntityLogic.Camp == UnitCamp.Hostility)
            {
                if (hostilityEntityLogic.m_UnitInfo == null)
                {
                    Log.Warning($"请为该实体传入显示数据, name = {name}");
#if UNITY_EDITOR
                    UnityEditor.EditorGUIUtility.PingObject(this);
#endif
                    return;
                }
                Log.Debug("遇见怪物了");
                //进入战斗
                GameEntry.Combat.OpenCombat(hostilityEntityLogic.m_UnitInfo.UnitGroupID);
            }
        }

        private void OnMouseEnter()
        {

        }

        private void OnMouseExit()
        {

        }
    }
}