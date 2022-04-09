using System.Collections;
using System.Collections.Generic;
using GameFramework;
using Newtonsoft.Json;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    //[JsonObject(MemberSerialization.Fields)]
    public class EquipExtraPropDataRow : DataRowBase, IReference
    {
        [JsonProperty]
        private int id;
        public override int Id { get { return id; } }
        [JsonProperty]
        public string Des { get; private set; }
        [JsonProperty]
        public float MinValue { get; private set; }
        [JsonProperty]
        public float MaxValue { get; private set; }
        /// <summary>
        /// 具体值，此值不可直接填写，为生成时从MinValue和MaxValue随而出
        /// </summary>
        [JsonProperty]
        public double Value { get; private set; }
        [JsonProperty]
        public UnitPropType PropType { get; private set; }
        [JsonProperty]
        public UnitPropValueType ValueType { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            //增加属性之后记得在此处进行赋值和GenerateInstance方法中赋值
            EquipExtraPropDataRow row = JsonTool.LoadJson<EquipExtraPropDataRow>(dataRowString);
            id = row.id;
            Des = row.Des;
            MinValue = row.MinValue;
            MaxValue = row.MaxValue;
            Value = row.Value;
            PropType = row.PropType;
            ValueType = row.ValueType;
            return true;
        }

        private string e_ItemDes = string.Empty;

        public string GetPropDes()
        {
            if (e_ItemDes == string.Empty)
            {
                e_ItemDes = $"{PropertyToString.GetStrHasNameAndValue(ValueType, PropType, Value)}\n".AddColor("#de00ff");
            }
            return e_ItemDes;
        }

        public void GetEquipProperty(List<AdditionProperty> allProps)
        {
            for (int i = 0; i < allProps.Count; i++)
            {
                if (allProps[i].PropType == PropType && allProps[i].ValueType == ValueType)
                {
                    allProps[i].AddValue(Value);
                    return;
                }
            }
            AdditionProperty result = ReferencePool.Acquire<AdditionProperty>();
            result.PropType = PropType;
            result.Category = UnitPropCategory.Equip;
            result.ValueType = ValueType;
            result.Value = Value;
            allProps.Add(result);
        }

        /// <summary>
        /// 生成额外数据实例,不使用时记得回收
        /// </summary>
        /// <param name="sourceData"></param>
        /// <returns></returns>
        public static EquipExtraPropDataRow GenerateInstance(int oriID)
        {
            var table = GameEntry.DataTable.GetDataTable<EquipExtraPropDataRow>(DataTableName.Equip.EXTRA_PROP_DATA_NAME);
            var data = table.GetDataRow(oriID);
            return GenerateInstance(data);
        }

        /// <summary>
        /// 生成额外数据实例，不使用时记得回收
        /// </summary>
        /// <param name="sourceData"></param>
        /// <returns></returns>
        public static EquipExtraPropDataRow GenerateInstance(EquipExtraPropDataRow sourceData)
        {
            EquipExtraPropDataRow result = ReferencePool.Acquire<EquipExtraPropDataRow>();
            result.id = sourceData.id;
            result.Des = sourceData.Des;
            result.MinValue = sourceData.MinValue;
            result.MaxValue = sourceData.MaxValue;
            result.Value = Random.Range(result.MinValue, result.MaxValue + 1);
            result.PropType = sourceData.PropType;
            result.ValueType = sourceData.ValueType;
            return result;
        }

        public void Clear()
        {
            
        }
    }

}