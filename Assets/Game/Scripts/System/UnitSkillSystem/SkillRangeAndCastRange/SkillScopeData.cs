using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 技能范围信息，数组表示各个等级的属性，只有一个时表示全等级相同
	/// </summary>
	public class SkillScopeData
	{
		/// <summary>
		/// 技能施法范围类型
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("施法范围类型")]
		public SkillCastRangeCategory[] CastRangeCategory { get; private set; }
		/// <summary>
		/// 技能施法范围
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("施法范围")]
		public int[] CastRange { get; private set; }
		/// <summary>
		/// 施法目标
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("施法目标")]
		public SkillCastTarget[] CastTarget { get; private set; }
		/// <summary>
		/// 技能生效范围类型
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("效果范围类型(具体请看属性说明)")]
		public SkillEffectiveRangeCategory[] EffectiveRangeCategory { get; private set; }
		/// <summary>
		/// 技能生效距离
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("效果作用范围")]
		public int[] EffectiveDistance { get; private set; }
	}

	/// <summary>
	/// 可施法范围类型
	/// </summary>
	public enum SkillCastRangeCategory
    {
		/// <summary>
		/// 正方形
		/// </summary>
		[LabelText("正方形")]
		Square,
		/// <summary>
		/// 圆形
		/// </summary>
		[LabelText("圆形")]
		Circle,
		/// <summary>
		/// 一行，表示单位所在整行皆为可施法范围
		/// </summary>
		[LabelText("一行")]
		Row,
    }

	/// <summary>
	/// 技能施法目标，和<see cref="SkillEffectTarget"/>不一样，这是在选择范围时使用
	/// </summary>
	[System.Flags]
	public enum SkillCastTarget
    {
		/// <summary>
		/// 无目标，立即释放技能，不需要选择位置
		/// </summary>
		[LabelText("直接释放")]
		NoTarget = 1,
		/// <summary>
		/// 只允许选择敌方单位
		/// </summary>
		[LabelText("敌方目标")]
		Enemy = 2,
		/// <summary>
		/// 只允许选择盟友
		/// </summary>
		[LabelText("盟友")]
		Ally = 4,
		/// <summary>
		/// 无单位，只能点地板
		/// </summary>
		[LabelText("无单位")]
		NoneUnit = 8,
		/// <summary>
		/// 点目标，包括敌方单位或者盟友单位，也可以没有任何目标，不包括自己
		/// </summary>
		[LabelText("点单位")]
		Point = 14,
		/// <summary>
		/// 自己
		/// </summary>
		[LabelText("自己")]
		Self = 16,
    }

	/// <summary>
	/// 技能生效范围类型
	/// </summary>
	public enum SkillEffectiveRangeCategory
    {
		/// <summary>
		/// 一行，<see cref="SkillScopeData.EffectiveDistance"/>为-1时表示一整行都是范围内（<see cref="SkillScopeData.CastTarget"/>必须为<see cref="SkillCastTarget.NoTarget"/>），否则为选择方向的范围，无视距离
		/// </summary>
		/// 0	0	0	0
		/// 0	0	0	0
		/// 2	1	1	1
		/// 0	0	0	0
		/// 如上所示，2为施法单位，1为生效范围，0为未生效范围
		[LabelText("一行")]
		Row,
		/// <summary>
		/// 正方形
		/// </summary>
		/// 0	0	0	0	0
		/// 0	1	1	1	0
		/// 0	1	2	1	0
		/// 0	1	1	1	0
		/// 0	0	0	0	0
		/// 如上所示，2为施法单位，1为生效范围，0为未生效范围
		[LabelText("正方形")]
		Square,
		/// <summary>
		/// 圆形
		/// </summary>
		/// 0	0	1	0	0
		/// 0	1	1	1	0
		/// 1	1	2	1	1
		/// 0	1	1	1	0
		///	0	0	1	0	0
		/// 如上所示，2为施法单位，1为生效范围，0为未生效范围
		[LabelText("圆形")]
		Circle,
    }

}