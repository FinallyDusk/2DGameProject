using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.Combat
{
    /// <summary>
    /// 战斗中执行技能所需参数
    /// </summary>
    public class CombatSkillArgs : IReference
    {
        /// <summary>
        /// 此技能的触发条件
        /// </summary>
        public EffectTriggerCode[] TriggerConditions { get; private set; }
        /// <summary>
        /// 触发单位
        /// </summary>
        public CombatUnitInfo TriggerUnit { get; private set; }
        public int SkillID { get; private set; }
        /// <summary>
        /// 技能等级
        /// </summary>
        public int SkillLv { get; private set; }
        /// <summary>
        /// 技能类型
        /// </summary>
        public SkillCategory Category { get; private set; }
        /// <summary>
        /// 技能实例
        /// </summary>
        public CombatSkillInstanceData SkillInstance { get; private set; }
        /// <summary>
        /// 操作目标，只有在<see cref="TriggerConditions"/>这些条件需要目标时才会有
        /// </summary>
        public CombatUnitInfo ActionTarget { get; private set; }
        /// <summary>
        /// 施法点
        /// </summary>
        public Vector3Int CastPoint { get; set; }
        /// <summary>
        /// 技能执行完成之后的回调
        /// </summary>
        public System.Action Callback { get; private set; }

        public void Clear()
        {

        }

        public static CombatSkillArgs Create(CombatUnitInfo triggerUnit, CombatSkillInstanceData skillInstance, SkillCategory category, CombatUnitInfo actionTarget, System.Action callback, EffectTriggerCode[] triggerConditions)
        {
            var result = ReferencePool.Acquire<CombatSkillArgs>();
            result.SkillID = skillInstance.Id;
            result.TriggerUnit = triggerUnit;
            result.SkillLv = skillInstance.Lv;
            result.SkillInstance = skillInstance;
            result.Category = category;
            result.ActionTarget = actionTarget;
            result.Callback = callback;
            result.TriggerConditions = triggerConditions;
            return result;
        }
    }
}