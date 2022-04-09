using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	public class UIFormDataRow : DataRowBase
	{
        public override int Id { get { return id; } }
        [ShowInInspector]
        [JsonProperty]
        private int id;
        /// <summary>
        /// 界面资源路径
        /// </summary>
        [JsonProperty]
        [ShowInInspector]
        public string FormAssetPath;
        /// <summary>
        /// 所属组
        /// </summary>
        public string GroupName;

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            var row = JsonTool.LoadJson<UIFormDataRow>(dataRowString);
            this.id = row.id;
            this.FormAssetPath = row.FormAssetPath;
            this.GroupName = row.GroupName;
            return true;
        }

    }
}