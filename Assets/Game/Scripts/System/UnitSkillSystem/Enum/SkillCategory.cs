using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 技能种类
	/// </summary>
	public enum SkillCategory
	{
		/// <summary>
		/// 主动技能
		/// </summary>
		[LabelText("主动技能")]
		Initiative,
		/// <summary>
		/// 被动技能
		/// </summary>
		[LabelText("被动技能")]
		Passivity,
	}
}