using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BoringWorld.UI.CombatForm
{
	public class CombatMessageItem : MonoBehaviour
	{
		private System.Action<CombatMessageItem> m_RecyleCallback;
		private DOTweenAnimation m_Anim;
		private TextMeshProUGUI m_MsgContent;

		public void OnInit(System.Action<CombatMessageItem> recyleCallback)
        {
			m_Anim = GetComponent<DOTweenAnimation>();
			m_Anim.onComplete.AddListener(OnClose);
			m_RecyleCallback = recyleCallback;
			m_MsgContent = GetComponentInChildren<TextMeshProUGUI>();
        }

		public void OnOpen(string msg)
        {
			gameObject.SetActive(true);
			m_MsgContent.text = msg;
			m_Anim.DORestart();
        }

		private void OnClose()
        {
			gameObject.SetActive(false);
			m_RecyleCallback(this);
        }
	}
}