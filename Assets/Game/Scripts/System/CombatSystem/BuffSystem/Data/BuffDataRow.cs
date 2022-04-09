using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	/// <summary>
	/// buff的基本数据
	/// </summary>
	public class BuffDataRow : DataRowBase
	{
        public override int Id { get { return m_Id; } }
		[JsonProperty]
		[ShowInInspector][LabelText("编号")]
		private int m_Id;
		[JsonProperty]
		[ShowInInspector]
		[LabelText("名称")]
		public string Name { get; private set; }
		[JsonProperty]
		[ShowInInspector]
		[LabelText("图片名")]
		public string IconName { get; private set; }
		[JsonProperty]
		[ShowInInspector]
		[LabelText("描述")]
		public string Des { get; private set; }
		/// <summary>
		/// 触发条件
		/// </summary>
		[JsonProperty]
		[ShowInInspector]
		[LabelText("效果触发条件")]
		public EffectConditionArg TakeEffectCondition { get; private set; }
		/// <summary>
		/// 衰减的条件
		/// </summary>
		[JsonProperty]
		[ShowInInspector]
		[LabelText("持续时间衰减条件")]
		public EffectConditionArg AttenuationCondition { get; private set; }
		/// <summary>
		/// 额外加成属性
		/// </summary>
		[JsonProperty]
		[ShowInInspector]
		[LabelText("额外加成属性(buff添加上就有)")]
		public AdditionProperty[] ExtraAddProperty { get; private set; }
		/// <summary>
		/// 特殊效果脚本
		/// </summary>
		[JsonProperty]
		[ShowInInspector]
		[LabelText("满足效果触发条件时运行的脚本")]
		public string EffectScriptFile { get; private set; }
		/// <summary>
		/// 效果持续次数，满足衰减条件时才会减少，当衰减条件为无时，表示为永久性buff
		/// </summary>
		[JsonProperty]
		[ShowInInspector]
		[LabelText("持续次数(当衰减条件没有时，表示无限触发)")]
		public int DurationCount { get; private set; }
		/// <summary>
		/// buff的特性标签
		/// </summary>
		[JsonProperty]
		[ShowInInspector]
		[LabelText("buff自身特性")]
		public BuffTags[] SelfTags { get; private set; }
		/// <summary>
		/// 可以给目标单位附带的标签
		/// </summary>
		[JsonProperty]
		[ShowInInspector]
		[LabelText("buff给目标单位添加的特性")]
		public UnitEffectTags[] EffectTags { get; private set; }
		/// <summary>
		/// buff种类
		/// </summary>
		[JsonProperty]
		[ShowInInspector]
		[LabelText("buff种类")]
		public BuffCategory Category { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
		{
			var row = JsonTool.LoadJson<BuffDataRow>(dataRowString);
			m_Id = row.m_Id;
			Name = row.Name;
			IconName = row.IconName;
			Des = row.Des;
			TakeEffectCondition = row.TakeEffectCondition;
			AttenuationCondition = row.AttenuationCondition;
			DurationCount = row.DurationCount;
			ExtraAddProperty = row.ExtraAddProperty;
			EffectScriptFile = row.EffectScriptFile;
			SelfTags = row.SelfTags;
			EffectTags = row.EffectTags;
			Category = row.Category;
			return true;
		}
    }


}