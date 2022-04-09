using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoringWorld.UI.RoleSkill
{
	public class AllSkillArea : MonoBehaviour
	{
		private Button m_InitiativeSkillTabBtn;
		private Button m_PassivitySkillTabBtn;

		private SkillPreviewItem m_SkillItemTemplate;
		private List<SkillPreviewItem> m_AllSkillItems;

		private RoleSkillFormLogic m_FormLogic;

		public void OnInit(RoleSkillFormLogic formLogic)
        {
			m_FormLogic = formLogic;

			m_InitiativeSkillTabBtn = this.GetComponentWithName<Button>("InitiativeSkillTab");
			m_PassivitySkillTabBtn = this.GetComponentWithName<Button>("PassivitySkillTab");
			m_InitiativeSkillTabBtn.onClick.AddListener(InitiativeSkillTabBtnClick);
			m_PassivitySkillTabBtn.onClick.AddListener(PassivitySkillTabBtnClick);

			m_SkillItemTemplate = this.GetComponentWithName<SkillPreviewItem>("SkillPreviewItem");
			m_SkillItemTemplate.OnInit(formLogic);
			m_AllSkillItems = new List<SkillPreviewItem>();
			m_AllSkillItems.Add(m_SkillItemTemplate);
        }

		public void OnRefresh(SkillCategory category, Unit unit)
        {
			UnitSkillShowInfo[] allInfos = unit.GetUnitAllCategorySkillInfo(category);
            for (int i = 0; i < allInfos.Length; i++)
            {
				if (m_AllSkillItems.Count <= i)
                {
					var obj = Instantiate(m_SkillItemTemplate, m_SkillItemTemplate.transform.parent);
					obj.OnInit(m_FormLogic);
					m_AllSkillItems.Add(obj);
                }
				m_AllSkillItems[i].OnRefresh(allInfos[i]);
            }
            for (int i = allInfos.Length; i < m_AllSkillItems.Count; i++)
            {
				m_AllSkillItems[i].OnClose();
            }
        }


		private void InitiativeSkillTabBtnClick()
        {
			m_FormLogic.ChangeShowSkillCategory(SkillCategory.Initiative);
        }

		private void PassivitySkillTabBtnClick()
        {
			m_FormLogic.ChangeShowSkillCategory(SkillCategory.Passivity);
		}
	}
}