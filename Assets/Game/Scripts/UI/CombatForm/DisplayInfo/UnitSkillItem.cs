using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BoringWorld.UI.CombatForm
{
	/// <summary>
	/// 单位技能脚本（战斗中显示详细信息处）
	/// </summary>
	public class UnitSkillItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private TextMeshProUGUI m_SkillName;
		private TextMeshProUGUI m_LvContent;
		private Image m_CDMask;
		private Image m_PitchOn;

		private string m_DisplayInfo;
		private RectTransform m_RT;

		public void OnInit()
        {
			m_SkillName = transform.Find("SkillName").GetComponent<TextMeshProUGUI>();
			m_LvContent = transform.Find("LvContent").GetComponent<TextMeshProUGUI>();
			m_CDMask = transform.Find("CDMask").GetComponent<Image>();
			m_CDMask.enabled = false;
			m_PitchOn = transform.Find("PitchOn").GetComponent<Image>();
			m_PitchOn.enabled = false;
			m_RT = GetComponent<RectTransform>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
			m_PitchOn.enabled = true;
			GameEntry.Event.Fire(this, SendTipMsgEventArgs.Create(m_DisplayInfo, null, new Rect(m_RT.position, m_RT.sizeDelta)));
        }

        public void OnPointerExit(PointerEventData eventData)
		{
			m_PitchOn.enabled = false;
			GameEntry.Event.Fire(this, CloseTipMsgEventArgs.Create());
		}

        /// <summary>
        /// 更新数据
        /// </summary>
        public void RefreshData(Combat.CombatSkillInstanceData data)
        {
			this.SetActive(true);
			m_SkillName.text = data.BaseData.Name;
			m_LvContent.text = data.Lv.ToString();
			if (data.NowCD == 0)
            {
				m_CDMask.enabled = false;
            }
            else
            {
				m_CDMask.enabled = true;
				m_CDMask.fillAmount = data.NowCD / (data.MaxCD * 1.0f);
            }
			m_DisplayInfo = data.Des;
        }

		public void OnHide()
        {
			gameObject.SetActive(false);
        }
	}
}