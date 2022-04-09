using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 图片类型
	/// </summary>
	/// 增加值时记得到<see cref="SpriteSystem.InternalPreLoadResources"/>方法中增加预读资源
	public enum SpriteType
	{
		/// <summary>
		/// 单位立绘头像
		/// </summary>
		UnitPaint,
		/// <summary>
		/// 战斗界面行动条图标
		/// </summary>
		CombatActionIcon,
		/// <summary>
		/// buff图标
		/// </summary>
		Buff,
	}
}