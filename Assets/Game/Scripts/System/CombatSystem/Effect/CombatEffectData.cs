using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.Combat
{
    /// <summary>
    /// 战斗中的效果数据，包括但不限于技能效果、buff效果
    /// </summary>
    public class CombatEffectData : IReference
    {
        /// <summary>
        /// 效果来源单位（不一定要有）
        /// </summary>
        private CombatUnitInfo m_EffectSource;
        /// <summary>
        /// 效果来源单位
        /// </summary>
        public CombatUnitInfo EffectSource { get { return m_EffectSource; } }
        /// <summary>
        /// 操作发起单位（不一定有）
        /// </summary>
        private CombatUnitInfo m_ActionSource;
        /// <summary>
        /// 操作发起者
        /// </summary>
        public CombatUnitInfo ActionSource { get { return m_ActionSource; } }
        /// <summary>
        /// 操作目标单位（不一定有）
        /// </summary>
        private CombatUnitInfo[] m_ActionTargets;
        /// <summary>
        /// 操作目标
        /// </summary>
        public CombatUnitInfo[] ActionTargets { get { return m_ActionTargets; } }
        /// <summary>
        /// 技能施法点
        /// </summary>
        public Vector3Int SkillCastPoint { get; set; }
        /// <summary>
        /// 最后的技能参数
        /// </summary>
        private CombatSkillInstanceData m_LastSkillData;
        public CombatSkillInstanceData LastSkillData { get { return m_LastSkillData; } }
        /// <summary>
        /// 最后的事件参数
        /// </summary>
        private CombatEventArgs m_LastEventArgs;
        public CombatEventArgs LastEventArgs { get { return m_LastEventArgs; } }
        /// <summary>
        /// 最后的buff参数
        /// </summary>
        private BuffInstanceData m_LastBuffData;
        public BuffInstanceData LastBuffData { get { return m_LastBuffData; } }
        private object m_EventSender;
        /// <summary>
        /// 事件发送者
        /// </summary>
        public object EventSender { get { return m_EventSender; } }
        private System.Action m_Callback;
        public System.Action Callback { get { return m_Callback; } }

        public void Clear()
        {
            m_LastEventArgs.Release();
            m_ActionSource = null;
            m_ActionTargets = null;
        }

        /// <summary>
        /// 设置操作的单位
        /// </summary>
        public void SetActionUnit(CombatUnitInfo source, CombatUnitInfo[] targets)
        {
            m_ActionSource = source;
            m_ActionTargets = targets;
        }

        public static CombatEffectData Create(CombatUnitInfo effectSource, CombatSkillInstanceData skilldata, CombatEventArgs eventArgs, BuffInstanceData buffData, object eventSender, System.Action callback)
        {
            var result = ReferencePool.Acquire<CombatEffectData>();
            result.m_EffectSource = effectSource;
            result.m_LastSkillData = skilldata;
            if (eventArgs != null)
            {
                result.m_LastEventArgs = eventArgs.Copy();
            }
            else
            {
                result.m_LastEventArgs = null;
            }
            result.m_LastBuffData = buffData;
            result.m_EventSender = eventSender;
            result.m_Callback = callback;
            return result;
        }
    }
}