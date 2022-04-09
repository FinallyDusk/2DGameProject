using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.UI.CombatForm
{
	/// <summary>
	/// 战斗中管理血条的区域
	/// </summary>
	public class UnitHeadToolBarArea : MonoBehaviour
	{
#pragma warning disable 0649
        [SerializeField] private UnitTopToolbar m_Template;
#pragma warning restore 0649

        private List<UnitTopToolbar> m_WorkItems;
		private List<UnitTopToolbar> m_FreeItems;

		public void OnInit()
        {
			m_FreeItems = new List<UnitTopToolbar>();
			m_WorkItems = new List<UnitTopToolbar>();
        }

		public void OnResetAllItem()
        {
            for (int i = 0; i < m_WorkItems.Count; i++)
            {
				m_WorkItems[i].OnClose(null);
            }
			m_FreeItems.AddRange(m_WorkItems);
        }

		public void OnShowHeadToolBar(CombatUnitInfo unit)
        {
			if (m_FreeItems.Count == 0)
            {
				var obj = GenerateHpBar();
				obj.OnInit();
				m_WorkItems.Add(obj);
				obj.OnOpen(unit);
            }
            else
            {
				var obj = m_FreeItems[0];
				m_FreeItems.RemoveAt(0);
				m_WorkItems.Add(obj);
				obj.OnOpen(unit);
            }
        }

		private UnitTopToolbar GenerateHpBar()
        {
			UnitTopToolbar result = Instantiate(m_Template, transform);
			return result;
        }

	}
}