using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BoringWorld.UI.RoleEquip
{
	public class RoleDressEquipPartItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
#pragma warning disable 0649
        [SerializeField] private UnitEquipPartType m_PartType;
#pragma warning restore 0649
        private TextMeshProUGUI m_EquipName;
        private Image m_PitchOn;

        private EquipItemDataRow m_EquipData;
        private RoleEquipFormLogic m_FormLogic;

        public void OnInit(RoleEquipFormLogic formLogic)
        {
            m_EquipName = this.GetComponentWithName<TextMeshProUGUI>("EquipName");
            m_PitchOn = this.GetComponentWithName<Image>("PitchOn");
            m_PitchOn.enabled = false;
            m_FormLogic = formLogic;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            m_FormLogic.ShowReplaceEquipArea(m_PartType);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_PitchOn.enabled = true;
            m_FormLogic.ShowEquipInfo(m_EquipData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_PitchOn.enabled = false;
        }

        public void OnRefresh(Unit unit)
        {
            m_EquipData = unit.GetUnitDressEquip(m_PartType);
            if (m_EquipData == null)
            {
                m_EquipName.text = string.Empty;
            }
            else
            {
                var row = GameEntry.DataTable.GetDataRow<RarityDataRow>(DataTableName.ITEM_RARITY_DATA_NAME, m_EquipData.Rarity);
                m_EquipName.text = $"<color={row.Color}>{m_EquipData.Name}</color>";
            }
        }

    }
}