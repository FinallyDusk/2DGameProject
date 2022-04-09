using BoringWorld.Combat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld.UI.CombatForm
{
    public class UnitTopToolbar : MonoBehaviour
    {
        private SliderBar m_HpBar;
        private SliderBar m_MpBar;
        private TextMeshProUGUI m_UnitSimpleInfo;
        private RectTransform m_RT;
#pragma warning disable 0649
        [SerializeField]
        private Transform m_TargetObj;
        [SerializeField]
        private BuffItem m_TemplateBuffItem;
#pragma warning restore 0649
        private RectTransform m_BuffArea;
        private List<BuffItem> m_WorkdBuffItems;
        private List<BuffItem> m_FreeBuffItems;

        public void OnInit()
        {
            m_HpBar = transform.Find("HpBar").GetComponent<SliderBar>();
            m_MpBar = transform.Find("MpBar").GetComponent<SliderBar>();
            m_UnitSimpleInfo = transform.Find("UnitSimpleInfo").GetComponent<TextMeshProUGUI>();
            m_RT = GetComponent<RectTransform>();
            m_BuffArea = transform.Find("BuffArea").GetComponent<RectTransform>();
            m_WorkdBuffItems = new List<BuffItem>();
            m_FreeBuffItems = new List<BuffItem>();

            m_HpBar.OnInit();
            m_MpBar.OnInit();
        }

        public void OnOpen(CombatUnitInfo unit)
        {
            m_TargetObj = unit.GetUnitHeadObject();
            m_UnitSimpleInfo.text = unit.BaseData.UnitName;
            m_HpBar.OnOpen((float)(unit.Prop.GetProperty(UnitPropType.NowHp)/ unit.Prop.GetProperty(UnitPropType.MaxHp)));
            m_MpBar.OnOpen((float)(unit.Prop.GetProperty(UnitPropType.NowMp) / unit.Prop.GetProperty(UnitPropType.MaxMp)));
            unit.Prop.RegisterPropChangeEvent(UnitPropType.NowHp, HpChange);
            unit.Prop.RegisterPropChangeEvent(UnitPropType.NowMp, MpChange);
            unit.RegisterBuffDisplay(AddBuff, RemoveBuff);
        }

        private System.Action<BuffInstanceData> AddBuff(int buffID)
        {
            for (int i = 0; i < m_WorkdBuffItems.Count; i++)
            {
                if (buffID == m_WorkdBuffItems[i].BuffId)
                {
                    Log.Error($"已经存在相同的实例buff，请检查，buffID = {buffID}");
                    return null;
                }
            }
            BuffItem obj = null;
            if (m_FreeBuffItems.Count == 0)
            {
                obj = CreateBuffItem();
            }
            else
            {
                obj = m_FreeBuffItems[0];
                m_FreeBuffItems.RemoveAt(0);
            }
            obj.OnOpen(buffID);
            m_WorkdBuffItems.Add(obj);
            return obj.OnRefresh;
        }

        private void RemoveBuff(int buffID)
        {
            int index = -1;
            for (int i = 0; i < m_WorkdBuffItems.Count; i++)
            {
                if (buffID == m_WorkdBuffItems[i].BuffId)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                var obj = m_WorkdBuffItems[index];
                m_WorkdBuffItems.RemoveAt(index);
                obj.OnClose();
                m_FreeBuffItems.Add(obj);
            }
        }

        private BuffItem CreateBuffItem()
        {
            var obj = Instantiate(m_TemplateBuffItem, m_BuffArea);
            obj.OnInit();
            return obj;
        }

        private void Update()
        {
            if (m_TargetObj != null)
            {
                m_RT.position = Camera.main.WorldToScreenPoint(m_TargetObj.position);
            }
        }

        public void OnClose(Unit unit)
        {
            m_TargetObj = null;
            m_RT.localPosition = new Vector3(1999, 1999, 1999);
            unit.Prop.UnRegisterPropChangeEvent(UnitPropType.NowHp, HpChange);
            unit.Prop.UnRegisterPropChangeEvent(UnitPropType.NowMp, MpChange);
        }

        private void HpChange(UnitProp prop)
        {
            m_HpBar.ValueChange(prop.GetProperty(UnitPropType.NowHp), prop.GetProperty(UnitPropType.MaxHp));
        }

        private void MpChange(UnitProp prop)
        {
            m_MpBar.ValueChange(prop.GetProperty(UnitPropType.NowMp), prop.GetProperty(UnitPropType.MaxMp));
        }
    } 
}
