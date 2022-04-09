using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 单位属性分类
	/// </summary>
	public enum UnitPropType
	{
		/// <summary>
		/// 最大生命值
		/// </summary>
		MaxHp,
		/// <summary>
		/// 当前生命值
		/// </summary>
		NowHp,
		/// <summary>
		/// 最大魔法值
		/// </summary>
		MaxMp,
		/// <summary>
		/// 当前魔法值
		/// </summary>
		NowMp,
		/// <summary>
		/// 每回合生命回复
		/// </summary>
		HpReply,
		/// <summary>
		/// 每回合魔法回复
		/// </summary>
		MpReply,
		/// <summary>
		/// 战斗时行动条移动速度
		/// </summary>
		CombatSpeed,
		/// <summary>
		/// 物理攻击力
		/// </summary>
		PhysicAtk,
		/// <summary>
		/// 魔法攻击力
		/// </summary>
		MagicAtk,
		/// <summary>
		/// 物伤增加（1 = 100%）
		/// </summary>
		[LabelText("物伤增加(1 = 100%)")]
		PhysicHarmAdd,
		/// <summary>
		/// 魔伤增加（1 = 100%）
		/// </summary>
		[LabelText("魔伤增加(1 = 100%)")]
		MagicHarmAdd,
		/// <summary>
		/// 最终伤害增加（1 = 100%）
		/// </summary>
		[LabelText("最终伤害增加(1 = 100%)")]
		FinallyHarmAdd,
		/// <summary>
		/// 物理防御力（1 = 100%减少物理伤害），伤害不能为负
		/// </summary>
		[LabelText("物理防御(1 = 100%)")]
		PhysicDef,
		/// <summary>
		/// 魔法防御力（1 = 100%减少魔法伤害），伤害不能为负
		/// </summary>
		[LabelText("魔法防御(1 = 100%)")]
		MagicDef,
		/// <summary>
		/// 最终伤害减少（1 = 100%伤害减少），伤害不能为负
		/// </summary>
		[LabelText("最终伤害减少(1 = 100%)")]
		FinallyHarmReduce,
		/// <summary>
		/// 暴击率（1 = 100%），除了真伤其他的任何伤害都可以暴击
		/// </summary>
		[LabelText("暴击率(1 = 100%)")]
		Crit,
		/// <summary>
		/// 暴击倍数（1 = 100%为基础伤害，即最终伤害 = 伤害 * 暴击倍数，所以暴击倍数最低为1）
		/// </summary>
		[LabelText("暴击倍数(1 = 100%)")]
		CritMulti,
		/// <summary>
		/// 闪避率（1 = 100%），任何伤害都可以闪避，优先计算闪避
		/// </summary>
		[LabelText("闪避率(1 = 100%)")]
		Evade,
	}
}