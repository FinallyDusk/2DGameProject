using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace BoringWorld
{
	/// <summary>
	/// 单位阵营
	/// </summary>
	public enum UnitCamp
	{
		/// <summary>
		/// 玩家单位
		/// </summary>
		[LabelText("玩家")]
		Player,
		/// <summary>
		/// 中立单位
		/// </summary>
		[LabelText("中立")]
		Neutrality,
		/// <summary>
		/// 敌对
		/// </summary>
		[LabelText("敌对")]
		Hostility,
	}
}