using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoringWorld.UI.RoleInfoForm
{
	public class RoleDetailPropertyArea : MonoBehaviour
	{
		private TextMeshProUGUI m_UnitName;
		private TextMeshProUGUI m_LvContent;
        private Slider m_ExpSlider;
        private TextMeshProUGUI m_ExpContent;
        private Slider m_HpSlider;
        private TextMeshProUGUI m_HpContent;
        private Slider m_MpSlider;
        private TextMeshProUGUI m_MpContent;
		private RoleDetailPropertyItem[] m_UnitAllProps;

        public void OnInit()
        {
            m_UnitName = this.GetComponentWithName<TextMeshProUGUI>("UnitName");
            m_LvContent = this.GetComponentWithName<TextMeshProUGUI>("LvContent");
            m_ExpSlider = this.GetComponentWithName<Slider>("ExpBar");
            m_ExpContent = this.GetComponentWithName<TextMeshProUGUI>("ExpContent");
            m_HpSlider = this.GetComponentWithName<Slider>("HpBar");
            m_HpContent = this.GetComponentWithName<TextMeshProUGUI>("HpContent");
            m_MpSlider = this.GetComponentWithName<Slider>("MpBar");
            m_MpContent = this.GetComponentWithName<TextMeshProUGUI>("MpContent");

            var t_Item = this.GetComponentWithName<Transform>("Content").GetComponentInChildren<RoleDetailPropertyItem>();
            var allEnums = System.Enum.GetValues(typeof(UnitPropType)) as UnitPropType[];
            m_UnitAllProps = new RoleDetailPropertyItem[allEnums.Length - 4];
            m_UnitAllProps[0] = t_Item;
            int offset = 0;
            int count = 1;
            for (int i = 0; i < allEnums.Length; i++)
            {
                if (count <= (i - offset))
                {
                    var temp = Instantiate(m_UnitAllProps[0], m_UnitAllProps[0].transform.parent);
                    m_UnitAllProps[i - offset] = temp;
                    count++;
                }
                if (allEnums[i] == UnitPropType.MaxHp || allEnums[i] == UnitPropType.MaxMp || 
                    allEnums[i] == UnitPropType.NowHp || allEnums[i] == UnitPropType.NowMp)
                {
                    offset++;
                    continue;
                }
                m_UnitAllProps[i - offset].Init(allEnums[i]);
            }
        }

        public void Refresh(Unit oldUnit, Unit nowUnit)
        {
            m_UnitName.text = nowUnit.BaseData.UnitName;
            m_LvContent.text = nowUnit.Lv.ToString();
            m_ExpSlider.value = nowUnit.NowExp * 1.0f / nowUnit.ExpLimit;
            m_ExpContent.text = $"{nowUnit.NowExp.ToString()}/{nowUnit.ExpLimit.ToString()}";
            if (oldUnit != nowUnit)
            {
                if (oldUnit != null)
                {
                    oldUnit.Prop.UnRegisterPropChangeEvent(UnitPropType.NowHp, RefreshHp);
                    oldUnit.Prop.UnRegisterPropChangeEvent(UnitPropType.NowMp, RefreshMp);
                }
                nowUnit.Prop.RegisterPropChangeEvent(UnitPropType.NowHp, RefreshHp);
                nowUnit.Prop.RegisterPropChangeEvent(UnitPropType.NowMp, RefreshMp);
                RefreshHp(nowUnit.Prop);
                RefreshMp(nowUnit.Prop);
            }
            for (int i = 0; i < m_UnitAllProps.Length; i++)
            {
                m_UnitAllProps[i].Refresh(oldUnit, nowUnit);
            }
        }

        private void RefreshHp(UnitProp prop)
        {
            m_HpSlider.value = (float)(prop.GetProperty(UnitPropType.NowHp) / prop.GetProperty(UnitPropType.MaxHp));
            m_HpContent.text = $"{prop.GetPropValueDes(UnitPropType.NowHp)}/{prop.GetPropValueDes(UnitPropType.MaxHp)}";
        }

        private void RefreshMp(UnitProp prop)
        {
            m_MpSlider.value = (float)(prop.GetProperty(UnitPropType.NowMp) / prop.GetProperty(UnitPropType.MaxMp));
            m_MpContent.text = $"{prop.GetPropValueDes(UnitPropType.NowMp)}/{prop.GetPropValueDes(UnitPropType.MaxMp)}";
        }

    }
}