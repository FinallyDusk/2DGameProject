using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.UI.RoleEquip
{
	public class RoleAllDressEquipArea : MonoBehaviour
	{
		private RoleDressEquipPartItem[] m_RoleAllDressItems;

		public void OnInit(RoleEquipFormLogic formLogic)
        {
			m_RoleAllDressItems = GetComponentsInChildren<RoleDressEquipPartItem>();
            for (int i = 0; i < m_RoleAllDressItems.Length; i++)
            {
				m_RoleAllDressItems[i].OnInit(formLogic);
            }
        }

		public void OnRefresh(Unit unit)
        {
            for (int i = 0; i < m_RoleAllDressItems.Length; i++)
            {
                m_RoleAllDressItems[i].OnRefresh(unit);
            }
        }

	}
}