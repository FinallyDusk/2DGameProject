using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using DG.Tweening;

namespace BoringWorld.UI.CombatForm
{
	public class CombatFormLogic : UIFormLogic
	{
		private UnitHeadToolBarArea m_UnitHeadToolBar;
		private FloatNumArea m_FloatNumArea;
		private ActionBarArea m_ActionBarArea;
		private BottomPlayerUnitInfoArea m_BottomUnitArea;
		private DisplayInfo m_DisplayInfo;
		private UnitActionBtnGroup m_UnitActionBtnGroup;
		private DOTweenAnimation m_StartCombatAnim;
		private CombatSystem m_Manager;
		private CombatMessageArea m_MessageArea;
		/// <summary>
		/// esc和鼠标右键事件
		/// </summary>
		private System.Action m_ActionCancelEvent;
		private bool m_ActionCancelEnabled;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
			m_UnitHeadToolBar = transform.Find("UnitHeadToolBarArea").GetComponent<UnitHeadToolBarArea>();
			m_FloatNumArea = transform.Find("FloatNumArea").GetComponent<FloatNumArea>();
			m_ActionBarArea = transform.Find("ActionBarArea").GetComponent<ActionBarArea>();
			m_BottomUnitArea = transform.Find("BottomPlayerUnitInfoArea").GetComponent<BottomPlayerUnitInfoArea>();
			m_DisplayInfo = transform.Find("DisplayInfo").GetComponent<DisplayInfo>();
			m_UnitActionBtnGroup = transform.Find("UnitActionBtnGroup").GetComponent<UnitActionBtnGroup>();
			m_StartCombatAnim = transform.Find("StartCombatAnim").GetComponent<DOTweenAnimation>();
			m_MessageArea = transform.Find("MessageArea").GetComponent<CombatMessageArea>();

			m_UnitHeadToolBar.OnInit();
			m_FloatNumArea.OnInit();
			m_ActionBarArea.OnInit();
			m_BottomUnitArea.OnInit();
			m_DisplayInfo.OnInit();
			m_UnitActionBtnGroup.OnInit(ActionBtnClickCallback);
			m_MessageArea.OnInit();

			m_Manager = GameEntry.Combat;
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
			m_StartCombatAnim.DORestart();
			m_ActionBarArea.OnResetAllItem();
			m_BottomUnitArea.OnResetAllItem();
			m_UnitHeadToolBar.OnResetAllItem();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
			if (m_ActionCancelEnabled)
            {
				if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                {
					m_ActionCancelEvent?.Invoke();
                }
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
			m_DisplayInfo.OnClose();
        }

		/// <summary>
		/// 展示广播消息
		/// </summary>
		public void DisplayRadioMessage(string msg)
        {
			m_MessageArea.DisplayeMessage(msg);
        }

		/// <summary>
		/// 修改操作取消的回调
		/// </summary>
		public void EnableActionCancelCallback(bool enabled, System.Action callback = null)
        {
			m_ActionCancelEnabled = enabled;
			m_ActionCancelEvent = callback;
        }

        /// <summary>
        /// 注册一个单位显示
        /// </summary>
        /// <param name="unit">要显示的单位</param>
        /// <param name="playerUnitInfo">是否在底部玩家单位栏显示</param>
        /// <param name="actionBar">是否显示进度条</param>
        public void RegisterUnitDisplay(CombatUnitInfo unit, bool playerUnitInfo = false, bool actionBar = true)
        {
			//显示头顶信息
			m_UnitHeadToolBar.OnShowHeadToolBar(unit);
			if (playerUnitInfo)
            {
				m_BottomUnitArea.BindDisplayUnit(unit);
            }
			if (actionBar)
            {
				m_ActionBarArea.BindUnitActionProgressBar(unit);
            }
        }

		/// <summary>
		/// 注册开始战斗动画结束后的回调
		/// </summary>
		/// <param name="callback"></param>
		public void RegisterStartCombatAnimFinshCallback(UnityEngine.Events.UnityAction callback)
        {
            m_StartCombatAnim.onComplete.AddListener(callback);
        }

        #region 按钮操作组操作
        /// <summary>
        /// 操作按钮点击之后的回调
        /// </summary>
        /// <param name="actionType">操作类型</param>
        private void ActionBtnClickCallback(UnitActionType actionType)
        {
            //此处不应该获取到CombatSystem，而是在初始化时将对应的方法传入
            m_Manager.UnitActionBtnClick(actionType);
        }

        /// <summary>
        /// 显示技能子层级操作按钮组
        /// </summary>
        public void DisplaySkillChildrenActionBtn(SkillChildrenItemData[] allItemDatas, System.Action<int> clickCallback)
        {
            m_UnitActionBtnGroup.ShowSkillChildrenBtnGroup(allItemDatas, clickCallback);
        }

        /// <summary>
        /// 显示可操作组
        /// </summary>
        public void ShowActionGroup()
        {
            m_UnitActionBtnGroup.OnOpen();
        }

        /// <summary>
        /// 关闭可操作组
        /// </summary>
        public void CloseActionGroup()
        {
            m_UnitActionBtnGroup.OnClose();
        }

        /// <summary>
        /// 暂停操作组
        /// </summary>
        public void PauseActionGroup()
        {
            m_UnitActionBtnGroup.OnPause();
        }

        /// <summary>
        /// 恢复操作组状态
        /// </summary>
        public void ResumeActionGroup()
        {
            m_UnitActionBtnGroup.OnResume();
        } 
        #endregion

        /// <summary>
        /// 显示漂浮文字
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="pos">此处需要为世界坐标系，不能为UI坐标系坐标</param>
        public void DisplayFloatText(string content, Vector3 pos)
        {
            m_FloatNumArea.ShowText(content, pos);
        }

        /// <summary>
        /// 显示单位信息
        /// </summary>
        public void DisplayUnitInfo(DisplayInfoData data)
        {
            m_DisplayInfo.OnOpen(data);
        }

        public void HideUnitInfo()
        {
            m_DisplayInfo.OnHide();
        }
    }
}