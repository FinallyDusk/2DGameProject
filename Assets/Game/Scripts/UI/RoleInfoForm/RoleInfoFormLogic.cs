using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace BoringWorld.UI.RoleInfoForm
{
	/// <summary>
	/// 角色详细信息界面逻辑
	/// </summary>
	public class RoleInfoFormLogic : UIFormLogic
	{
#pragma warning disable 0649
		/// <summary>
		/// 单位立绘图标
		/// </summary>
		private Image m_UnitPaintIcon;
		private ChangeRoleArea m_ChangeRoleArea;
        private RoleDetailPropertyArea m_DetailPropertyItem;
#pragma warning restore 0649

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
			m_UnitPaintIcon = this.GetComponentWithName<Image>("RolePaint");
			m_ChangeRoleArea = this.GetComponentWithName<ChangeRoleArea>("ChangeRoleArea");
			m_DetailPropertyItem = this.GetComponentWithName<RoleDetailPropertyArea>("RoleDetailProperty");
			m_ChangeRoleArea.OnInit();
			m_DetailPropertyItem.OnInit();
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
			m_ChangeRoleArea.Open(OnChangeShowUnitCallback);
        }

		private void OnChangeShowUnitCallback(Unit oldUnit, Unit nowUnit)
        {
			//更改立绘
			m_UnitPaintIcon.sprite = GameEntry.Sprite.GetSprite(nowUnit.BaseData.PaintAssetName, SpriteType.UnitPaint);
			m_DetailPropertyItem.Refresh(oldUnit, nowUnit);
        }

    }
}