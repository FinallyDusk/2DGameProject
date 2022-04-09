using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{

	public class FloatNumArea : MonoBehaviour
	{
		private FloatNum m_Template;

		private List<FloatNum> m_FreeItems;
		private List<FloatNum> m_WorkItems;

		public void OnInit()
        {
			m_Template = GetComponentInChildren<FloatNum>();
			m_Template.OnInit(this);
			m_FreeItems = new List<FloatNum>();
			m_WorkItems = new List<FloatNum>();
        }

		public void ShowText(string content, Vector3 pos)
        {
			FloatNum temp = null;
			if (m_FreeItems.Count == 0)
            {
				temp = GenerateFloatNum();
            }
            else
            {
				temp = m_FreeItems[0];
				m_FreeItems.RemoveAt(0);
            }
			temp.ShowText(content, pos);
        }

		private FloatNum GenerateFloatNum()
        {
			var result = Instantiate(m_Template, transform);
			result.OnInit(this);
			return result;
        }

		public void RecyleItem(FloatNum item)
        {
			m_WorkItems.Remove(item);
			m_FreeItems.Add(item);
        }
	}
}