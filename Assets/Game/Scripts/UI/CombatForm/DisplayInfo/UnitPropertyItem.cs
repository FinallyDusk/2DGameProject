using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BoringWorld.UI.CombatForm
{
	public class UnitPropertyItem : MonoBehaviour
	{
		private TextMeshProUGUI m_PropertyName;
		private TextMeshProUGUI m_BaseValue;
		private TextMeshProUGUI m_ExtraValue;

		public void OnInit(string propName)
        {
			m_PropertyName = transform.Find("PropertyName").GetComponent<TextMeshProUGUI>();
			m_PropertyName.text = propName;
			m_BaseValue = transform.Find("BaseProperty").GetComponent<TextMeshProUGUI>();
			m_ExtraValue = transform.Find("ExtraProperty").GetComponent<TextMeshProUGUI>();
        }

		public void OnUpdateData(string baseValue, string extraValue)
        {
			m_BaseValue.text = baseValue;
			m_ExtraValue.text = extraValue;
        }
	}
}