using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	public enum UIFormType
	{
		/// <summary>
		/// 游戏标题界面UI
		/// </summary>
		GameTitle = 100,
		/// <summary>
		/// 游戏主界面
		/// </summary>
		GameMainInterface = 101,
		/// <summary>
		/// 游戏主菜单
		/// </summary>
		GameMenu = 200,
		/// <summary>
		/// 战斗界面UI
		/// </summary>
		CombatForm = 201,
		/// <summary>
		/// 人物信息界面
		/// </summary>
		RoleInfoForm = 301,
		/// <summary>
		/// 人物技能界面
		/// </summary>
		RoleSkillForm = 302,
		/// <summary>
		/// 人物装备界面
		/// </summary>
		RoleEquipForm = 303,
		/// <summary>
		/// 背包界面
		/// </summary>
		BackpackForm = 304,
		/// <summary>
		/// 加载进度界面
		/// </summary>
		LoadProgress = 400,
		/// <summary>
		/// 提示面板
		/// </summary>
		TipForm = 401,
	}
}