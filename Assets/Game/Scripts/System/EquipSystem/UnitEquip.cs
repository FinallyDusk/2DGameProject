using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 单位装备数据
	/// </summary>
	public class UnitEquip : IReference
	{
		/// <summary>
		/// 单位每个部位可以穿戴的装备类型
		/// </summary>
		private Dictionary<UnitEquipPartType, EquipType> m_UnitWearableEquipType;
		/// <summary>
		/// 单位穿戴的装备
		/// </summary>
		private Dictionary<UnitEquipPartType, EquipItemDataRow> m_UnitWearingEquip;

		private Unit m_Master;
		private List<AdditionProperty> m_AllProps;

		public UnitEquip()
        {
			m_UnitWearableEquipType = new Dictionary<UnitEquipPartType, EquipType>()
            {
                {UnitEquipPartType.Armor, EquipType.Armor},
                {UnitEquipPartType.Designation, EquipType.Designation},
                {UnitEquipPartType.Exclusive, EquipType.Exclusive},
                {UnitEquipPartType.Helmet, EquipType.Helmet},
                {UnitEquipPartType.HolyThings, EquipType.HolyThings},
                {UnitEquipPartType.LegArmor, EquipType.LegArmor},
                {UnitEquipPartType.Necklace, EquipType.Necklace},
                {UnitEquipPartType.Ring1, EquipType.Ring},
                {UnitEquipPartType.Ring2, EquipType.Ring},
                {UnitEquipPartType.Shoes, EquipType.Shoes},
                {UnitEquipPartType.Soul, EquipType.Soul}
            };

            m_UnitWearingEquip = new Dictionary<UnitEquipPartType, EquipItemDataRow>();
			m_AllProps = new List<AdditionProperty>();
		}

		public void Init(Unit master, EquipType weaponType)
        {
			m_Master = master;
			m_UnitWearableEquipType[UnitEquipPartType.Weapon] = weaponType;
        }

		/// <summary>
		/// 获得对应部位的装备
		/// </summary>
		/// <returns></returns>
		public EquipItemDataRow GetPartEquip(UnitEquipPartType part)
        {
			if (m_UnitWearingEquip.TryGetValue(part, out var result))
            {
				return result;
            }
			return null;
        }

		/// <summary>
		/// 获得可以穿戴的装备类型
		/// </summary>
		/// <returns></returns>
		public EquipType GetDressEquipType(UnitEquipPartType part)
        {
			return m_UnitWearableEquipType[part];
        }

		/// <summary>
		/// 穿戴装备
		/// </summary>
		/// <returns></returns>
		public bool DressEquip(UnitEquipPartType part, EquipItemDataRow newEquipData, out EquipItemDataRow oldEquipData)
        {
			//先检查是否符合穿戴条件
			oldEquipData = null;
			if (m_UnitWearableEquipType[part] != newEquipData.EquipType)
            {
				return false;
            }
			m_UnitWearingEquip.TryGetValue(part, out oldEquipData);
			m_UnitWearingEquip[part] = newEquipData;
			return true;
        }

		/// <summary>
		/// 重新计算属性
		/// </summary>
		public List<AdditionProperty> RecalcProperty()
        {
			m_AllProps.Clear();
			foreach (var item in m_UnitWearingEquip)
            {
				if (item.Value == null) continue;
				item.Value.GetEquipProperty(m_AllProps);
            }
			return m_AllProps;
        }

		public void Clear()
        {
			m_UnitWearableEquipType.Clear();
			m_UnitWearingEquip.Clear();
        }
	}

	/// <summary>
	/// 额外属性类，用于装备、技能等额外加成属性
	/// </summary>
	public class AdditionProperty : IReference
    {
		public UnitPropType PropType;
		public UnitPropCategory Category;
		public UnitPropValueType ValueType;
		public double Value;

		public void AddValue(double delta)
        {
			Value += delta;
        }

        public void Clear()
        {
            
        }
    }
}