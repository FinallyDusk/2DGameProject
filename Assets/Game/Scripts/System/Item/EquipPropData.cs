using GameFramework;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 装备固定属性数据
	/// </summary>
	public class EquipPropData
	{
		[JsonProperty]
		public UnitPropType PropType { get; private set; }
		/// <summary>
		/// 属性值类型
		/// </summary>
		[JsonProperty]
		public UnitPropValueType ValueType { get; private set; }
		[JsonProperty]
		public double Value { get; private set; }

		private string Des;

		public string GetDes()
        {
			if (string.IsNullOrEmpty(Des))
            {
				Des = $"{PropertyToString.GetStrHasNameAndValue(ValueType, PropType, Value)}\n";
            }
			return Des;
        }

		public void GetEquipProp(List<AdditionProperty> allProps)
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
	}
}