using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	/// <summary>
	/// 属性变为字符串
	/// </summary>
	public static class PropertyToString
	{
        /// <summary>
        /// 无法对Hp和Mp进行特殊更改，多用于对属性提升时的获得字符串
        /// </summary>
        /// <param name="propType"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
		public static string GetStrHasNameAndValue(UnitPropValueType valueType, UnitPropType propType, double v1)
        {
            string prefix = string.Empty;
            string value = string.Empty;
            switch (valueType)
            {
                case UnitPropValueType.Fixed:
                    prefix = "<color=#ffffff>增加</color>";
                    value = GetStrOnlyValue(propType, v1);
                    break;
                case UnitPropValueType.Percentage:
                    prefix = "<color=#ff0000>提高</color>";
                    value = $"{(v1 * 100).ToString("f0")}%";
                    break;
            }
            var name = GameEntry.DataTable.GetDataRow<PropertyTypeDesDataRow>(DataTableName.PROPERTY_TYPE_DES_DATA_NAME, propType.GetHashCode()).PropName;
            return $"{prefix} {name} {value}";
        }

        /// <summary>
        /// 无法对Hp和Mp进行特殊更改，此方法是用于获得总属性的描述的方法
        /// </summary>
        /// <param name="propType"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
		public static string GetStrOnlyValue(UnitPropType propType, double v1)
        {
            switch (propType)
            {
                case UnitPropType.MaxHp:
                case UnitPropType.NowHp:
                case UnitPropType.MaxMp:
                case UnitPropType.NowMp:
                case UnitPropType.HpReply:
                case UnitPropType.MpReply:
                case UnitPropType.CombatSpeed:
                case UnitPropType.PhysicAtk:
                case UnitPropType.MagicAtk:
                    return v1.ToString("f0");
                case UnitPropType.PhysicHarmAdd:
                case UnitPropType.MagicHarmAdd:
                case UnitPropType.FinallyHarmAdd:
                case UnitPropType.PhysicDef:
                case UnitPropType.MagicDef:
                case UnitPropType.FinallyHarmReduce:
                case UnitPropType.Crit:
                case UnitPropType.CritMulti:
                case UnitPropType.Evade:
                    return $"{(v1 * 100).ToString("f0")}%";
            }
            throw new System.NotImplementedException($"还未实现类型为{propType}的格式");
        }
    }
}