using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 效果所有触发代码，由战斗中各种事件触发，具体请查看<see cref="Combat.CombatEventArgs"/>
	/// </summary>
	public enum EffectTriggerCode
	{
		/// <summary>
		/// 准备开始战斗
		/// </summary>
		[LabelText("请不要使用这个条件")]
		PrepareCombatStart = -1,
		/// <summary>
		/// 无任何触发代码,注：buff使用时表示立即生效
		/// </summary>
		[LabelText("无"), Tooltip("给buff使用时表示立即此buff立即生效")]
		None,
		/// <summary>
		/// 战斗开始，一场战斗中只会在战斗开始时触发
		/// </summary>
		[LabelText("战斗开始")]
		CombatStart,
		/// <summary>
		/// 回合开始，注：是每个单位的回合开始，如果需要自己的回合开始，请自己判断
		/// </summary>
		[LabelText("回合开始")]
		TurnStart,
		/// <summary>
		/// 回合结束
		/// </summary>
		[LabelText("回合结束")]
		TurnEnd,
		/// <summary>
		/// 属性发生改变，注：不包括当前Hp与当前Mp发生变化，如果需要自己的属性发生变化，请自己判断
		/// </summary>
		[LabelText("属性变化")]
		PropertyChange,
		/// <summary>
		/// 战斗结束
		/// </summary>
		[LabelText("战斗结束")]
		CombatEnd,
		/// <summary>
		/// 造成伤害前
		/// </summary>
		[LabelText("伤害流程-计算完伤害加成后")]
		CalcHarm_AdditionAfter,
		/// <summary>
		/// 伤害加成前
		/// </summary>
		[LabelText("伤害流程前")]
		CalcHarm_Before,
		/// <summary>
		/// 伤害流程全部执行完毕后
		/// </summary>
		[LabelText("伤害流程-全部完成后")]
		CalcHarm_Complete,
	}
}