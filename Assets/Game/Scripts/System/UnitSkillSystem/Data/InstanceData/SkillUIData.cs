using BoringWorld.UI.RoleSkill;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 技能UI显示数据
	/// </summary>
	public interface ISkillUIDisplayData : IReference
	{
		/// <summary>
		/// 技能基础数据
		/// </summary>
		SkillBaseData BaseData { get; }
		/// <summary>
		/// 技能等级
		/// </summary>
		int Lv { get; }
		/// <summary>
		/// 尝试提升技能等级
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		bool TryUpgradeSkillLevel(out string message);
		/// <summary>
		/// 尝试降低技能等级
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		bool TryLowerSkillLevel(out string message);
		/// <summary>
		/// 获得技能信息
		/// </summary>
		/// <returns></returns>
		UnitSkillShowInfo GetSkillInfo();
	}

    /// <summary>
    /// 技能UI显示用数据
    /// </summary>
    public class SkillUIData : ISkillUIDisplayData 
    {
        protected int m_Id;
        protected SkillBaseData m_BaseData;
        protected int m_Lv;
        protected bool m_UnLock;
		protected Unit m_BelongUnit;

        public int Id { get { return m_Id; } }
        public SkillBaseData BaseData { get { return m_BaseData; } }
        public int Lv { get { return m_Lv; } }
        public bool UnLock { get { return m_UnLock; } }

		/// <summary>
		/// 尝试提升技能等级，注：此处不负责减少点数
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool TryUpgradeSkillLevel(out string message)
		{
			message = string.Empty;
			if (m_UnLock == false)
            {
				message = "技能还未解锁";
				return false;
            }
			if (m_BaseData == null)
			{
				return false;
			}
			if (m_Lv == m_BaseData.MaxLv)
			{
				message = "技能已经达到最大等级";
				return false;
			}
			if (m_BelongUnit.SkillPoint < m_BaseData.UpgradePoint)
			{
				message = "人物技能点数不足";
				return false;
			}
			int needLv = m_BaseData.UnLockLv + m_BaseData.UpgradeLvDelta * (m_Lv - 1);
			if (m_BelongUnit.Lv < needLv)
			{
				message = $"人物等级不够，需求等级：{needLv}";
				return false;
			}
			//达成所有条件
			m_Lv += 1;
			return true;
		}

		/// <summary>
		/// 尝试减少技能等级，注：此处不负责增加点数
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool TryLowerSkillLevel(out string message)
		{
			message = string.Empty;
			if (m_UnLock == false)
			{
				message = "技能还未解锁";
				return false;
			}
			if (m_BaseData == null)
			{
				return false;
			}
			if (m_Lv <= 0)
			{
				message = "技能等级为0，不能继续降低";
				return false;
			}
			//达成所有条件
			m_Lv -= 1;
			return true;
		}

		public UnitSkillShowInfo GetSkillInfo()
		{
			var iCostMp = m_BaseData.GetCostMp(m_Lv);
			string costMp = string.Empty;
			if (iCostMp == int.MinValue)
            {
				costMp = "---";
            }
            else
            {
				costMp = iCostMp.ToString();
            }

			var iCDTurn = m_BaseData.GetCDTurn(m_Lv);
			string cdTurn = string.Empty;
			if (iCDTurn == int.MinValue)
			{
				cdTurn = "---";
			}
			else
			{
				cdTurn = iCDTurn.ToString();
			}

			UnitSkillShowInfo result = new UnitSkillShowInfo(m_Id, m_BaseData.Name, !m_UnLock, m_Lv.ToString(), costMp, cdTurn, m_BaseData.Des, m_BaseData.UpgradePoint.ToString());
			return result;
		}


		public void Clear()
        {
            
        }

		public static SkillUIData Create(SkillBaseData baseData, Unit belongUnit, bool unLock, int lv)
		{
			SkillUIData result = ReferencePool.Acquire<SkillUIData>();
			result.m_Id = baseData.Id;
			result.m_BaseData = baseData;
			result.m_BelongUnit = belongUnit;
			result.m_UnLock = unLock;
			result.m_Lv = lv;
			return result;
		}
    }
}