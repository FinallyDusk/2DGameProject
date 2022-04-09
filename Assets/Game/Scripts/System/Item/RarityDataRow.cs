using GameFramework;
using Newtonsoft.Json;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    //[JsonObject(MemberSerialization.Fields)]
    public class RarityDataRow : DataRowBase
    {
        [JsonProperty]
        private int id;
        public override int Id { get { return id; } }
        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty]
        public string Des { get; private set; }
        /// <summary>
        /// 颜色,采用十六进制，例如#ff0000ff，可以包含透明度
        /// </summary>
        [JsonProperty]
        public string Color { get; private set; }
        [JsonProperty]
        public int MinExtraPropCount { get; private set; }
        [JsonProperty]
        public int MaxExtraPropCount { get; private set; }
        [JsonProperty]
        public int MinSkillCount { get; private set; }
        [JsonProperty]
        public int MaxSkillCount { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            RarityDataRow row = JsonTool.LoadJson<RarityDataRow>(dataRowString);
            id = row.Id;
            Des = row.Des;
            Color = row.Color;
            MinExtraPropCount = row.MinExtraPropCount;
            MaxExtraPropCount = row.MaxExtraPropCount;
            MinSkillCount = row.MinSkillCount;
            MaxSkillCount = row.MaxSkillCount;
            return true;
        }
    }
}