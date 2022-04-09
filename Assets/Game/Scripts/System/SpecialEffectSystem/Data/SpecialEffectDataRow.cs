using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    /// <summary>
    /// 特效数据类
    /// </summary>
    public class SpecialEffectDataRow : DataRowBase
    {
        public override int Id
        {
            get
            {
                return id;
            }
        }
        [JsonProperty][ShowInInspector]
        private int id;
        /// <summary>
        /// 资源名，便于记忆
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        public string AssetName { get; private set; }
        /// <summary>
        /// 特效资源路径
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        public string AssetPath { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            var row = JsonTool.LoadJson<SpecialEffectDataRow>(dataRowString);
            id = row.id;
            AssetName = row.AssetName;
            AssetPath = row.AssetPath;
            return true;
        }
    }
}