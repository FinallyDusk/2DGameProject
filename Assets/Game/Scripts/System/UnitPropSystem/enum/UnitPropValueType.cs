using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 单位属性值类型
	/// </summary>
	public enum UnitPropValueType
	{
		/// <summary>
		/// 固定值
		/// </summary>
		Fixed,
		/// <summary>
		/// 百分比属性，注：百分比属性是提升所有属性的百分比，具体可以查看注释里面的示例
		/// </summary>
		/// 示例：
		///    单位A属性一览（物理攻击力）：
		///			300（基础） 100（装备） 100（技能） 0（额外）
		///						30%（装备） 20%（技能） 10%（额外）
		///	   则单位A的物理攻击力 = (300 + 100 + 100 + 0) * (1 + 30% + 20% + 10%) = 800
		Percentage,
	}
}