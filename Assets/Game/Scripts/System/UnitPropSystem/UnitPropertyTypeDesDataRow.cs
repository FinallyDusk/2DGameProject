using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    /// <summary>
    /// 属性类型描述数据行
    /// </summary>
	public class PropertyTypeDesDataRow : DataRowBase
	{
        public override int Id { get { return id; } }
        [JsonProperty]
        private int id;
        [JsonProperty]
        public string PropName { get; private set; }

        [JsonProperty]
        public string PropDes { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            var row = JsonTool.LoadJson<PropertyTypeDesDataRow>(dataRowString);
            id = row.id;
            PropName = row.PropName;
            PropDes = row.PropDes;
            return true;
        }

    }
}