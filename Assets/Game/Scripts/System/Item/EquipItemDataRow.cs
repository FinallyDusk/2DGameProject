using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using Newtonsoft.Json;
using System.Data;

namespace BoringWorld
{
    //[JsonObject(MemberSerialization.Fields)]
    public class EquipItemDataRow : ItemDataRow, IReference
    {
        /// <summary>
        /// 装备等级限制
        /// </summary>
        [JsonProperty]
        public int LevelLimit { get; private set; }
        /// <summary>
        /// 装备类型
        /// </summary>
        [JsonProperty]
        public EquipType EquipType { get; private set; }
        /// <summary>
        /// 装备固定属性
        /// </summary>
        [JsonProperty]
        public EquipPropData[] EquipProp { get; private set; }
        /// <summary>
        /// 额外属性ID，<see cref="EquipExtraPropDataRow.id"/>,可以重复
        /// </summary>
        [JsonProperty]
        private int[] ExtraPropIDs { get; set; }
        /// <summary>
        /// 详细额外属性，不需要填写，只在生成实例时随机出现并保存
        /// </summary>
        [JsonProperty]
        public EquipExtraPropDataRow[] ExtraProps { get; private set; }
        /// <summary>
        /// 附加的额外技能库
        /// </summary>
        [JsonProperty]
        private int[] ExtraSkillIDs { get; set; }
        public int[] ExtraSkills { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            //增加属性时需要注意
            //----------------------
            //需要在GenerateInstance时将新加的属性赋值！！！！
            //----------------------
            EquipItemDataRow row = JsonTool.LoadJson<EquipItemDataRow>(dataRowString);
            id = row.id;
            Name = row.Name;
            Des = row.Des;
            IconName = row.IconName;
            Sellable = row.Sellable;
            SellPrice = row.SellPrice;
            BuyPrice = row.BuyPrice;
            HeapUp = row.HeapUp;
            Rarity = row.Rarity;
            LevelLimit = row.LevelLimit;
            EquipType = row.EquipType;
            EquipProp = row.EquipProp;
            ExtraPropIDs = row.ExtraPropIDs;
            //MinExtraPropCount = row.MinExtraPropCount;
            //MaxExtraPropCount = row.MaxExtraPropCount;
            ExtraProps = row.ExtraProps;
            ExtraSkillIDs = row.ExtraSkillIDs;
            //MinSkillCount = row.MinSkillCount;
            //MaxSkillCount = row.MaxSkillCount;
            ExtraSkillIDs = row.ExtraSkillIDs;
            row.Release();
            return true;
        }

        /// <summary>
        /// 获得装备属性
        /// </summary>
        /// <param name="properties"></param>
        public void GetEquipProperty(List<AdditionProperty> allProps)
        {
            //先增加自身固定属性
            for (int i = 0; i < EquipProp.Length; i++)
            {
                EquipProp[i].GetEquipProp(allProps);
            }
            for (int i = 0; i < ExtraProps.Length; i++)
            {
                ExtraProps[i].GetEquipProperty(allProps);
            }
        }

        public override string GetItemDes()
        {
            return GetOrProduceDes();
        }

        private string ItemDes = string.Empty;

        private string GetOrProduceDes()
        {
            if (ItemDes == string.Empty)
            {
                var table = GameEntry.DataTable.GetDataTable<RarityDataRow>(DataTableName.ITEM_RARITY_DATA_NAME);
                var rarityData = table.GetDataRow(Rarity);
                string fixedDataDes = string.Empty;
                for (int i = 0; i < EquipProp.Length; i++)
                {
                    fixedDataDes += EquipProp[i].GetDes();
                }
                string extraDataDes = string.Empty;
                for (int i = 0; i < ExtraProps.Length; i++)
                {
                    extraDataDes += ExtraProps[i].GetPropDes();
                }
                ItemDes = /*$"<size=30>{Name.ToString().AddColor(rarityData.Color)}</size>\n" +*/
                          $"<size=24>{GameEntry.Config.GetString(EquipType.ToString())}\n" + // {rarityData.Des}
                          $"需要等级 {LevelLimit}</size>\n" +
                          $"<line-height=5>\n<size=80%><color=#828282>{Des}</color></size>\n</line-height>\n" +
                          $"{fixedDataDes}{extraDataDes}\n" //+
                          //todo，此处还需要填写技能模板
                          /*$"<color=#d9d919>出售价格 {SellPrice.ToString()}金币</color>"*/;
            }
            return ItemDes;
        }

        public void Clear()
        {

        }

        /// <summary>
        /// 生成装备对象实例，会随机出现属性，不使用之后要记得回收
        /// </summary>
        /// <param name="oriID"></param>
        /// <returns></returns>
        public static EquipItemDataRow GenerateInstance(int oriID)
        {
            var table = GameEntry.DataTable.GetDataTable<EquipItemDataRow>(DataTableName.Equip.EQUIP_ITEM_DATA_NAME);
            var data = table.GetDataRow(oriID);
            return GenerateInstance(data);
        }

        /// <summary>
        /// 生成装备对象实例，会随机出现属性，不使用之后要记得回收
        /// </summary>
        /// <param name="ori"></param>
        /// <returns></returns>
        public static EquipItemDataRow GenerateInstance(EquipItemDataRow ori)
        {
            var rarity = GameEntry.DataTable.GetDataRow<RarityDataRow>(DataTableName.ITEM_RARITY_DATA_NAME, ori.Rarity);

            EquipItemDataRow result = ReferencePool.Acquire<EquipItemDataRow>();
            result.id = ori.id;
            result.Name = ori.Name;
            result.Des = ori.Des;
            result.IconName = ori.IconName;
            result.Sellable = ori.Sellable;
            result.SellPrice = ori.SellPrice;
            result.BuyPrice = ori.BuyPrice;
            result.HeapUp = ori.HeapUp;
            result.Rarity = ori.Rarity;
            result.LevelLimit = ori.LevelLimit;
            result.EquipType = ori.EquipType;
            result.EquipProp = ori.EquipProp.Clone() as EquipPropData[];
            //生成随机属性
            //如果没有随机属性就生成
            if (result.ExtraProps == null)
            {
                if (ori.ExtraPropIDs.IsNullOrEmpty())
                {
                    result.ExtraProps = System.Array.Empty<EquipExtraPropDataRow>();
                }
                else
                {
                    int extraPropCount = Random.Range(rarity.MinExtraPropCount, rarity.MaxExtraPropCount + 1);
                    result.ExtraProps = new EquipExtraPropDataRow[extraPropCount];
                    for (int i = 0; i < extraPropCount; i++)
                    {
                        result.ExtraProps[i] = EquipExtraPropDataRow.GenerateInstance(ori.ExtraPropIDs[Random.Range(0, ori.ExtraPropIDs.Length)]);
                    }
                }
            }
            //生成随机技能
            if (result.ExtraSkills == null)
            {
                if (ori.ExtraSkillIDs.IsNullOrEmpty())
                {
                    result.ExtraSkills = System.Array.Empty<int>();
                }
                else
                {
                    int extraSkillCount = Random.Range(rarity.MinSkillCount, rarity.MaxSkillCount + 1);
                    result.ExtraSkills = new int[extraSkillCount];
                    for (int i = 0; i < extraSkillCount; i++)
                    {
                        result.ExtraSkills[i] = ori.ExtraSkillIDs[Random.Range(0, ori.ExtraSkillIDs.Length)];
                    }
                }
            }
            return result;
        }
    }
}