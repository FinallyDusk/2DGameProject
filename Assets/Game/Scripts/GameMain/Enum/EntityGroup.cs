using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 实体各个层级
	/// </summary>
	public enum EntityGroup
	{
		/// <summary>
		/// 场景层
		/// </summary>
		Scene = 0,
		/// <summary>
		/// 物品层
		/// </summary>
		Item = 3,
		/// <summary>
		/// 单位层
		/// </summary>
		Unit = 5,
		/// <summary>
		/// 鼠标指引者
		/// </summary>
		MouseGuide = 11,
	}
}