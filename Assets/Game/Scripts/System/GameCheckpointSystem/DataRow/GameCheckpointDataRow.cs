using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    /// <summary>
    /// todo，游戏关卡数据，只是暂时先做个简单的，并未深入实现
    /// </summary>
	public class GameCheckpointDataRow : DataRowBase
	{
        [JsonIgnore]
		public override int Id
        {
            get
            {
                return id;
            }
        }

        [JsonProperty][ShowInInspector][LabelText("编号")]
        private int id;
        /// <summary>
        /// 场景资源路径
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("地图场景预制体路径")]
        public string SceneAssetPath { get; private set; }
        /// <summary>
        /// 玩家单位进入场景时的初始位置
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("玩家进入场景时的初始位置(-1为读档点)")]
        public Dictionary<int, int[]>  PlayerUnitInitPosition { get; private set; }
        /// <summary>
        /// 该地图中所有单位配置
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("地图上的其他单位配置")]
        public CheckpointUnitConfig[] AllUnitConfig { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            var row = JsonTool.LoadJson<GameCheckpointDataRow>(dataRowString);
            id = row.id;
            SceneAssetPath = row.SceneAssetPath;
            PlayerUnitInitPosition = row.PlayerUnitInitPosition;
            AllUnitConfig = row.AllUnitConfig;
            return true;
        }
    }

    /// <summary>
    /// 关卡中敌人配置
    /// </summary>
    public class CheckpointUnitConfig
    {
        /// <summary>
        /// 坐标
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("坐标")]
        public int[] Position { get; private set; }
        /// <summary>
        /// 单位组ID，第二维表示从中随机出一个单位基本数据ID
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        [LabelText("单位随机ID（每一组随机一个出来）")]
        public int[][] UnitID { get; private set; }
    }
}