using BoringWorld.UITabGroup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.UI.CombatForm
{
    public class SkillContent : BaseTabContent
    {
#pragma warning disable 0649
        [SerializeField] private UnitSkillItem m_ItemTemplate;
        [SerializeField] private Transform m_ItemParent;
#pragma warning restore 0649
        private List<UnitSkillItem> m_AllItems;

        public override void OnInit(object userData)
        {
            m_AllItems = new List<UnitSkillItem>();
        }

        public override void OnUpdateData(object userData)
        {
            var allSkill = userData as List<Combat.CombatSkillInstanceData>;
            for (int i = m_AllItems.Count; i < allSkill.Count; i++)
            {
                m_AllItems.Add(CreateItem());
            }
            int index = 0;
            foreach (var item in allSkill)
            {
                m_AllItems[index++].RefreshData(item);
            }
            for (int i = allSkill.Count; i < m_AllItems.Count; i++)
            {
                m_AllItems[i].OnHide();
            }
        }

        private UnitSkillItem CreateItem()
        {
            var result = Instantiate(m_ItemTemplate, m_ItemParent);
            result.OnInit();
            return result;
        }
    }
}