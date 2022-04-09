using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoringWorld.UI.RoleInfoForm;
using UnityGameFramework.Runtime;

namespace BoringWorld.UI.RoleEquip
{
	public class RoleEquipFormLogic : UIFormLogic
	{
		private ChangeRoleArea m_ChangeRole;
		private RoleDetailPropertyArea m_RoleDetailProp;
		private RoleAllDressEquipArea m_RoleAllDressEquip;
		private ReplaceEquipArea m_ReplaceEquip;
		private EquipDetailInfoArea m_EquipDetailInfo;

		private BackpackSystem m_BackSystem;
		private Unit m_Unit;
		private UnitEquipPartType m_PartType;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
			m_ChangeRole = this.GetComponentWithName<ChangeRoleArea>("ChangeRoleArea");
			m_RoleDetailProp = this.GetComponentWithName<RoleDetailPropertyArea>("RoleDetailProperty");
			m_RoleAllDressEquip = this.GetComponentWithName<RoleAllDressEquipArea>("RoleAllDressEquipArea");
			m_ReplaceEquip = this.GetComponentWithName<ReplaceEquipArea>("ReplaceEquipArea");
			m_EquipDetailInfo = this.GetComponentWithName<EquipDetailInfoArea>("EquipDetailInfoArea");

			m_ChangeRole.OnInit();
			m_RoleAllDressEquip.OnInit(this);
			m_RoleDetailProp.OnInit();
			m_ReplaceEquip.OnInit();
			m_EquipDetailInfo.OnInit();

			m_BackSystem = GameEntry.Backpack;
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
			m_ChangeRole.Open(OnRefresh);
			m_EquipDetailInfo.OnOpen();
        }

		private void OnRefresh(Unit oldUnit, Unit nowUnit)
        {
			m_RoleDetailProp.Refresh(oldUnit, nowUnit);
			m_RoleAllDressEquip.OnRefresh(nowUnit);
			m_Unit = nowUnit;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
			m_ReplaceEquip.OnUpdate();
        }

        /// <summary>
        /// 显示替换装备界面
        /// </summary>
        public void ShowReplaceEquipArea(UnitEquipPartType part)
        {
			m_PartType = part;
			m_ReplaceEquip.OnOpen(m_BackSystem.FindReplaceEquipItem(m_Unit.GetUnitDressEquipType(part)), this);
        }

		/// <summary>
		/// 显示准备装备信息
		/// </summary>
		/// <param name="data"></param>
		public void ShowEquipInfo(EquipItemDataRow data)
        {
			m_EquipDetailInfo.OnRefresh(data);
        }

		/// <summary>
		/// 更换装备
		/// </summary>
		public void ReplaceTheEquipment(BackpackItemData data)
        {
			if (m_Unit.ReplaceTheEquipment(m_PartType, data.ItemData as EquipItemDataRow, out var replaceEquip))
            {
				if (replaceEquip != null)
				{
					m_BackSystem.AddItemToBag(ItemType.Equip, replaceEquip, 1);
				}
				//刷新显示
				m_BackSystem.RecyleItem(ItemType.Equip, data);
				m_RoleAllDressEquip.OnRefresh(m_Unit);
				m_ReplaceEquip.OnClose();
            }
            else
            {
				Log.Error("替换装备失败");
				return;
            }
        }

    }
}