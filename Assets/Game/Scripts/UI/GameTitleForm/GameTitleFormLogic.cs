using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	public class GameTitleFormLogic : UIFormLogic
	{
#pragma warning disable 0649
        private Button m_StartGameBtn;
		private Button m_ContinueGameBtn;
		private Button m_SettingBtn;
		private Button m_QuitBtn;
#pragma warning restore 0649

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_StartGameBtn = this.GetComponentWithName<Button>("StartGameBtn");
            m_ContinueGameBtn = this.GetComponentWithName<Button>("ContinueGameBtn");
            m_SettingBtn = this.GetComponentWithName<Button>("SettingBtn");
            m_QuitBtn = this.GetComponentWithName<Button>("QuitBtn");
			m_StartGameBtn.onClick.AddListener(StartGameBtnClick);
			m_ContinueGameBtn.onClick.AddListener(ContinueGameBtnClick);
			m_SettingBtn.onClick.AddListener(SettingBtnClick);
			m_QuitBtn.onClick.AddListener(QuitBtnClick);
        }

        private void StartGameBtnClick()
        {
            GameMain.GameStatus = GameStatus.PreLoad;
            GameEntry.UI.CloseUIForm(this);
        }

        private void ContinueGameBtnClick()
        {
            throw new NotImplementedException();
        }

        private void SettingBtnClick()
        {
            throw new NotImplementedException();
        }

        private void QuitBtnClick()
        {
            throw new NotImplementedException();
        }
    }
}