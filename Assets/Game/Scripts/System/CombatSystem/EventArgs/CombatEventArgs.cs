using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.Combat
{
    /// <summary>
    /// 战斗中的事件消息
    /// </summary>
	public class CombatEventArgs : GameEventArgs
	{
        public static int EventID { get { return GameFrameworkExtension.GetEventID(nameof(CombatEventArgs)); } }

        public override int Id { get { return EventID; } }
        /// <summary>
        /// 事件码
        /// </summary>
        public EffectTriggerCode EventCode { get; private set; }
        /// <summary>
        /// 触发操作的单位
        /// </summary>
        public CombatUnitInfo TriggerActionUnit { get; private set; }
        /// <summary>
        /// 该操作针对的目标
        /// </summary>
        public CombatUnitInfo[] ActionTargets { get; private set; }

        public override void Clear()
        {
            
        }

        public CombatEventArgs Copy()
        {
            CombatEventArgs result = ReferencePool.Acquire<CombatEventArgs>();
            result.EventCode = EventCode;
            result.TriggerActionUnit = TriggerActionUnit;
            result.ActionTargets = ActionTargets;
            return result;
        }

        public static CombatEventArgs Create(EffectTriggerCode eventCode, CombatUnitInfo triggerActionUnit, CombatUnitInfo[] actionTargets)
        {
            var result = ReferencePool.Acquire<CombatEventArgs>();
            result.EventCode = eventCode;
            result.TriggerActionUnit = triggerActionUnit;
            result.ActionTargets = actionTargets;
            return result;
        }
    }
}