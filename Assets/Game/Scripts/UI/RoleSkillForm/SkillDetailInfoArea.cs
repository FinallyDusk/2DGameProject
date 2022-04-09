using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoringWorld.UI.RoleSkill
{
	public class SkillDetailInfoArea : MonoBehaviour
	{
		private TextMeshProUGUI m_SkillName;
		private TextMeshProUGUI m_LvContent;
		private TextMeshProUGUI m_CostMpContent;
		private TextMeshProUGUI m_CDTurnContent;
		private TextMeshProUGUI m_Des;

		private TextMeshProUGUI m_ActionMessageContent;
		private Button m_SkillLvDownBtn;
		private Button m_SkillLvUpBtn;
		private TextMeshProUGUI m_NeedPointContent;
		private TextMeshProUGUI m_UnitSkillPointContent;

		private RoleSkillFormLogic m_FormLogic;

		public void OnInit(RoleSkillFormLogic formLogic)
        {
			m_SkillName = this.GetComponentWithName<TextMeshProUGUI>("SkillName");
			m_LvContent = this.GetComponentWithName<TextMeshProUGUI>("LvContent");
			m_CostMpContent = this.GetComponentWithName<TextMeshProUGUI>("CostContent");
			m_CDTurnContent = this.GetComponentWithName<TextMeshProUGUI>("TurnContent");
			m_Des = this.GetComponentWithName<TextMeshProUGUI>("SkillDesContent");
			m_ActionMessageContent = this.GetComponentWithName<TextMeshProUGUI>("ActionMessageContent");
			m_SkillLvDownBtn = this.GetComponentWithName<Button>("SkillLvDownBtn");
			m_SkillLvUpBtn = this.GetComponentWithName<Button>("SkillLvUpBtn");
			m_NeedPointContent = this.GetComponentWithName<TextMeshProUGUI>("NeedPointContent");
			m_UnitSkillPointContent = this.GetComponentWithName<TextMeshProUGUI>("RoleSkillPointContent");

			m_SkillLvDownBtn.onClick.AddListener(SkillLvDownBtnClick);
			m_SkillLvUpBtn.onClick.AddListener(SkillLvUpBtnClick);

			m_SkillLvDownBtn.interactable = false;
			m_SkillLvUpBtn.interactable = false;

			m_FormLogic = formLogic;
		}

		public void OnRefresh(UnitSkillShowInfo skillInfo, int unitSkillPoint)
        {
			m_SkillName.text = skillInfo.Name;
			m_LvContent.text = skillInfo.Lv;
			m_CostMpContent.text = skillInfo.CostMp;
			m_CDTurnContent.text = skillInfo.CDTurn;
			m_Des.text = $"<line-height=80%>\n{skillInfo.Des}</line-height>\n";
			m_ActionMessageContent.text = string.Empty;
			m_NeedPointContent.text = skillInfo.NeedPoint;
			m_UnitSkillPointContent.text = unitSkillPoint.ToString();
			if (skillInfo.SkillLock)
            {
				m_ActionMessageContent.text = "该技能未解锁";
            }
			m_SkillLvDownBtn.interactable = !skillInfo.SkillLock;
			m_SkillLvUpBtn.interactable = !skillInfo.SkillLock;
        }

		public void RefreshActionMsg(string msg)
        {
			m_ActionMessageContent.text = msg;
        }

		public void OnClose()
        {
			m_SkillName.text = string.Empty;
			m_LvContent.text = "--";
			m_CostMpContent.text = "--";
			m_CDTurnContent.text = "--";
			m_Des.text = string.Empty;
			m_ActionMessageContent.text = string.Empty;
			m_NeedPointContent.text = "--";
			m_UnitSkillPointContent.text = "--";
			m_SkillLvDownBtn.interactable = false;
			m_SkillLvUpBtn.interactable = false;
		}

        private void SkillLvUpBtnClick()
        {
			m_FormLogic.UpgradeSkillLevel();
        }

        private void SkillLvDownBtnClick()
        {
			m_FormLogic.LowerSkillLevel();
        }
    }
}