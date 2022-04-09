using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoringWorld.UI.CombatForm
{
	/// <summary>
	/// 单位操作按钮组
	/// </summary>
	public class UnitActionBtnGroup : MonoBehaviour
	{
		private ActionBtnGroup m_ActionGroup;
		private ChildrenBtnGroup m_ChildrenGroup;

		private Transform m_DetailItemInfo;
		private TextMeshProUGUI m_DetailInfo;
		private ScrollRect m_Scroll;

		private bool m_ChildOpen;
		private bool m_Pause;

		public void OnInit(Action<UnitActionType> actionBtnCallback)
        {
			m_ActionGroup = this.GetComponentWithName<ActionBtnGroup>("ActionBtnGroup");
			m_ChildrenGroup = this.GetComponentWithName<ChildrenBtnGroup>("ChildrenBtnGroup");
			m_DetailItemInfo = transform.Find("DetailItemInfo");
			m_DetailInfo = m_DetailItemInfo.GetComponentWithName<TextMeshProUGUI>("DetailInfo");
			m_Scroll = GetComponent<ScrollRect>();

			m_ActionGroup.OnInit(actionBtnCallback);
			m_ChildrenGroup.OnInit(ShowDetailItemInfo, HideDetailItemInfo);

			m_ChildOpen = false;

			OnClose();
        }

		public void OnOpen()
        {
			gameObject.SetActive(true);
			if (m_ChildOpen)
            {
				m_ActionGroup.ResetOpen();
				m_ChildrenGroup.ResetOpen();
				m_ChildOpen = false;
			}
            else
			{
				m_ActionGroup.OnOpen();
				m_ChildrenGroup.OnClose();
			}
			m_DetailItemInfo.SetActive(false);
			m_Scroll.content = m_ActionGroup.GetComponent<RectTransform>();
		}

		public void ShowDetailItemInfo(string content)
        {
			m_DetailItemInfo.SetActive(true);
			m_DetailInfo.text = content;
        }

		public void HideDetailItemInfo()
        {
			m_DetailItemInfo.SetActive(false);
        }


		/// <summary>
		/// 显示子技能操作按钮组
		/// </summary>
		/// <param name="allItemDatas"></param>
		public void ShowSkillChildrenBtnGroup(SkillChildrenItemData[] allItemDatas, System.Action<int> clickCallback)
        {
			//主操作按钮组隐藏
			m_ActionGroup.OnHide();
			m_ChildrenGroup.OnOpenSkill(allItemDatas, clickCallback);
			m_Scroll.content = m_ChildrenGroup.GetComponent<RectTransform>();
			m_ChildOpen = true;
        }

		public void OnClose()
        {
			m_Pause = false;
			gameObject.SetActive(false);
        }

		/// <summary>
		/// 暂停操作组（在进行操作的时候）
		/// </summary>
		public void OnPause()
        {
			if (gameObject.activeSelf == false) return;
			gameObject.SetActive(false);
			m_DetailItemInfo.SetActive(false);
			m_Pause = true;
		}

		/// <summary>
		/// 界面从暂停状态退出
		/// </summary>
		public void OnResume()
		{
			if (m_Pause)
			{
				gameObject.SetActive(true);
				m_Pause = false;
			}
		}

	}
}