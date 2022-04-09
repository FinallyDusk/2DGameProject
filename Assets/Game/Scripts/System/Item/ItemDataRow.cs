using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    public abstract class ItemDataRow : DataRowBase
    {
        [JsonProperty]
        protected int id;
        public override int Id
        {
            get
            {
                return id;
            }
        }
        /// <summary>
        /// 物品名称
        /// </summary>
        [JsonProperty]
        public string Name { get; protected set; }
        /// <summary>
        /// 物品描述
        /// </summary>
        [JsonProperty]
        protected string Des { get; set; }
        /// <summary>
        /// 图标名字
        /// </summary>
        [JsonProperty]
        public string IconName { get; protected set; }
        /// <summary>
        /// 是否可以出售
        /// </summary>
        [JsonProperty]
        public bool Sellable { get; protected set; }
        /// <summary>
        /// 出售价格
        /// </summary>
        [JsonProperty]
        public int SellPrice { get; protected set; }
        /// <summary>
        /// 购买价格
        /// </summary>
        [JsonProperty]
        public int BuyPrice { get; protected set; }
        /// <summary>
        /// 能否堆叠
        /// </summary>
        [JsonProperty]
        public bool HeapUp { get; protected set; }
        /// <summary>
        /// 稀有度
        /// </summary>
        [JsonProperty]
        public int Rarity { get; protected set; }

        public abstract string GetItemDes();

        private string m_RarityItemName = string.Empty;
    } 
}
