using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BoringWorld.UI.RoleSkill
{
	public class SkillPreviewItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private TextMeshProUGUI m_SkillName;
		private Image m_LockMask;
		private Image m_PitchOn;

		private UnitSkillShowInfo m_ShowInfo;
		private RoleSkillFormLogic m_FormLogic;

		public void OnInit(RoleSkillFormLogic formLogic)
        {
			m_SkillName = this.GetComponentWithName<TextMeshProUGUI>("SkillName");
			m_LockMask = this.GetComponentWithName<Image>("LockMask");
			m_PitchOn = this.GetComponentWithName<Image>("PitchOn");
			m_FormLogic = formLogic;
        }

        public void OnPointerEnter(PointerEventData eventData)
		{
			m_FormLogic.ShowSkillInfo(m_ShowInfo);
			m_PitchOn.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
			m_PitchOn.enabled = false;
		}

        public void OnRefresh(UnitSkillShowInfo skillInfo)
        {
			gameObject.SetActive(true);
			m_SkillName.text = skillInfo.Name;
			m_LockMask.enabled = skillInfo.SkillLock;
			m_ShowInfo = skillInfo;
			m_PitchOn.enabled = false;
        }

		public void OnClose()
        {
			gameObject.SetActive(false);
        }
	}


	/// <summary>
	/// 单位技能显示信息
	/// </summary>
	public struct UnitSkillShowInfo
    {
		public int SkillID;
		public string Name;
		public bool SkillLock;
		public string Lv;
		public string CostMp;
		public string CDTurn;
		public string Des;
		public string NeedPoint;

		public UnitSkillShowInfo(int skillID, string name, bool skillLock, string lv, string costMp, string cdTurn, string des, string needPoint)
        {
			SkillID = skillID;
			Name = name;
			SkillLock = skillLock;
			Lv = lv;
			CostMp = costMp;
			CDTurn = cdTurn;
			Des = des;
			NeedPoint = needPoint;
        }
    }
}