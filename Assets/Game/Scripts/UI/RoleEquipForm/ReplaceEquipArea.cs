using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoringWorld.UI.RoleEquip
{
	public class ReplaceEquipArea : MonoBehaviour
	{
		private LoopSlider m_Slider;
		private Button m_CloseBtn;

		public void OnInit()
        {
			m_Slider = GetComponentInChildren<LoopSlider>();
			gameObject.SetActive(false);
			m_CloseBtn = this.GetComponentWithName<Button>("CloseBtn");
			m_CloseBtn.onClick.AddListener(() => { OnClose(); });
        }

		public void OnOpen(ILoopSliderDataSource source, RoleEquipFormLogic userData)
        {
			gameObject.SetActive(true);
			GameEntry.Input.AddDisableKeyCodeDown(KeyCode.Escape);
			m_Slider.InitLoopSlider(source, userData);
        }

		public void OnUpdate()
        {
			if (Input.GetMouseButtonDown(1))
            {
				OnClose();
            }
        }

		public void OnClose()
        {
			gameObject.SetActive(false);
			GameEntry.Input.RemoveDisableKeyCodeDown(KeyCode.Escape);
        }
		
	}
}