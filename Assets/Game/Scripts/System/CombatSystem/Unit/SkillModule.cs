using BoringWorld.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    public partial class CombatUnitInfo
    {
        /// <summary>
        /// 战斗技能模块
        /// </summary>
        public class SkillModule
        {
            //private List<CombatSkillInstanceData> m_InitiativeSkill;
            //private List<CombatSkillInstanceData> m_CounterSkill;
            //private List<CombatSkillInstanceData> m_PassivitySkill;
            private Dictionary<int, CombatSkillInstanceData> m_AllSkills;
            private List<CombatSkillInstanceData> m_AllSkillList;
            private CombatUnitInfo m_Belong;
            /// <summary>
            /// 准备使用的技能
            /// </summary>
            public CombatSkillInstanceData PrepareUseSkill;

            public SkillModule()
            {
                m_AllSkills = new Dictionary<int, CombatSkillInstanceData>();
                m_AllSkillList = new List<CombatSkillInstanceData>();
            }

            public void OnInit(CombatUnitInfo belong)
            {
                PrepareUseSkill = null;
                m_AllSkills.Clear();
                m_Belong = belong;
                var allSkill = m_Belong.m_Instance.GetSkillInstances();
                if (allSkill.IsNullOrEmpty() == false)
                {
                    foreach (var item in allSkill)
                    {
                        var data = CombatSkillInstanceData.Create(m_Belong, item.BaseData, item.Lv);
                        m_AllSkills.Add(item.BaseData.Id, data);
                        m_AllSkillList.Add(data);
                    }
                    RegisterSkill();
                }
            }

            /// <summary>
            /// 注册技能触发条件
            /// </summary>
            private void RegisterSkill()
            {
                foreach (var item in m_AllSkills)
                {
                    item.Value.RegisterSkill();
                }
            }

            /// <summary>
            /// 取消注册技能触发条件
            /// </summary>
            private void UnRegisterSkill()
            {
                foreach (var item in m_AllSkills)
                {
                    item.Value.UnRegisterSkill();
                }
            }

            public CombatSkillInstanceData GetSkillData(int skillID)
            {
                if (m_AllSkills.TryGetValue(skillID, out var result))
                {
                    return result;
                }
                throw new System.NullReferenceException($"找不到技能ID = {skillID}的数据");
            }

            /// <summary>
            /// 进入自己回合
            /// </summary>
            public void EnterTurn()
            {
                foreach (var item in m_AllSkills)
                {
                    item.Value.EnterTurn();
                }
            }

            public List<CombatSkillInstanceData> GetAllSkill()
            {
                return m_AllSkillList;
            }

            public void Clear()
            {
                UnRegisterSkill();
                foreach (var item in m_AllSkills)
                {
                    item.Value.Release();
                }
                m_AllSkills.Clear();
                m_AllSkillList.Clear();
            }
        }
    }
}