using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	/// <summary>
	/// 单位数据类
	/// </summary>
	public class UnitDataRow : DataRowBase
	{
        public override int Id { get { return id; } }
		[JsonProperty][ShowInInspector][LabelText("编号")]
		private int id;
		[JsonProperty][ShowInInspector]
		[LabelText("单位名称")]
		public string UnitName { get; private set; }
		[JsonProperty]
		[ShowInInspector]
		[LabelText("单位故事背景"), MultiLineProperty(4)]
		public string UnitStory { get; private set; }
		/// <summary>
		/// 立绘图片路径
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("单位立绘名")]
		public string PaintAssetName { get; private set; }
		/// <summary>
		/// 战斗行动条图标
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("战斗中行动条的图标")]
		public string CombatActionIcon { get; private set; }
		/// <summary>
		/// 地图显示资源名称
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("在地图上时的图标名")]
		public string MapDisplayIconName { get; private set; }
		/// <summary>
		/// 实体资源路径
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("预制体路径")]
		public string EntityAssetPath { get; private set; }
		/// <summary>
		/// 基础属性映射ID
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("基础属性的ID")]
		public int PropertyDataID { get; private set; }
		/// <summary>
		/// 单位阵营
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("单位阵营")]
		public UnitCamp Camp { get; private set; }
		/// <summary>
		/// 基础等级
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("基础等级")]
		public int BaseLv { get; private set; }
		/// <summary>
		/// 基础技能点数
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("基础技能点数")]
		public int BaseSkillPoint { get; private set; }
		/// <summary>
		/// 人物升级获得的技能点数
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("升级时获得的技能点数")]
		public int LvUpgradeSkillPoint { get; private set; }
		/// <summary>
		/// 移动距离
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("移动距离")]
		public int MoveDistance { get; private set; }
		/// <summary>
		/// 攻击距离
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("攻击距离")]
		public int AtkDistance { get; private set; }
		/// <summary>
		/// 武器类型
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("普攻伤害类型")]
		public EquipType WeaponType { get; private set; }
		/// <summary>
		/// 普攻伤害类型
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("普攻伤害类型")]
		public HarmType AtkHarmType { get; private set; }
		/// <summary>
		/// 普攻特效，-1表示为没有特效
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("普攻伤害特效")]
		public SpecialEffectConfig AtkSpecialEffect { get; private set; }
		/// <summary>
		/// 单位基础装备
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("基础装备")]
		public Dictionary<UnitEquipPartType, int> BaseEquip { get; private set; }
		/// <summary>
		/// 单位基础技能
		/// </summary>
		[JsonProperty][ShowInInspector]
		[LabelText("基础技能")]
		public UnitBaseSkillData[] BaseSkill { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
			UnitDataRow row = JsonTool.LoadJson<UnitDataRow>(dataRowString);
			id = row.id;
			UnitName = row.UnitName;
			UnitStory = row.UnitStory;
			PaintAssetName = row.PaintAssetName;
			CombatActionIcon = row.CombatActionIcon;
			MapDisplayIconName = row.MapDisplayIconName;
			PropertyDataID = row.PropertyDataID;
			Camp = row.Camp;
			EntityAssetPath = row.EntityAssetPath;
			BaseLv = row.BaseLv;
			BaseSkillPoint = row.BaseSkillPoint;
			LvUpgradeSkillPoint = row.LvUpgradeSkillPoint;
			MoveDistance = row.MoveDistance;
			AtkDistance = row.AtkDistance;
			WeaponType = row.WeaponType;
			AtkHarmType = row.AtkHarmType;
			AtkSpecialEffect = row.AtkSpecialEffect;
			BaseEquip = row.BaseEquip;
			BaseSkill = row.BaseSkill;
			return true;
        }

		/// <summary>
		/// 单位基础技能数据
		/// </summary>
		[SerializeField]
		public class UnitBaseSkillData
        {
			/// <summary>
			/// 单位所有种类技能
			/// </summary>
			[LabelText("技能ID")]
			public int SkillID;
			/// <summary>
			/// 单位解锁的技能
			/// </summary>
			[LabelText("是否解锁")]
			public bool UnLock;
			/// <summary>
			/// 单位基础技能等级，一般用于敌人
			/// </summary>
			[LabelText("技能初始等级")]
			public int SkillBaseLv;
        }
    }
}