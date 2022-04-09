using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.UI.CombatForm
{
	public class CombatMessageArea : MonoBehaviour
	{
		private CombatMessageItem m_Template;
		private List<CombatMessageItem> m_FreeItems;

		public void OnInit()
        {
			m_Template = GetComponentInChildren<CombatMessageItem>();
			m_FreeItems = new List<CombatMessageItem>();
        }

		public void DisplayeMessage(string msg)
        {
			if (m_FreeItems.Count == 0)
            {
				var obj = CreateItem();
				obj.OnOpen(msg);
            }
            else
            {
				var obj = m_FreeItems[0];
				m_FreeItems.RemoveAt(0);
				obj.OnOpen(msg);
            }
        }

		private CombatMessageItem CreateItem()
        {
			var result = Instantiate(m_Template, m_Template.transform.parent);
			result.OnInit(RecyleItem);
			return result;
        }

		private void RecyleItem(CombatMessageItem item)
        {
			m_FreeItems.Add(item);
        }
	}
}