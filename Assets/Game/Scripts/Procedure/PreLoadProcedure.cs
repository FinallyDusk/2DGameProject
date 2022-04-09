using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	/// <summary>
	/// 预加载资源流程
	/// </summary>
	/// 功能：
	/// 1. 读取所有数据表
	/// 2. 在1完成之后，系统的预加载资源（注：是系统所需资源的预加载，另外，尽量不要出现加载这个系统需要其他系统先加载的情况）
	public class PreLoadProcedure : ProcedureBase
	{
		private const int MAX_DATA_TABLE_COUNT = 10; 
		private int m_DataTabelCount;
		private const int SYSTEM_PRELOAD_COUNT = 9;
		private int m_SystemPreLoadCount;
		private const int CREATE_PLAYER_UNIT_COUNT = 1;
		private int m_CreatePlayerUnitCount;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
			//m_DataTabelCount = m_AllDataTableNames.Count;
			//打开加载界面
			GameMain.ShowLoadProgressForm(PreLoadFinsh);
			//加载数据表
			GameEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, LoadDataTableSuccessCallback);
			m_DataTabelCount = 10;

			//关卡数据--1
			var checkpointTable = GameEntry.DataTable.CreateDataTable(typeof(GameCheckpointDataRow), DataTableName.GAME_CHECKPOINT_DATA_NAME);
			checkpointTable.ReadDataByConfig(this);
			//单位数据--2
			var unitDataTable = GameEntry.DataTable.CreateDataTable(typeof(UnitDataRow), DataTableName.UNIT_DATA_NAME);
			unitDataTable.ReadDataByConfig(this);
			//单位基础属性数据--3
			var unitBasePropertyDataTable = GameEntry.DataTable.CreateDataTable(typeof(UnitBasePropDataRow), DataTableName.UNIT_BASE_PROPERTY_DATA_NAME);
			unitBasePropertyDataTable.ReadDataByConfig(this);
			//属性类型描述数据--4
			var propertyTypeDesDataTable = GameEntry.DataTable.CreateDataTable(typeof(PropertyTypeDesDataRow), DataTableName.PROPERTY_TYPE_DES_DATA_NAME);
			propertyTypeDesDataTable.ReadDataByConfig(this);
			//装备数据--5
			var equipDataTable = GameEntry.DataTable.CreateDataTable(typeof(EquipItemDataRow), DataTableName.Equip.EQUIP_ITEM_DATA_NAME);
			equipDataTable.ReadDataByConfig(this);
			//装备额外数据--6
			var equipExtraDataTable = GameEntry.DataTable.CreateDataTable(typeof(EquipExtraPropDataRow), DataTableName.Equip.EXTRA_PROP_DATA_NAME);
			equipExtraDataTable.ReadDataByConfig(this);
			//道具稀有度数据--7
			var itemRarityDataTable = GameEntry.DataTable.CreateDataTable(typeof(RarityDataRow), DataTableName.ITEM_RARITY_DATA_NAME);
			itemRarityDataTable.ReadDataByConfig(this);
			//主动技能数据--8
			var initiativeSkillDataTable = GameEntry.DataTable.CreateDataTable(typeof(SkillBaseData), DataTableName.SKILL_BASE_DATA_NAME);
			initiativeSkillDataTable.ReadDataByConfig(this);
			//特效数据--9
			var specialEffectDataTable = GameEntry.DataTable.CreateDataTable(typeof(SpecialEffectDataRow), DataTableName.SPECIAL_EFFECT_DATA_NAME);
			specialEffectDataTable.ReadDataByConfig(this);
			//buff数据--10
			var buffDataTable = GameEntry.DataTable.CreateDataTable(typeof(BuffDataRow), DataTableName.BUFF_DATA_NAME);
			buffDataTable.ReadDataByConfig(this);


			GameMain.LoadProgress.ChangeLoadTitleContent("加载数据表中...");
		}

		protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
			if (GameMain.GameStatus == GameStatus.Normal)
            {
				ChangeState<GameNormalProcedure>(procedureOwner);
				return;
            }
			//GameMain.LoadProgress.ChangeLoadProgress(1);
        }

		private void LoadDataTableSuccessCallback(object sender, System.EventArgs e)
        {
			LoadDataTableSuccessEventArgs le = e as LoadDataTableSuccessEventArgs;
			if (le.UserData != this) return;
			m_DataTabelCount--;
			if (m_DataTabelCount == 0)
            {
				SystemPreLoad();
            }
			GameMain.LoadProgress.ChangeLoadProgress(PreLoadResourceWeight.DATA_TABLE * (1f / MAX_DATA_TABLE_COUNT));
        }

		/// <summary>
		/// 各个系统的预加载
		/// </summary>
		private void SystemPreLoad()
        {
			m_SystemPreLoadCount = 9;
			float systemPreLoadProgress = PreLoadResourceWeight.SYSTEM_PRELOAD * (1f / SYSTEM_PRELOAD_COUNT);

			GameEntry.Checkpoint.PreLoad(SystemPreLoadFinsh, systemPreLoadProgress);
			GameEntry.Unit.PreLoad(SystemPreLoadFinsh, systemPreLoadProgress);
			GameEntry.Sprite.PreLoad(SystemPreLoadFinsh, systemPreLoadProgress);
			GameEntry.UnitProp.PreLoad(SystemPreLoadFinsh, systemPreLoadProgress);
			GameEntry.Backpack.PreLoad(SystemPreLoadFinsh, systemPreLoadProgress);
			GameEntry.UnitSkill.PreLoad(SystemPreLoadFinsh, systemPreLoadProgress);
			GameEntry.Combat.PreLoad(SystemPreLoadFinsh, systemPreLoadProgress);
			GameEntry.SpecialEffect.PreLoad(SystemPreLoadFinsh, systemPreLoadProgress);
			GameEntry.Lua.LoadLuaFilesConfig(() =>
			{
				SystemPreLoadFinsh();
				GameMain.LoadProgress.ChangeLoadProgress(systemPreLoadProgress);
			});
			GameMain.LoadProgress.ChangeLoadTitleContent("加载系统中...");
		}

		private void SystemPreLoadFinsh()
        {
			m_SystemPreLoadCount--;
			if (m_SystemPreLoadCount == 0)
            {
				LoadPlayerUnit();
            }
        }

		/// <summary>
		/// 加载玩家单位
		/// </summary>
		private void LoadPlayerUnit()
        {
			//此处分为新游戏和读取数据加载
			//todo
			m_CreatePlayerUnitCount = 2;
			GameEntry.Event.Subscribe(CreateUnitSuccessEventArgs.EventID, CreatePlayerUnitFinsh);
			GameEntry.Unit.CreateUnit(100, this);
			GameEntry.Unit.CreateUnit(200, this);
			GameMain.LoadProgress.ChangeLoadTitleContent("加载玩家单位...");
		}

		private void CreatePlayerUnitFinsh(object sender, System.EventArgs e)
        {
			var ce = e as CreateUnitSuccessEventArgs;
			if (ce.UserData != this)
            {
				return;
            }
			m_CreatePlayerUnitCount--;
			GameMain.LoadProgress.ChangeLoadProgress(PreLoadResourceWeight.INIT_PLAYER_UNIT * (1f / CREATE_PLAYER_UNIT_COUNT));
			//将玩家可操作单位添加到GameMain中
			GameMain.AddPlayerUnit(ce.UnitInstance);
			if (m_CreatePlayerUnitCount == 0)
            {
				//玩家单位加载完成，接下来进入地图区域
				//todo
				GameEntry.Checkpoint.LoadMap(100, LoadMapFinsh);
				GameMain.LoadProgress.ChangeLoadTitleContent("加载地图...");
			}
        }

		private void LoadMapFinsh(int mapID)
        {
			//此处为随便写，记得更改
			//todo
			if (mapID == 100)
            {
				GameMain.LoadProgress.ChangeLoadProgress(PreLoadResourceWeight.LOAD_ENTRY_MAP);
			}
			//加载鼠标指引器
			PreLoadMouseGuide();
		}

		private void PreLoadFinsh()
        {
			GameMain.GameStatus = GameStatus.Normal;
        }

		private void PreLoadMouseGuide()
		{
			//加载鼠标指引器
			GameMain.LoadProgress.ChangeLoadTitleContent("加载鼠标引导中...");
			GameEntry.Resource.LoadAsset(GameEntry.Config.GetString("MouseGuide"), new GameFramework.Resource.LoadAssetCallbacks(LoadMouseGuideSuccessCallback), this);
		}

		private void LoadMouseGuideSuccessCallback(string assetName, object asset, float duration, object userData)
		{
			if (userData != this) return;
			var obj = asset as GameObject;
			if (obj == null)
            {
				Log.Error("加载鼠标指引器出错，请检查");
				return;
            }
			var _InsObj = GameObject.Instantiate(obj, GameObject.FindGameObjectWithTag(Tags.MOUSE_GUIDE).transform);
			var sc = _InsObj.GetComponent<MouseGuide>();
			GameMain.MouseGuide = sc;
			sc.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder = (int)EntityGroup.MouseGuide;
			GameMain.LoadProgress.ChangeLoadProgress(PreLoadResourceWeight.MOUSE_GUIDE);
		}

		/// <summary>
		/// 预加载资源时的各个资源权重分配
		/// </summary>
		private class PreLoadResourceWeight
		{
			/// <summary>
			/// 鼠标指引器
			/// </summary>
			public const float MOUSE_GUIDE = 20f;
			/// <summary>
			/// 系统预加载权重
			/// </summary>
			public const float SYSTEM_PRELOAD = 30f;
			/// <summary>
			/// 数据表
			/// </summary>
			public const float DATA_TABLE = 10f;
			/// <summary>
			/// 加载入口地图
			/// </summary>
			public const float LOAD_ENTRY_MAP = 20f;
			/// <summary>
			/// 初始化玩家单位
			/// </summary>
			public const float INIT_PLAYER_UNIT = 20f;

		}
	}
}