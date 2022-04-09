using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    /// <summary>
    /// 加载进度界面
    /// </summary>
    public class LoadProgressFormLogic : UIFormLogic
	{
        private TextMeshProUGUI m_ProgressContent;
        private TextMeshProUGUI m_LoadContent;
        
        private System.Action m_LoadFinshCallback;
#pragma warning disable 0414
        private float m_Progress;
        private float m_ProgressTotal;
#pragma warning restore 0414
        private const float PROGRESS_TOTAL = 100;
        private const float OFFSET = 0.01f;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_ProgressContent = this.GetComponentWithName<TextMeshProUGUI>("ProgressContent");
            m_LoadContent = this.GetComponentWithName<TextMeshProUGUI>("LoadContent");
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            System.Action sa = userData as System.Action;
			if (sa == null)
            {
                if (GameMain.LoadProgress == null)
                {
                    GameMain.LoadProgress = this;
                    GameEntry.UI.CloseUIForm(this);
                }
                else
                {
                    Log.Fatal("加载进度界面需要一个加载完成的回调");
                }
                return;
            }
            m_LoadFinshCallback += sa;
            m_Progress = 0;
            m_ProgressTotal = PROGRESS_TOTAL;
        }

        public void ChangeLoadTitleContent(string content)
        {
            m_LoadContent.text = content;
        }

        public void ChangeLoadProgress(float progressDelta)
        {
            m_Progress += progressDelta;
            m_ProgressContent.text = m_Progress.ToString("f1");
            float differenceValue = Mathf.Abs(m_Progress - m_ProgressTotal);
            if (differenceValue <= OFFSET)
            {
                m_LoadFinshCallback?.Invoke();
                GameEntry.UI.CloseUIForm(this);
            }
        }

    }
}