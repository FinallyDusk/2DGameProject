using GameFramework;
using GameFramework.DataTable;
using GameFramework.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    /// <summary>
    /// 游戏关卡系统，负责加载各个关卡
    /// </summary>
    /// 先加载基础地形模板（Prefab），然后通过其中的参数进行随机化生成怪物和道具
    public class GameCheckpointSystem : BaseSystem
    {
        /// <summary>
        /// 默认地图显示物体的路径
        /// </summary>
        private const string DEFAULT_MAP_DISPLAY_ITEM_PATH = "DefaultMapDisplayItemPath";

        /// <summary>
        /// 关卡父物体
        /// </summary>
        private Transform m_CheckpointParent;
        private IDataTable<GameCheckpointDataRow> m_AllData;
        /// <summary>
        /// 已经加载的地图资源模板
        /// </summary>
        private Dictionary<int, GameObject> m_LoadMapTemplate;
        private LoadAssetCallbacks m_LoadMapAssetCallbacks;
        /// <summary>
        /// 加载中的地图ID
        /// </summary>
        private int m_LoadingMapId;
        /// <summary>
        /// 加载地图成功回调
        /// </summary>
        private System.Action<int> m_LoadMapSuccessCallback;

        /// <summary>
        /// 当前关卡数据
        /// </summary>
        private GameCheckpointDataRow m_CurrentCheckpointData;
        /// <summary>
        /// 当前地图资源
        /// </summary>
        private CheckpointMapLogic m_CurrentMapAsset;
        /// <summary>
        /// 当前地图所有格子的标签
        /// </summary>
        private Dictionary<Vector3Int, TileTag> m_MapAllTags;
        /// <summary>
        /// 各个地图转移的key
        /// </summary>
        private int m_IntersectionKey;
        /// <summary>
        /// 地图上所有怪物单位组（存储的是<see cref="Unit"/>的ID）
        /// </summary>
        private Dictionary<int, List<int>> m_MapAllEnemyUnitGroup;
        /// <summary>
        /// 地图上所有显示物品
        /// </summary>
        private Dictionary<int, MapDisplayItemLogic> m_AllMapDisplayItems;

        //关于加载地图是否完成的判断
        private int m_LoadUnitCount;
        /// <summary>
        /// 单位加载是否完成
        /// </summary>
        private bool m_LoadUnitFinsh;
        /// <summary>
        /// 加载玩家单位是否完成
        /// </summary>
        private bool m_LoadPlayerUnitFinsh;

        

        public override void OnInit()
        {
            m_LoadMapTemplate = new Dictionary<int, GameObject>();
            m_LoadMapAssetCallbacks = new LoadAssetCallbacks(LoadMapSuccessCallback, LoadMapFailureCallback);
            m_LoadingMapId = -1;
            m_MapAllTags = new Dictionary<Vector3Int, TileTag>();
            m_MapAllEnemyUnitGroup = new Dictionary<int, List<int>>();
            m_AllMapDisplayItems = new Dictionary<int, MapDisplayItemLogic>();
        }

        protected override void InternalPreLoadResources()
        {
            m_CheckpointParent = ((MonoBehaviour)GameEntry.Entity.GetEntityGroup(EntityGroup.Scene.ToString()).Helper).transform;
            //获取所有关卡数据
            m_AllData = GameEntry.DataTable.GetDataTable<GameCheckpointDataRow>(DataTableName.GAME_CHECKPOINT_DATA_NAME);

            m_IntersectionKey = 0;
            PreLoadFinsh();
        }

        public TileTag GetTileTag(Vector3Int pos)
        {
            if (m_MapAllTags.TryGetValue(pos, out TileTag result))
            {
                return result;
            }
            //此处暂时这样做
            if (m_CurrentMapAsset != null && m_CurrentMapAsset.IsAdornTile(pos)) return TileTag.Wall;
            return TileTag.Normal;
        }

        /// <summary>
        /// 获取Group中的所有单位实例ID
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<int> GetMapUnitInstanceIDs(int groupID)
        {
            if (m_MapAllEnemyUnitGroup.TryGetValue(groupID, out var result))
            {
                return result;
            }
            Log.Error($"查找不到对应的GroupID = {groupID}，将无法构建敌方单位");
            return null;
        }

        /// <summary>
        /// 获得当前地图实体
        /// </summary>
        /// <returns></returns>
        public CheckpointMapLogic GetCurrentMap()
        {
            return m_CurrentMapAsset;
        }

        /// <summary>
        /// 隐藏当前地图
        /// </summary>
        public void HideCurrentMap()
        {
            m_CurrentMapAsset.gameObject.SetActive(false);
            foreach (var item in m_AllMapDisplayItems)
            {
                item.Value.SetActive(false);
            }
        }

        /// <summary>
        /// 显示当前地图
        /// </summary>
        public void DisplayCurrentMap()
        {
            m_CurrentMapAsset.gameObject.SetActive(true);
            foreach (var item in m_AllMapDisplayItems)
            {
                item.Value.SetActive(true);
            }
        }

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="mapId"></param>
        public void LoadMap(int mapId, System.Action<int> loadMapSuccessCallback)
        {
            m_LoadMapSuccessCallback = loadMapSuccessCallback;
            
            m_LoadUnitFinsh = false;
            m_LoadPlayerUnitFinsh = false;

            //如果地图已经存在，则直接进行生成
            if (m_LoadMapTemplate.ContainsKey(mapId))
            {
                InternalGenerateMap(mapId);
            }
            else
            {
                //如果预制体不存在，则需要读取资源
                var row = m_AllData.GetDataRow(mapId);
                if (row == null)
                {
                    Log.Fatal($"地图加载出错，mapId = {mapId}");
                    return;
                }
                m_LoadingMapId = mapId;
                GameEntry.Resource.LoadAsset(row.SceneAssetPath, m_LoadMapAssetCallbacks, this);
            }
        }

        /// <summary>
        /// 内部生成地图
        /// </summary>
        private void InternalGenerateMap(int mapId)
        {
            GameEntry.Event.Subscribe(CreateUnitSuccessEventArgs.EventID, CreateUnitSuccessCallback);
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventID, LoadMapDisplayItemSuccessCallback);

            m_CurrentCheckpointData = m_AllData.GetDataRow(mapId);
            //需要进行的操作如下
            //1. 删除之前生成的地图，物品和敌人
            //2. 生成新的地图，物品和敌人
            var template = m_LoadMapTemplate[mapId];
            m_CurrentMapAsset = Object.Instantiate(template, m_CheckpointParent).GetComponent<CheckpointMapLogic>();
            m_CurrentMapAsset.SetActive(true);
            //2.1 生成敌人（异步）
            InternalGenerateUnit();
            //清除地图上的所有标签
            m_MapAllTags.Clear();

            //3. 地图生成完成后移动玩家单位
            InternalLoadPlayerUnit();

            CheckLoadMapFinsh();
        }

        /// <summary>
        /// 地图内部加载玩家单位
        /// </summary>
        private void InternalLoadPlayerUnit()
        {
            Vector3 pos = Vector3.zero;
            if (m_IntersectionKey == 0 && m_CurrentCheckpointData.PlayerUnitInitPosition.ContainsKey(m_IntersectionKey))
            {
                var t = m_CurrentCheckpointData.PlayerUnitInitPosition[m_IntersectionKey];
                pos = new Vector3(t[0], t[1], 0);
            }
            if (GameMain.PlayerMapUnit == null)
            {
                LoadMapDisplayItemArgs arg = ReferencePool.Acquire<LoadMapDisplayItemArgs>();
                arg.GroupID = 100;
                arg.AllUnitBaseID = new int[GameMain.GetPlayerAllUnitCount()];
                for (int i = 0; i < arg.AllUnitBaseID.Length; i++)
                {
                    arg.AllUnitBaseID[i] = GameMain.GetPlayerUnitByIndex(i).BaseData.Id;
                }
                arg.Position = pos;
                arg.Player = true;

                GameEntry.Entity.ShowEntity<MapDisplayItemLogic>(GameEntry.Unit.GetCurrentEntityID(), GameEntry.Config.GetString(DEFAULT_MAP_DISPLAY_ITEM_PATH), EntityGroup.Unit, arg);
            }
            else
            {
                m_LoadPlayerUnitFinsh = true;
                GameMain.PlayerMapUnit.TransformPosition(pos);
            }
        }

        private void InternalGenerateUnit()
        {
            var allUnitConfig = m_CurrentCheckpointData.AllUnitConfig;
            if (allUnitConfig == null)
            {
                m_LoadUnitFinsh = true;
                CheckLoadMapFinsh();
            }
            m_LoadUnitCount = 0;
            for (int i = 0; i < allUnitConfig.Length; i++)
            {
                if (allUnitConfig[i].UnitID.IsNullOrEmpty()) continue;
                //加载地图显示物体
                int groupID = 1000 + i;
                if (m_MapAllEnemyUnitGroup.ContainsKey(groupID))
                {
                    m_MapAllEnemyUnitGroup[groupID].Clear();
                }
                else
                {
                    m_MapAllEnemyUnitGroup[groupID] = new List<int>(allUnitConfig[i].UnitID.Length);
                }

                m_LoadUnitCount += allUnitConfig[i].UnitID.Length;

                LoadMapDisplayItemArgs arg = ReferencePool.Acquire<LoadMapDisplayItemArgs>();
                arg.GroupID = groupID;
                arg.AllUnitBaseID = new int[allUnitConfig[i].UnitID.Length];
                arg.Position = new Vector3(allUnitConfig[i].Position[0], allUnitConfig[i].Position[1], 0);
                for (int j = 0; j < allUnitConfig[i].UnitID.Length; j++)
                {
                    arg.AllUnitBaseID[j] = allUnitConfig[i].UnitID[j].RandomOne();
                }
                arg.Player = false;

                GameEntry.Entity.ShowEntity<MapDisplayItemLogic>(GameEntry.Unit.GetCurrentEntityID(), GameEntry.Config.GetString(DEFAULT_MAP_DISPLAY_ITEM_PATH), EntityGroup.Unit, arg);
            }
        }

        /// <summary>
        /// 加载地图显示物体成功的回调
        /// </summary>
        private void LoadMapDisplayItemSuccessCallback(object sender, System.EventArgs e)
        {
            var se = e as ShowEntitySuccessEventArgs;
            if (se == null)
            {
                return;
            }
            var arg = se.UserData as LoadMapDisplayItemArgs;
            if (arg == null)
            {
                return;
            }

            LoadUnitArgs[] args = new LoadUnitArgs[arg.AllUnitBaseID.Length];
            MapUnitShowInfo showInfo = MapUnitShowInfo.Create(arg.GroupID);
            for (int j = 0; j < arg.AllUnitBaseID.Length; j++)
            {
                LoadUnitArgs t_arg = ReferencePool.Acquire<LoadUnitArgs>();
                t_arg.UnitBaseDataID = arg.AllUnitBaseID[j];
                t_arg.UserData = this;
                t_arg.GroupID = arg.GroupID;

                //todo，加上显示信息
                args[j] = t_arg;
            }

            var firstRow = GameEntry.DataTable.GetDataRow<UnitDataRow>(DataTableName.UNIT_DATA_NAME, arg.AllUnitBaseID[0]);
            var logic = se.Entity.Logic as MapDisplayItemLogic;
            logic.SetSprite(firstRow.MapDisplayIconName, SpriteType.UnitPaint);
            if (arg.Player == false)
            {
                logic.RegisterShowInfo(showInfo);
                logic.Camp = UnitCamp.Hostility;
            }
            else
            {
                GameMain.PlayerMapUnit = logic;
                logic.RegisterShowInfo(null);
                logic.Camp = UnitCamp.Player;
                var rigibody2D = logic.gameObject.AddComponent<Rigidbody2D>();
                rigibody2D.gravityScale = 0;
            }
            logic.TransformPosition(arg.Position);
            m_AllMapDisplayItems[arg.GroupID] = logic;

            if (arg.Player == false)
            {
                for (int j = 0; j < args.Length; j++)
                {
                    GameEntry.Unit.CreateUnit(args[j].UnitBaseDataID, args[j]);
                }
            }
            else
            {
                m_LoadPlayerUnitFinsh = true;
                CheckLoadMapFinsh();
            }
        }

        private void CreateUnitSuccessCallback(object sender, System.EventArgs e)
        {
            CreateUnitSuccessEventArgs ce = e as CreateUnitSuccessEventArgs;
            if (ce == null)
            {
                return;
            }
            LoadUnitArgs arg = ce.UserData as LoadUnitArgs;
            if (arg == null)
            {
                return;
            }
            //ce.UnitInstance.Hide();

            //将单位实例ID添加进组中
            m_MapAllEnemyUnitGroup[arg.GroupID].Add(ce.UnitInstanceID);

            m_LoadUnitCount--;
            if (m_LoadUnitCount == 0)
            {
                m_LoadUnitFinsh = true;
                CheckLoadMapFinsh();
            }
        }

        /// <summary>
        /// 检测地图是否加载完全
        /// </summary>
        private void CheckLoadMapFinsh()
        {
            if (m_LoadUnitFinsh == false || m_LoadPlayerUnitFinsh == false) return;
            GameEntry.Event.Unsubscribe(CreateUnitSuccessEventArgs.EventID, CreateUnitSuccessCallback);
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventID, LoadMapDisplayItemSuccessCallback);
            //地图生成完成
            m_LoadMapSuccessCallback?.Invoke(m_CurrentCheckpointData.Id);
        }

        private void LoadMapSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            if (userData != this) return;
            var template = asset as GameObject;
            m_LoadMapTemplate.Add(m_LoadingMapId, template);
            InternalGenerateMap(m_LoadingMapId);
            m_LoadingMapId = -1;
        }

        private void LoadMapFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            if (userData != this) return;
            Log.Fatal($"地图加载出错， id = {m_LoadingMapId}, assetName = {assetName}， status = {status}， errorMessage = {errorMessage}");
        }

        public override void OnExit()
        {
            m_CheckpointParent = null;
            m_AllData = null;
            m_LoadMapTemplate.Clear();
            m_LoadingMapId = 0;
            m_CurrentCheckpointData = null;
            m_CurrentMapAsset = null;
            m_MapAllEnemyUnitGroup.Clear();
        }

        /// <summary>
        /// 加载单位参数
        /// </summary>
        private class LoadUnitArgs : IReference
        {
            public int UnitBaseDataID;
            public object UserData;
            public int GroupID;

            public void Clear()
            {
                
            }
        }

        /// <summary>
        /// 加载地图显示物体参数
        /// </summary>
        private class LoadMapDisplayItemArgs : IReference
        {
            public int GroupID;
            public int[] AllUnitBaseID;
            public Vector3 Position;
            public bool Player;


            public void Clear()
            {
                
            }
        }
    }
}