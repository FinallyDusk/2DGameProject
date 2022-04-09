using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 技能特效位置
	/// </summary>
	public enum SkillSpecialEffectPosition
	{
		/// <summary>
		/// 施法点
		/// </summary>
		CastPoint,
		/// <summary>
		/// 自身
		/// </summary>
		Self,
		/// <summary>
		/// 绝对位置
		/// </summary>
		Absolute,
		/// <summary>
		/// 相对位置
		/// </summary>
		Relative,
	}
}