using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld.UI.RoleSkill
{
	public class RoleSkillFormLogic : UIFormLogic
	{
        private ChangeRoleArea m_ChangeRole;
        private AllSkillArea m_UnitAllSkill;
        private SkillDetailInfoArea m_SkillDetailInfo;

        private Unit m_Unit;
        private int m_ShowSkillID;
        private SkillCategory m_SkillCategory;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_ChangeRole = this.GetComponentWithName<ChangeRoleArea>("ChangeRoleArea");
            m_UnitAllSkill = this.GetComponentWithName<AllSkillArea>("AllSkillArea");
            m_SkillDetailInfo = this.GetComponentWithName<SkillDetailInfoArea>("SkillDetailInfoArea");

            m_ChangeRole.OnInit();
            m_UnitAllSkill.OnInit(this);
            m_SkillDetailInfo.OnInit(this);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_ChangeRole.Open(ChangeShowUnitCallback);
        }

        private void ChangeShowUnitCallback(Unit oldUnit, Unit nowUnit)
        {
            m_Unit = nowUnit;
            ChangeShowSkillCategory(SkillCategory.Initiative);
        }

        public void ShowSkillInfo(UnitSkillShowInfo skillInfo)
        {
            m_ShowSkillID = skillInfo.SkillID;
            m_SkillDetailInfo.OnRefresh(skillInfo, m_Unit.SkillPoint);
        }

        /// <summary>
        /// 改变显示技能类型
        /// </summary>
        public void ChangeShowSkillCategory(SkillCategory category)
        {
            m_SkillCategory = category;
            m_UnitAllSkill.OnRefresh(category, m_Unit);
            m_SkillDetailInfo.OnClose();
        }

        public void UpgradeSkillLevel()
        {
            if (m_Unit.TryUpgradeSkillLevel(m_ShowSkillID, out string msg))
            {
                m_SkillDetailInfo.OnRefresh(m_Unit.GetSkillShowInfo(m_ShowSkillID), m_Unit.SkillPoint);
            }
            m_SkillDetailInfo.RefreshActionMsg(msg);
        }

        public void LowerSkillLevel()
        {
            if (m_Unit.TryLowerSkillLevel(m_ShowSkillID, out string msg))
            {
                m_SkillDetailInfo.OnRefresh(m_Unit.GetSkillShowInfo(m_ShowSkillID), m_Unit.SkillPoint);
            }
            m_SkillDetailInfo.RefreshActionMsg(msg);
        }
    }
}