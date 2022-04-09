using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.Combat
{
    /// <summary>
    /// 伤害数据,需要手动回收
    /// </summary>
    public class HarmData : IReference
    {
        /// <summary>
        /// 伤害来源
        /// </summary>
        public CombatUnitInfo Source { get; private set; }
        /// <summary>
        /// 伤害目标
        /// </summary>
        public CombatUnitInfo Target { get; private set; }
        /// <summary>
        /// 基础伤害值
        /// </summary>
        public double BaseValue { get; private set; }
        /// <summary>
        /// 在伤害来源进行加成后的值
        /// </summary>
        public double AfterAdditonValue { get; private set; }
        /// <summary>
        /// 最终值
        /// </summary>
        public double FinallyValue { get; private set; }
        private HarmType m_HarmType;
        /// <summary>
        /// 伤害类型
        /// </summary>
        public HarmType HarmType
        {
            get
            {
                return m_HarmType;
            }
            set
            {
                if (value == m_HarmType) return;
                switch (value)
                {
                    case HarmType.Physical:
                    case HarmType.Magic:
                        AllowCrit = true;
                        AllowEvade = true;
                        AllowExtraAdd = true;
                        AllowExtraReduce = true;
                        break;
                    case HarmType.Real:
                        AllowCrit = false;
                        AllowEvade = false;
                        AllowExtraAdd = false;
                        AllowExtraReduce = false;
                        break;
                    default:
                        throw new System.NotImplementedException($"还未实现该类型{value}的操作");
                }
                m_HarmType = value;
            }
        }
        /// <summary>
        /// 是否发生暴击
        /// </summary>
        public bool Crit { get; private set; }
        /// <summary>
        /// 是否经过伤害加成
        /// </summary>
        public bool HarmAdd { get; private set; }
        /// <summary>
        /// 是否闪避
        /// </summary>
        public bool Evade { get; set; }
        /// <summary>
        /// 是否经过伤害减免
        /// </summary>
        public bool HarmReduce { get; private set; }
        /// <summary>
        /// 允许暴击
        /// </summary>
        public bool AllowCrit { get; private set; }
        /// <summary>
        /// 允许闪避
        /// </summary>
        public bool AllowEvade { get; private set; }
        /// <summary>
        /// 允许额外加成
        /// </summary>
        public bool AllowExtraAdd { get; private set; }
        /// <summary>
        /// 允许额外减免
        /// </summary>
        public bool AllowExtraReduce { get; private set; }
        /// <summary>
        /// 忽略防御的系数（最高1）
        /// </summary>
        public double IgnoreDefRate { get; set; }

        /// <summary>
        /// 伤害执行
        /// </summary>
        public void Execute()
        {
            var targets = new CombatUnitInfo[] { Target };
            //todo，发送伤害消息
            GameEntry.Event.FireNow(this, CombatEventArgs.Create(EffectTriggerCode.CalcHarm_Before, Source, targets));
            Source.ProcessHarm(this);
            GameEntry.Event.FireNow(this, CombatEventArgs.Create(EffectTriggerCode.CalcHarm_AdditionAfter, Source, targets));
            Target.TakeHarm(this);
            GameEntry.Event.FireNow(this, CombatEventArgs.Create(EffectTriggerCode.CalcHarm_Complete, Source, targets));
        }

        /// <summary>
        /// 设置原始数据，在伤害来源处设置
        /// </summary>
        /// <param name="afterAddtionValue">从伤害来源的最终伤害</param>
        /// <param name="crit">是否发生了暴击</param>
        /// <param name="harmAdd">是否获得了各种伤害加成</param>
        public void SetOriginalData(double afterAddtionValue, bool crit, bool harmAdd)
        {
            AfterAdditonValue = afterAddtionValue;
            Crit = crit;
            HarmAdd = harmAdd;
        }

        /// <summary>
        /// 设置最终数据，在伤害目标处设置
        /// </summary>
        /// <param name="finallyValue">最终伤害（实际减少生命值）</param>
        /// <param name="evade">是否闪避（与上面那个项不冲突，即成功闪避的情况下，finallyValue也不一定为0）</param>
        /// <param name="harmReduce">是否发生伤害减免</param>
        public void SetFinallyData(double finallyValue, bool evade, bool harmReduce)
        {
            FinallyValue = finallyValue;
            Evade = evade;
            HarmReduce = harmReduce;
        }

        public void Clear()
        {
            
        }

        public static HarmData Create(CombatUnitInfo source, CombatUnitInfo target, double baseValue, HarmType harmType, bool allowCrit = true, bool allowEvade = true, bool allowExtraAdd = true, bool allowExtraReduce = true)
        {
            HarmData result = ReferencePool.Acquire<HarmData>();
            result.Source = source;
            result.Target = target;
            result.BaseValue = baseValue;
            result.HarmType = harmType;
            result.AllowCrit = allowCrit;
            result.AllowEvade = allowEvade;
            result.AllowExtraAdd = allowExtraAdd;
            result.AllowExtraReduce = allowExtraReduce;
            return result;
        }
    }
}