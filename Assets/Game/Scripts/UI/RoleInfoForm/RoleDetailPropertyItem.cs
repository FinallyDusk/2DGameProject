using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BoringWorld.UI
{
	public class RoleDetailPropertyItem : MonoBehaviour
	{
        private UnitPropType m_PropType;
        private TextMeshProUGUI m_PropName;
        private TextMeshProUGUI m_PropValue;
        public void Init(UnitPropType propType)
        {
            m_PropType = propType;
            m_PropValue = this.GetComponentWithName<TextMeshProUGUI>("PropertyValue");
            m_PropName = this.GetComponentWithName<TextMeshProUGUI>("PropertyName");
            m_PropName.text = GameEntry.DataTable.GetDataRow<PropertyTypeDesDataRow>(DataTableName.PROPERTY_TYPE_DES_DATA_NAME, (int)m_PropType).PropName;
        }

		public void Refresh(Unit oldUnit, Unit newUnit)
        {
            m_PropValue.text = newUnit.GetUnitPropValueDes(m_PropType);
            if (oldUnit != newUnit)
            {
                if (oldUnit != null)
                {
                    oldUnit.Prop.UnRegisterPropChangeEvent(m_PropType, ChangeShow);
                }
                newUnit.Prop.RegisterPropChangeEvent(m_PropType, ChangeShow);
            }
        }

        private void ChangeShow(UnitProp prop)
        {
            m_PropValue.text = prop.GetPropValueDes(m_PropType);
        }
	}
}