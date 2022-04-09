using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.UI.CombatForm
{
	/// <summary>
	/// 行动条
	/// </summary>
	public class ActionBarArea : MonoBehaviour
	{
		private ActionItem m_Template;

		private List<ActionItem> m_WorkItems;
		private List<ActionItem> m_FreeItems;

		public void OnInit()
        {
			m_Template = GetComponentInChildren<ActionItem>();
			m_Template.OnInit();
			m_Template.SetActive(false);

			m_WorkItems = new List<ActionItem>();
			m_FreeItems = new List<ActionItem>();
        }

		public void OnResetAllItem()
        {
            for (int i = 0; i < m_WorkItems.Count; i++)
            {
				m_WorkItems[i].OnClose(null);
            }
			m_FreeItems.AddRange(m_WorkItems);
        }

		/// <summary>
		/// 将行动条绑定一个单位
		/// </summary>
		/// <param name="target"></param>
		public void BindUnitActionProgressBar(CombatUnitInfo target)
        {
			if (m_FreeItems.Count == 0)
			{
				var obj = Instantiate(m_Template, m_Template.transform.parent);
				obj.OnInit();
				obj.OnOpen(target);
				m_WorkItems.Add(obj);
			}
            else
            {
				var obj = m_FreeItems[0];
				m_FreeItems.RemoveAt(0);
				obj.OnOpen(target);
				m_WorkItems.Add(obj);
            }
        }

		public void OnClose()
        {

        }
	}
}