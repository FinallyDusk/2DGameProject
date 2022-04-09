using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoringWorld
{
	public class ChangeRoleArea : MonoBehaviour
	{
		private Button m_PrevBtn;
		private Button m_NextBtn;
		private TextMeshProUGUI m_RoleName;
		private System.Action<Unit, Unit> m_ChangeCallback;
		private int m_UnitIndex;
		/// <summary>
		/// 玩家可操作单位最大索引值
		/// </summary>
		private int m_PlayerUnitMaxIndex;

		public void OnInit()
        {
			m_PrevBtn = this.GetComponentWithName<Button>("PrevBtn");
			m_NextBtn = this.GetComponentWithName<Button>("NextBtn");
			m_RoleName = this.GetComponentWithName<TextMeshProUGUI>("RoleName");
			m_PrevBtn.onClick.AddListener(PrevBtnClick);
			m_NextBtn.onClick.AddListener(NextBtnClick);
        }

		public void Open(System.Action<Unit, Unit> callback)
        {
			m_ChangeCallback = callback;
			m_UnitIndex = 0;
			m_PlayerUnitMaxIndex = GameMain.GetPlayerAllUnitCount() - 1;
			m_ChangeCallback(null, GameMain.GetPlayerUnitByIndex(m_UnitIndex));
        }

		private void PrevBtnClick()
        {
			var oldUnit = GameMain.GetPlayerUnitByIndex(m_UnitIndex);
			m_UnitIndex--;
			if (m_UnitIndex < 0)
            {
				m_UnitIndex = m_PlayerUnitMaxIndex;
            }
			m_ChangeCallback(oldUnit, GameMain.GetPlayerUnitByIndex(m_UnitIndex));
        }

		private void NextBtnClick()
        {
			var oldUnit = GameMain.GetPlayerUnitByIndex(m_UnitIndex);
			m_UnitIndex++;
			if (m_UnitIndex > m_PlayerUnitMaxIndex)
            {
				m_UnitIndex = 0;
            }
			m_ChangeCallback(oldUnit, GameMain.GetPlayerUnitByIndex(m_UnitIndex));
        }
	}
}