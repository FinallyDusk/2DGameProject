using GameFramework.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	/// <summary>
	/// 游戏全局信息类
	/// </summary>
	public static class GameMain
	{
		public static string EntryMapId = "EntryMapId";
		public static MouseGuide MouseGuide;
		public static GameStatus GameStatus { get; set; }
		private static LoadProgressFormLogic m_LoadProgress;
		/// <summary>
		/// 加载进度设置
		/// </summary>
		public static LoadProgressFormLogic LoadProgress
        {
            get
            {
				return m_LoadProgress;
            }
            set
            {
				if (m_LoadProgress == null && value != null)
                {
					m_LoadProgress = value;
                }
            }
        }

		/// <summary>
		/// 显示加载进度界面
		/// </summary>
		/// <param name="loadFinshCallback"></param>
		public static void ShowLoadProgressForm(System.Action loadFinshCallback)
        {
			GameEntry.UI.OpenUIFormByType(UIFormType.LoadProgress, loadFinshCallback);
        }

		#region 玩家单位操作相关

		private static MapDisplayItemLogic m_PlayerMapUnit;

		/// <summary>
		/// 玩家地图显示单位
		/// </summary>
		public static MapDisplayItemLogic PlayerMapUnit
        {
            get
            {
				return m_PlayerMapUnit;
            }
            set
            {
				m_PlayerMapUnit = value;
            }
        }

		/// <summary>
		/// 玩家所有可操作单位
		/// </summary>
		private static List<Unit> m_PlayerAllUnit;

		/// <summary>
		/// 增加玩家可操作单位
		/// </summary>
		public static void AddPlayerUnit(Unit unit)
        {
			if (m_PlayerAllUnit.Contains(unit))
            {
				return;
            }
			m_PlayerAllUnit.Add(unit);
        }

		public static Unit GetPlayerUnitByIndex(int index)
        {
			if (index < 0 || index >= m_PlayerAllUnit.Count)
            {
				Log.Error($"请求了错误的索引<color=#ff0000>{index}</color>，当前玩家可操作单位有<color=#00ff00>{m_PlayerAllUnit.Count}</color>个");
				return null;
            }
			return m_PlayerAllUnit[index];
        }

		public static int GetPlayerAllUnitCount()
        {
			return m_PlayerAllUnit.Count;
        }

		#endregion

		public static void StartGame()
        {
			m_PlayerAllUnit = new List<Unit>();
		}

		/// <summary>
		/// 配置
		/// </summary>
		public static class Config
        {
			/// <summary>
			/// 移动间隔时间
			/// </summary>
			public static float MoveIntervalTime { get; private set; }
			/// <summary>
			/// 血量恢复颜色
			/// </summary>
			public static string RestoreHpColor { get; private set; }

			public static void OnInit()
            {
				MoveIntervalTime = GameEntry.Config.GetFloat("MoveIntervalTime");
				RestoreHpColor = GameEntry.Config.GetString("RestoreHpColor");
            }
        }
	}
}