using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BoringWorld.UI.CombatForm
{
	public class ActionBtnItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private Image m_PitchOn;
        private Button m_Btn;
        private System.Action m_ClickCallback;

		public void OnInit(System.Action clickCallback)
        {
			m_PitchOn = transform.Find("PitchOn").GetComponent<Image>();
			m_PitchOn.enabled = false;
            m_ClickCallback = clickCallback;
            m_Btn = GetComponent<Button>();
            m_Btn.onClick.AddListener(BtnClick);
        }

        private void BtnClick()
        {
            m_ClickCallback?.Invoke();
        }

        public void OnHide()
        {
            m_PitchOn.enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_PitchOn.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_PitchOn.enabled = false;
        }
    }
}