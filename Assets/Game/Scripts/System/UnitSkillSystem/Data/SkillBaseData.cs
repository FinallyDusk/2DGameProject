using BoringWorld.UI.RoleSkill;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    public class SkillBaseData : DataRowBase
    {
        [JsonIgnore]
        public override int Id { get { return m_Id; } }
        [JsonProperty]
        [ShowInInspector][LabelText("编号")]
        private int m_Id;
        /// <summary>
        /// 技能名称
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("技能名")]
        public string Name { get; private set; }
        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty][ShowInInspector][MultiLineProperty]
        [LabelText("技能描述")]
        public string Des { get; private set; }
        /// <summary>
        /// 技能类型
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("技能类型")]
        public SkillCategory Category { get; private set; }
        /// <summary>
        /// 解锁等级
        /// </summary>
        [JsonProperty][ShowInInspector, TableColumnWidth(20)]
        [LabelText("解锁等级")]
        public int UnLockLv { get; private set; }
        /// <summary>
        /// 升级技能的等级间隔
        /// </summary>
        [JsonProperty][ShowInInspector, TableColumnWidth(20)]
        [LabelText("技能提升等级间隔")]
        public int UpgradeLvDelta { get; private set; }
        /// <summary>
        /// 技能升级点数
        /// </summary>
        [JsonProperty][ShowInInspector, TableColumnWidth(20)]
        [LabelText("技能升级所需点数")]
        public int UpgradePoint { get; private set; }
        /// <summary>
        /// 最大等级
        /// </summary>
        [JsonProperty][ShowInInspector, TableColumnWidth(20)]
        [LabelText("技能最大等级")]
        public int MaxLv { get; private set; }
        /// <summary>
        /// 技能条件参数
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("技能触发条件(主动技能不需要填写)")]
        public EffectConditionArg SkillCondition { get; private set; }
        /// <summary>
        /// 脚本运行文件路径
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("效果执行函数名")]
        public string EffectScritpFile { get; private set; }
        /// <summary>
        /// 技能范围信息
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("技能范围信息")]
        public SkillScopeData ScopeData { get; private set; }
        [JsonProperty]
        [ShowInInspector, TableColumnWidth(20)]
        [LabelText("魔法消耗")]
        public int[] CostMp { get; private set; }
        /// <summary>
        /// 回合CD（不同技能类型有不同的用法）
        /// </summary>
        [JsonProperty]
        [ShowInInspector, TableColumnWidth(20)]
        [LabelText("冷却时间")]
        public int[] CDTurn { get; private set; }
        /// <summary>
        /// 特效配置
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("特效配置")]
        public SpecialEffectConfig SpecialEffect { get; private set; }
        /// <summary>
        /// 额外加成属性
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("技能额外加成属性(永久性)")]
        private AdditionProperty[] ExtraAddProperty { get; set; }
        /// <summary>
        /// 自定义数据
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("自定义数据(谨慎使用)")]
        public Dictionary<string, string[]> CustomData { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            var row = JsonConvert.DeserializeObject<SkillBaseData>(dataRowString);
            m_Id = row.m_Id;
            Name = row.Name;
            Des = row.Des;
            Category = row.Category;
            UnLockLv = row.UnLockLv;
            UpgradeLvDelta = row.UpgradeLvDelta;
            UpgradePoint = row.UpgradePoint;
            MaxLv = row.MaxLv;
            SkillCondition = row.SkillCondition;
            EffectScritpFile = row.EffectScritpFile;
            ScopeData = row.ScopeData;
            CostMp = row.CostMp;
            CDTurn = row.CDTurn;
            SpecialEffect = row.SpecialEffect;
            ExtraAddProperty = row.ExtraAddProperty;
            CustomData = row.CustomData;
            return true;
        }

        public int GetCostMp(int lv)
        {
            if (CostMp.IsNullOrEmpty())
            {
                return int.MinValue;
            }
            if (CostMp.Length == 1)
            {
                return CostMp[0];
            }
            return CostMp[lv - 1];
        }
        public int GetCDTurn(int lv)
        {
            if (CDTurn.IsNullOrEmpty())
            {
                return int.MinValue;
            }
            if (CDTurn.Length == 1)
            {
                return CDTurn[0];
            }
            return CDTurn[lv - 1];
        }
    }

    /// <summary>
    /// 效果触发条件
    /// </summary>
    public class EffectConditionArg
    {
        /// <summary>
        /// 触发代码，只需要满足其中一个条件即可
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("条件触发码（只需要满足其中一个即可）")]
        public EffectTriggerCode[] TriggerCode { get; private set; }
        /// <summary>
        /// 条件是否需要为自己触发的
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("是否需要为自己触发")]
        public bool SelfTrigger { get; private set; }
    }
}