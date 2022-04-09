using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.UI.CombatForm
{
	using UITabGroup;
    public class PropContent : BaseTabContent
    {
#pragma warning disable 0649
        [SerializeField] private UnitPropertyItem m_PropertyItemTemplate;
#pragma warning restore 0649
        private UnitPropertyItem[] m_AllPropertyItem;
        private Transform m_PropertyItemParent;
        private static int PROPERTY_OFFSET = 0;
        
        public override void OnInit(object userData)
        {
            m_PropertyItemParent = transform.Find("Viewport/Content");
            var allTypes = System.Enum.GetValues(typeof(UnitPropType));
            m_AllPropertyItem = new UnitPropertyItem[allTypes.Length - PROPERTY_OFFSET];
            for (int i = PROPERTY_OFFSET; i < allTypes.Length; i++)
            {
                m_AllPropertyItem[i - PROPERTY_OFFSET] = CreateItem(GameEntry.DataTable.GetDataRow<PropertyTypeDesDataRow>(DataTableName.PROPERTY_TYPE_DES_DATA_NAME, i).PropName);
            }
        }

        private UnitPropertyItem CreateItem(string propName)
        {
            var result = Instantiate(m_PropertyItemTemplate, m_PropertyItemParent);
            result.OnInit(propName);
            return result;
        }

        public override void OnUpdateData(object userData)
        {
            var unitProp = userData as UnitProp;
            for (int i = 0; i < m_AllPropertyItem.Length; i++)
            {
                var propType = (UnitPropType)i;
                string baseValue = PropertyToString.GetStrOnlyValue(propType, unitProp.GetBaseProperty(propType));
                string extraValue = $"<color=#00ff00>{PropertyToString.GetStrOnlyValue(propType, unitProp.GetExtraProperty(propType))}</color>";
                m_AllPropertyItem[i].OnUpdateData(baseValue, extraValue);
            }
        }
    }
}