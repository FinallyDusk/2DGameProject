using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 单位属性来源分类
	/// </summary>
	public enum UnitPropCategory
	{
		/// <summary>
		/// 基础属性
		/// </summary>
		Base,
		/// <summary>
		/// 装备属性
		/// </summary>
		Equip,
		/// <summary>
		/// 技能和buff类属性
		/// </summary>
		SkillAndBuff,
		/// <summary>
		/// 其他
		/// </summary>
		Other,
	}
}