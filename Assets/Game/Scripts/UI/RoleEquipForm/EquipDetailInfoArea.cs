using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BoringWorld.UI.RoleEquip
{
	public class EquipDetailInfoArea : MonoBehaviour
	{
		private TextMeshProUGUI m_EquipName;
		private TextMeshProUGUI m_Rarity;
		private TextMeshProUGUI m_EquipContent;

		public void OnInit()
        {
			m_EquipName = this.GetComponentWithName<TextMeshProUGUI>("EquipName");
			m_Rarity = this.GetComponentWithName<TextMeshProUGUI>("Rarity");
			m_EquipContent = this.GetComponentWithName<TextMeshProUGUI>("EquipDetailContent");
        }

		public void OnOpen()
        {
			m_EquipName.text = string.Empty;
			m_Rarity.text = string.Empty;
			m_EquipContent.text = string.Empty;
        }

		public void OnRefresh(EquipItemDataRow equipData)
		{
			if (equipData == null)
            {
				m_EquipName.text = string.Empty;
				m_Rarity.text = string.Empty;
				m_EquipContent.text = string.Empty;
				return;
			}
			var row = GameEntry.DataTable.GetDataRow<RarityDataRow>(DataTableName.ITEM_RARITY_DATA_NAME, equipData.Rarity);
			m_EquipName.text = $"<color={row.Color}>{equipData.Name}</color>";
			m_Rarity.text = row.Des;
			m_EquipContent.text = equipData.GetItemDes();
        }
	}
}