using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	/// <summary>
	/// 游戏主菜单
	/// </summary>
	public class GameMenuFormLogic : UIFormLogic
	{
		#region 组件
		//按钮组
		private Button m_RoleInfoBtn;
		private Button m_SkillBtn;
		private Button m_EquipBtn;
		private Button m_BackpackBtn;
		private Button m_SaveGameBtn;
		private Button m_QuitGameBtn;
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
			m_RoleInfoBtn = this.GetComponentWithName<Button>("RoleInfoBtn");
			m_RoleInfoBtn.onClick.AddListener(RoleInfoBtnClick);
			m_SkillBtn = this.GetComponentWithName<Button>("SkillBtn");
			m_SkillBtn.onClick.AddListener(SkillBtnClick);
			m_EquipBtn = this.GetComponentWithName<Button>("EquipBtn");
			m_EquipBtn.onClick.AddListener(EquipBtnClick);
			m_BackpackBtn = this.GetComponentWithName<Button>("BackpackBtn");
			m_BackpackBtn.onClick.AddListener(BackpackBtnClick);
			m_SaveGameBtn = this.GetComponentWithName<Button>("SaveGameBtn");
			m_SaveGameBtn.onClick.AddListener(SaveGameBtnClick);
			m_QuitGameBtn = this.GetComponentWithName<Button>("QuitGameBtn");
			m_QuitGameBtn.onClick.AddListener(QuitGameBtnClick);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            GameMain.MouseGuide.SetActive(false);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameMain.MouseGuide.SetActive(true);
        }

        private void RoleInfoBtnClick()
        {
            GameEntry.UI.OpenUIFormByType(UIFormType.RoleInfoForm);
        }

        private void SkillBtnClick()
        {
            GameEntry.UI.OpenUIFormByType(UIFormType.RoleSkillForm);
        }

        private void EquipBtnClick()
        {
            GameEntry.UI.OpenUIFormByType(UIFormType.RoleEquipForm);
        }

        private void BackpackBtnClick()
        {
            GameEntry.UI.OpenUIFormByType(UIFormType.BackpackForm);
        }

        private void SaveGameBtnClick()
        {
            throw new NotImplementedException();
        }

        private void QuitGameBtnClick()
        {
            throw new NotImplementedException();
        }
    }
}