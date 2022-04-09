using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 游戏状态
	/// </summary>
	public enum GameStatus
	{
		/// <summary>
		/// 标题界面
		/// </summary>
		Title,
		/// <summary>
		/// 预加载资源状态
		/// </summary>
		PreLoad,
		/// <summary>
		/// 游玩时正常状态
		/// </summary>
		Normal,

	}
}