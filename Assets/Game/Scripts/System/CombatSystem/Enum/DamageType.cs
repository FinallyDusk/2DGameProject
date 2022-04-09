using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 伤害类型
	/// </summary>
	public enum HarmType
	{
		/// <summary>
		/// 物理伤害
		/// </summary>
		Physical,
		/// <summary>
		/// 魔法伤害
		/// </summary>
		Magic,
		/// <summary>
		/// 真实伤害，不受加成、暴击、闪避、减免的影响
		/// </summary>
		Real,
	}
}