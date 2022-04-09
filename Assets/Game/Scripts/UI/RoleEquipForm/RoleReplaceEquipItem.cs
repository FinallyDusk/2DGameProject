using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BoringWorld.UI.RoleEquip
{
	public class RoleReplaceEquipItem : BaseLoopSliderItem, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
        private Image m_PitchOn;
        private TextMeshProUGUI m_EquipItem;
        private BackpackItemData m_ItemData;
        //此处应该由数据进行然后通知UI进行刷新，暂时使用UI逻辑去刷新
        private RoleEquipFormLogic m_FormLogic;

        public override void InitItem(object userData)
        {
            base.InitItem(userData);
            m_PitchOn = this.GetComponentWithName<Image>("PitchOn");
            m_EquipItem = this.GetComponentWithName<TextMeshProUGUI>("EquipName");
            m_FormLogic = userData as RoleEquipFormLogic;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_ItemData == null || m_ItemData.ItemData == null) return;
            m_FormLogic.ReplaceTheEquipment(m_ItemData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_ItemData == null || m_ItemData.ItemData == null) return;
            m_FormLogic.ShowEquipInfo(m_ItemData.ItemData as EquipItemDataRow);
            m_PitchOn.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_ItemData == null || m_ItemData.ItemData == null) return;
            m_PitchOn.enabled = false;
        }

        public override void RefreshData(object data)
        {
            base.RefreshData(data);
            m_ItemData = data as BackpackItemData;
            if (m_ItemData == null || m_ItemData.ItemData == null)
            {
                Hide();
            }
            else
            {
                var row = GameEntry.DataTable.GetDataRow<RarityDataRow>(DataTableName.ITEM_RARITY_DATA_NAME, m_ItemData.ItemData.Rarity);
                m_EquipItem.text = $"<color={row.Color}>{m_ItemData.ItemData.Name}</color>";
            }
            m_PitchOn.enabled = false;
        }


    }
}