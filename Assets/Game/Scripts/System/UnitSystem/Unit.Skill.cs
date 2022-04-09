using BoringWorld.UI.RoleSkill;
using GameFramework;
using GameFramework.DataTable;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BoringWorld.UnitDataRow;

namespace BoringWorld
{
	public partial class Unit
	{
        public interface IUnitSkillMoudle : IReference
        {
            ISkillUIDisplayData[] GetSkillInstances();
            ISkillUIDisplayData GetSkillInstance(int skillID);
            UnitSkillShowInfo[] GetSkillShowInfos(SkillCategory category);
            UnitSkillShowInfo GetSkillShowInfo(int skillID);
        }

        /// <summary>
        /// 单位技能模块
        /// </summary>
        private class UnitSkillModule : IUnitSkillMoudle 
        {
            private Dictionary<int, SkillUIData> m_AllSkill;
            private IDataTable<SkillBaseData> m_BaseDataTable;
            private Unit m_Belong;
            private List<int> m_SearchResult;

            public UnitSkillModule()
            {
                m_AllSkill = new Dictionary<int, SkillUIData>();
                m_SearchResult = new List<int>();
            }

            private void InitModule(UnitBaseSkillData[] baseSkillData)
            {
                m_AllSkill.Clear();
                if (baseSkillData.IsNullOrEmpty()) return;
                for (int i = 0; i < baseSkillData.Length; i++)
                {
                    var baseData = m_BaseDataTable.GetDataRow(baseSkillData[i].SkillID);
                    int lv = 0;
                    if (baseSkillData[i].UnLock == false)
                    {
                        lv = 0;
                    }
                    SkillUIData instance = SkillUIData.Create(baseData, m_Belong, baseSkillData[i].UnLock, lv);
                    m_AllSkill.Add(baseSkillData[i].SkillID, instance);
                }
            }

            public ISkillUIDisplayData[] GetSkillInstances()
            {
                if (m_AllSkill.Count == 0) return null;
                ISkillUIDisplayData[] result = m_AllSkill.Values.ToArray();
                return result;
            }

            public ISkillUIDisplayData GetSkillInfo(int skillID)
            {
                if (m_AllSkill.TryGetValue(skillID, out var result))
                {
                    return result;
                }
                throw new System.Exception($"未找到id = {skillID}的技能实例");
            }

            public UnitSkillShowInfo[] GetSkillShowInfos(SkillCategory category)
            {
                m_SearchResult.Clear();
                foreach (var item in m_AllSkill)
                {
                    if (item.Value.BaseData.Category == category)
                    {
                        m_SearchResult.Add(item.Key);
                    }
                }
                UnitSkillShowInfo[] result = new UnitSkillShowInfo[m_SearchResult.Count];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = GetSkillShowInfo(m_SearchResult[i]);
                }
                return result;
            }

            public UnitSkillShowInfo GetSkillShowInfo(int skillID)
            {
                return GetSkillInstance(skillID).GetSkillInfo();
            }

            public ISkillUIDisplayData GetSkillInstance(int skillID)
            {
                if (m_AllSkill.TryGetValue(skillID, out var skill))
                {
                    return skill;
                }
                throw new System.NullReferenceException($"查找不到ID = {skillID}的技能");
            }

            public void Clear()
            {
                foreach (var item in m_AllSkill)
                {
                    item.Value.Release();
                }
            }

            public static UnitSkillModule Create(Unit belong, UnitBaseSkillData[] baseSkillData)
            {
                UnitSkillModule result = ReferencePool.Acquire<UnitSkillModule>();
                result.m_Belong = belong;
                result.m_BaseDataTable = GameEntry.DataTable.GetDataTable<SkillBaseData>(DataTableName.SKILL_BASE_DATA_NAME);
                result.InitModule(baseSkillData);
                return result;
            }
        }
    }
}