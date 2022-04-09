using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityGameFramework.Runtime;

namespace BoringWorld.UI.Backpack
{
    /// <summary>
    /// 背包格子UI
    /// </summary>
    public class BackpackGridItem : BaseLoopSliderItem, IPointerEnterHandler, IPointerExitHandler
    {
        private Image m_ItemIcon;
        private Image m_PitchOn;
        private TextMeshProUGUI m_ItemName;
        private TextMeshProUGUI m_ItemCount;

        private BackpackFormLogic m_FormLogic;
        private BackpackItemData m_ItemData;

        public override void InitItem(object userData)
        {
            base.InitItem(userData);
            m_FormLogic = userData as BackpackFormLogic;
            if (m_FormLogic == null)
            {
                Log.Error($"此处必须要传入BackpackFormLogic才行");
                return;
            }
            m_ItemIcon = this.GetComponentWithName<Image>("ItemIcon");
            m_PitchOn = this.GetComponentWithName<Image>("PitchOn");
            m_ItemName = this.GetComponentWithName<TextMeshProUGUI>("ItemName");
            m_ItemCount = this.GetComponentWithName<TextMeshProUGUI>("ItemCount");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_ItemName == null || m_ItemData.ItemData == null) return;
            m_FormLogic.ShowItemDetailInfo(m_ItemData.ItemData);
            m_PitchOn.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_ItemName == null || m_ItemData.ItemData == null) return;
            m_PitchOn.enabled = false;
        }

        public override void RefreshData(object data)
        {
            base.RefreshData(data);
            //todo，记得刷新图片
            m_ItemData = data as BackpackItemData;
            var rarityRow = GameEntry.DataTable.GetDataRow<RarityDataRow>(DataTableName.ITEM_RARITY_DATA_NAME, m_ItemData.ItemData.Rarity);
            m_ItemName.text = $"<color={rarityRow.Color}>{m_ItemData.ItemData.Name}</color>";
            m_ItemCount.text = m_ItemData.Count.ToString();
            m_PitchOn.enabled = false;
        }
    }
}