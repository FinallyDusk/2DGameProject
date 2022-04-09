using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    using Combat;
	/// <summary>
	/// 游戏主界面面板
	/// </summary>
	/// 目前只有按ESC打开菜单的功能
	public class MainInterfaceFormLogic : UIFormLogic
	{
        private GameObject m_MiniMapObj;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            //锁定游戏主界面
            GameEntry.UI.SetUIFormInstanceLocked(UIForm, true);

            m_MiniMapObj = transform.Find("SmallMapArea/SmallMapIcon").gameObject;

            GameEntry.Event.Subscribe(CombatEventArgs.EventID, CombatStartCallback);
            GameEntry.Event.Subscribe(CombatEventArgs.EventID, CombatEndCallback);
        }

        private void CombatStartCallback(object sender, System.EventArgs e)
        {
            var ce = e as CombatEventArgs;
            if (ce == null) return;
            if (ce.EventCode == EffectTriggerCode.PrepareCombatStart)
            {
                m_MiniMapObj.SetActive(false);
            }
        }

        private void CombatEndCallback(object sender, System.EventArgs e)
        {
            var ce = e as CombatEventArgs;
            if (ce == null) return;
            if (ce.EventCode == EffectTriggerCode.CombatEnd)
            {
                m_MiniMapObj.SetActive(true);
            }
        }
    }
}