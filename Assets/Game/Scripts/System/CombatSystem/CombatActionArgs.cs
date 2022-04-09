using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.Combat
{
    /// <summary>
    /// 战斗中各种操作需要的参数
    /// </summary>
    public class CombatActionArgs : IReference
    {
        private CombatUnitInfo m_ActionSource;
        public CombatUnitInfo ActionSource { get { return m_ActionSource; } }
        private List<CombatUnitInfo> m_ActionTargets;
        /// <summary>
        /// 操作目标群体（第一个为主目标）
        /// </summary>
        public List<CombatUnitInfo> ActionTargets { get { return m_ActionTargets; } }
        private System.Action m_Complete;
        /// <summary>
        /// 操作完成时的回调
        /// </summary>
        public System.Action Complete { get { return m_Complete; } }
        /// <summary>
        /// 操作需要的点
        /// </summary>
        public Vector3Int ActionPoint { get; set; }
        /// <summary>
        /// 操作中的技能实例
        /// </summary>
        public CombatSkillInstanceData SkillInstance { get; set; }
        /// <summary>
        /// 触发码（不一定有）
        /// </summary>
        public EffectTriggerCode TriggerCode { get; set; }
        /// <summary>
        /// 伤害数据（不一定有）
        /// </summary>
        public HarmData Harm { get; set; }

        public CombatActionArgs()
        {
            m_ActionTargets = new List<CombatUnitInfo>();
        }

        /// <summary>
        /// 增加完成回调
        /// </summary>
        public void AddCompleteCallback(System.Action complete)
        {
            if (complete == null) return;
            m_Complete += complete;
        }

        /// <summary>
        /// 增加操作目标
        /// </summary>
        /// <param name="target"></param>
        public void AddActionTarget(CombatUnitInfo target)
        {
            m_ActionTargets.Add(target);
        }

        /// <summary>
        /// 增加操作目标
        /// </summary>
        /// <param name="targets"></param>
        public void AddActionTarget(IEnumerable<CombatUnitInfo> targets)
        {
            if (targets == null) return;
            m_ActionTargets.AddRange(targets);
        }

        public void Clear()
        {
            m_Complete = null;
            SkillInstance = null;
            ActionPoint = Vector3Int.zero;
            m_ActionTargets.Clear();
        }

        public static CombatActionArgs Create(CombatUnitInfo source)
        {
            var result = ReferencePool.Acquire<CombatActionArgs>();
            result.m_ActionSource = source;
            return result;
        }
    }
}