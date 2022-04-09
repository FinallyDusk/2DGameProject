using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.Combat
{
    /// <summary>
    /// buff实例数据
    /// </summary>
    /// todo,有很多开放的方法，不应该让外部知道
    public class BuffInstanceData : IReference
    {
        private BuffDataRow m_BaseData;
        private CombatUnitInfo m_Target;
        private CombatUnitInfo m_Source;
        private int m_InstanceID;
        private System.Action<BuffInstanceData> m_ChangeAction;
        /// <summary>
        /// 是否为第一回合标志，自己给自己添加的buff并且处于自己的回合的时候是不会减少持续时间的
        /// </summary>
        //private bool m_FirstTurn;
        private int m_DurationCount;

        public string IconName { get { return m_BaseData.IconName; } }
        public int InstanceID { get { return m_InstanceID; } }

        /// <summary>
        /// 获得显示信息
        /// </summary>
        /// <returns></returns>
        public string GetDisplayInfo()
        {
            if (m_BaseData.AttenuationCondition == null || m_BaseData.AttenuationCondition.TriggerCode == null) return string.Empty;
            return m_DurationCount.ToString();
        }

        /// <summary>
        /// 获得描述信息
        /// </summary>
        /// <returns></returns>
        public string GetDes()
        {
            return m_BaseData.Des;
        }

        /// <summary>
        /// 添加到单位上去
        /// </summary>
        public void AddToUnit(CombatUnitInfo target, System.Action<BuffInstanceData> changeAction)
        {
            m_Target = target;
            m_ChangeAction = changeAction;
            m_ChangeAction?.Invoke(this);
            //增加属性
            if (m_BaseData.ExtraAddProperty.IsNullOrEmpty() == false)
            {
                foreach (var item in m_BaseData.ExtraAddProperty)
                {
                    m_Target.Prop.AddProperty(item.PropType, item.Category, item.ValueType, item.Value);
                }
            }
            //给单位添加效果标签
            if (m_BaseData.EffectTags.IsNullOrEmpty() == false)
            {
                m_Target.AddTag(m_BaseData.EffectTags);
            }
            //计算第一回合标签
            //if (m_Source == m_Target && m_Target.ActiveTurn)
            //{
            //    m_FirstTurn = true;
            //}
            //注册效果生效消息
            if (m_BaseData.TakeEffectCondition != null)
            {
                GameEntry.Event.Subscribe(CombatEventArgs.EventID, TriggerEffect);
            }
            //注册效果次数衰减消息
            if (m_BaseData.AttenuationCondition != null)
            {
                GameEntry.Event.Subscribe(CombatEventArgs.EventID, EffectCountAttenuation);
            }
        }

        /// <summary>
        /// 触发效果
        /// </summary>
        private void TriggerEffect(object sender, GameEventArgs e)
        {
            var ce = e as CombatEventArgs;
            if (ce == null) return;
            if (m_BaseData.TakeEffectCondition.TriggerCode.Has(ce.EventCode) == false) return;
            if (m_BaseData.TakeEffectCondition.SelfTrigger && ce.TriggerActionUnit != m_Target) return;
            //执行效果
            var ed = CombatEffectData.Create(m_Target, null, ce, this, sender, null);
            ed.SetActionUnit(ce.TriggerActionUnit, ce.ActionTargets);
            GameEntry.Lua.ExecuteCombatEffectAction(m_BaseData.EffectScriptFile, ed);
        }

        private void EffectCountAttenuation(object sender, GameEventArgs e)
        {
            var ce = e as CombatEventArgs;
            //判断是否为战斗事件
            if (ce == null) return;
            //判断是否包含次数衰减的消息码
            if (m_BaseData.AttenuationCondition.TriggerCode.Has(ce.EventCode) == false) return;
            //判断是否为“自己”触发的事件
            if (m_BaseData.AttenuationCondition.SelfTrigger && ce.TriggerActionUnit != m_Target) return;
            m_DurationCount--;
            //if (ce.EventCode == EffectTriggerCode.TurnEnd)
            //{
            //    if (m_FirstTurn)
            //    {
            //        m_DurationCount++;
            //        return;
            //    }
            //}
            if (m_DurationCount == 0)
            {
                m_Target.RemoveBuff(this);
                return;
            }
            //调用回调
            m_ChangeAction?.Invoke(this);
        }

        public void Clear()
        {
            //减少属性
            if (m_BaseData.ExtraAddProperty.IsNullOrEmpty() == false)
            {
                foreach (var item in m_BaseData.ExtraAddProperty)
                {
                    m_Target.Prop.ReduceProperty(item.PropType, item.Category, item.ValueType, item.Value);
                }
            }
            //给单位减少效果标签
            if (m_BaseData.EffectTags.IsNullOrEmpty() == false)
            {
                m_Target.RemoveTag(m_BaseData.EffectTags);
            }
            //m_FirstTurn = false;
            //取消注册消息
            if (m_BaseData.TakeEffectCondition != null)
            {
                GameEntry.Event.Unsubscribe(CombatEventArgs.EventID, TriggerEffect);
            }
            if (m_BaseData.AttenuationCondition != null)
            {
                GameEntry.Event.Unsubscribe(CombatEventArgs.EventID, EffectCountAttenuation);
            }
        }

        public static BuffInstanceData Create(int instanceID, BuffDataRow baseData)
        {
            var result = ReferencePool.Acquire<BuffInstanceData>();
            result.m_BaseData = baseData;
            result.m_DurationCount = baseData.DurationCount;
            result.m_InstanceID = instanceID;
            return result;
        }
    }
}