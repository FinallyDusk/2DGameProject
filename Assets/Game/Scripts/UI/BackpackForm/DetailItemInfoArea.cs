using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BoringWorld.UI.Backpack
{
	public class DetailItemInfoArea : MonoBehaviour
	{
#pragma warning disable 0649
        private TextMeshProUGUI m_ItemName;
        private TextMeshProUGUI m_ItemRarity;
        //private TextMeshProUGUI m_ItemLvContent;
        private TextMeshProUGUI m_SellPriceContent;
        private TextMeshProUGUI m_ItemDesContent;
#pragma warning restore 0649

		public void OnInit()
        {
			m_ItemName = this.GetComponentWithName<TextMeshProUGUI>("ItemName");
			m_ItemRarity = this.GetComponentWithName<TextMeshProUGUI>("ItemRarity");
			m_SellPriceContent = this.GetComponentWithName<TextMeshProUGUI>("SellPriceContent");
			m_ItemDesContent = this.GetComponentWithName<TextMeshProUGUI>("ItemDetailDesContent");
        }

		public void Clear()
        {
			m_ItemName.text = string.Empty;
			m_ItemRarity.text = string.Empty;
			m_SellPriceContent.text = string.Empty;
			m_ItemDesContent.text = string.Empty;
        }

        public void ShowData(ItemDataRow data)
        {
			var rarityRow = GameEntry.DataTable.GetDataRow<RarityDataRow>(DataTableName.ITEM_RARITY_DATA_NAME, data.Rarity);
			m_ItemName.text = $"<color={rarityRow.Color}>{data.Name}</color>";
			m_ItemRarity.text = rarityRow.Des;//$"<color={rarityRow.Color}>{rarityRow.Des}</color>";
			//if (data is EquipItemDataRow)
			//{
			//	m_ItemLvContent.gameObject.SetActive(true);
			//	m_ItemLvContent.text = 
			//}
			m_SellPriceContent.text = data.SellPrice.ToString("f0");
			m_ItemDesContent.text = data.GetItemDes();
        }
	}
}