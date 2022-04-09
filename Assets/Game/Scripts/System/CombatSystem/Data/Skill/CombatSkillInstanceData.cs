using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld.Combat
{
	/// <summary>
	/// 战斗中技能实例数据
	/// </summary>
	public class CombatSkillInstanceData : IReference
	{
		/// <summary>
		/// 是否已经注册的
		/// </summary>
		private bool m_Register;
		private CombatUnitInfo m_Belong;

		private SkillBaseData m_BaseData;
		public SkillBaseData BaseData { get { return m_BaseData; } }
		public int Id { get { return m_BaseData.Id; } }
		private int m_Lv;
		public int Lv { get { return m_Lv; } }
		/// <summary>
		/// 施法目标类型
		/// </summary>
		public SkillCastTarget CastTarget
		{
			get
			{
				if (m_BaseData.ScopeData == null)
				{
					return default;
				}
				if (m_BaseData.ScopeData.CastTarget.IsNullOrEmpty())
				{
					return default;
				}
				if (m_BaseData.ScopeData.CastTarget.Length == 1)
				{
					return m_BaseData.ScopeData.CastTarget[0];
				}
				return m_BaseData.ScopeData.CastTarget[m_Lv - 1];
			}
		}
		/// <summary>
		/// 施法范围类型
		/// </summary>
		public SkillCastRangeCategory CastRangeCategory
		{
			get
			{
				if (m_BaseData.ScopeData == null)
				{
					return default;
				}
				if (m_BaseData.ScopeData.CastRangeCategory.IsNullOrEmpty())
				{
					return default;
				}
				if (m_BaseData.ScopeData.CastRangeCategory.Length == 1)
				{
					return m_BaseData.ScopeData.CastRangeCategory[0];
				}
				return m_BaseData.ScopeData.CastRangeCategory[m_Lv - 1];
			}
		}
		/// <summary>
		/// 施法距离
		/// </summary>
		public int CastDistance
		{
			get
			{
				if (m_BaseData.ScopeData == null)
				{
					return default;
				}
				if (m_BaseData.ScopeData.CastRange.IsNullOrEmpty())
				{
					return default;
				}
				if (m_BaseData.ScopeData.CastRange.Length == 1)
				{
					return m_BaseData.ScopeData.CastRange[0];
				}
				return m_BaseData.ScopeData.CastRange[m_Lv - 1];
			}
		}
		/// <summary>
		/// 效果作用范围类型
		/// </summary>
		public SkillEffectiveRangeCategory EffectiveRangeCategory
		{
			get
			{
				if (m_BaseData.ScopeData == null)
				{
					return default;
				}
				if (m_BaseData.ScopeData.EffectiveRangeCategory.IsNullOrEmpty())
				{
					return default;
				}
				if (m_BaseData.ScopeData.EffectiveRangeCategory.Length == 1)
				{
					return m_BaseData.ScopeData.EffectiveRangeCategory[0];
				}
				return m_BaseData.ScopeData.EffectiveRangeCategory[m_Lv - 1];
			}
		}
		/// <summary>
		/// 效果作用距离
		/// </summary>
		public int EffectiveDistance
		{
			get
			{
				if (m_BaseData.ScopeData == null)
				{
					return default;
				}
				if (m_BaseData.ScopeData.EffectiveDistance.IsNullOrEmpty())
				{
					return default;
				}
				if (m_BaseData.ScopeData.EffectiveDistance.Length == 1)
				{
					return m_BaseData.ScopeData.EffectiveDistance[0];
				}
				return m_BaseData.ScopeData.EffectiveDistance[m_Lv - 1];
			}
		}
		/// <summary>
		/// 效果方法名
		/// </summary>
		public string EffectFunctionName { get { return m_BaseData.EffectScritpFile; } }
		/// <summary>
		/// 技能触发条件
		/// </summary>
		public EffectConditionArg Condition { get { return m_BaseData.SkillCondition; } }
		public int MaxCD { get { return m_BaseData.GetCDTurn(m_Lv); } }
		private int m_NowCD;
		public int NowCD { get { return m_NowCD; } }
		private string m_Des;
		public string Des { get { return m_Des; } }


		public bool CheckSkillAvailable(out string msg)
        {
			msg = string.Empty;
			if (m_NowCD > 0)
            {
				msg = "技能还在冷却中";
				return false;
            }
			return true;
        }

		/// <summary>
		/// 尝试获取自定义数据
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string TryGetCustomData(string name)
		{
			if (m_BaseData.CustomData == null) return null;
			if (m_BaseData.CustomData.TryGetValue(name, out var objs))
			{
				if (objs.Length == 1) return objs[0];
				return objs[m_Lv];
			}
			return null;
		}

		/// <summary>
		/// 进入所属单位回合
		/// </summary>
		public void EnterTurn()
        {
			if (m_NowCD > 0)
            {
				m_NowCD--;
            }
        }

		/// <summary>
		/// 注册技能
		/// </summary>
		public void RegisterSkill()
        {
			if (m_Register == false && m_BaseData.Category == SkillCategory.Passivity)
			{
				if (m_BaseData.SkillCondition == null || m_BaseData.SkillCondition.TriggerCode.IsNullOrEmpty())
                {
					Log.Error($"请检查技能: id = {m_BaseData.Id}--{m_BaseData.Name}的触发条件 ");
					return;
                }
				GameEntry.Event.Subscribe(CombatEventArgs.EventID, CheckSkillTrigger);
				m_Register = true;
			}
        }

		/// <summary>
		/// 取消注册技能
		/// </summary>
		public void UnRegisterSkill()
        {
			if (m_BaseData.Category == SkillCategory.Passivity)
			{
				GameEntry.Event.Unsubscribe(CombatEventArgs.EventID, CheckSkillTrigger);
				m_Register = false;
			}
        }

		/// <summary>
		/// 执行技能
		/// </summary>
		public void ExecuteSkill(CombatUnitInfo source, CombatUnitInfo[] targets, object sender, EffectTriggerCode triggerCode, Vector3Int actionPoint, System.Action complete)
        {
			//减少魔法值
			m_Belong.Prop.ReduceProperty(UnitPropType.NowMp, UnitPropCategory.Base, UnitPropValueType.Fixed, m_BaseData.GetCostMp(m_Lv));
			//进入cd
			m_NowCD = MaxCD;
            //获得技能效果执行文件
			if (m_BaseData.Category == SkillCategory.Passivity)
			{
				if (Condition == null && Condition.TriggerCode.Has(triggerCode) == false)
				{
					return;
				}
			}
			Log.Debug($"<color=#00ffff>{m_BaseData.Name}</color>技能开始执行");
			//构建参数
			CombatEffectData ed = CombatEffectData.Create(m_Belong, this, null, null, sender, complete);
			ed.SetActionUnit(source, targets);
			ed.SkillCastPoint = actionPoint;
			GameEntry.Lua.ExecuteCombatEffectAction(EffectFunctionName, ed);
		}

		/// <summary>
		/// 检查被动技能触发
		/// </summary>
		private void CheckSkillTrigger(object sender, System.EventArgs e)
        {
			if (m_BaseData.Category != SkillCategory.Passivity) return;
			var ce = e as CombatEventArgs;
			if (m_BaseData.SkillCondition.TriggerCode.Has(ce.EventCode) == false) return;
			if (m_BaseData.SkillCondition.SelfTrigger && ce.TriggerActionUnit != m_Belong) return;
			if (CheckSkillAvailable(out var msg) == false) return;
			//todo,触发效果,此处只有被动技能触发
			ExecuteSkill(ce.TriggerActionUnit, ce.ActionTargets, sender, ce.EventCode, Vector3Int.zero, null);
        }

		private void SetDes()
		{
			string costMp = string.Empty;
			string cd = string.Empty;
			int iCostMp = m_BaseData.GetCostMp(m_Lv);
			if (iCostMp == int.MinValue)
			{
				costMp = "---";
			}
			else
			{
				costMp = iCostMp.ToString();
			}
			int iCD = m_BaseData.GetCDTurn(m_Lv);
			if (iCD == int.MinValue)
			{
				cd = "---";
			}
			else
			{
				cd = iCD.ToString();
			}
			m_Des = $"<size=130%>{m_BaseData.Name}</size>\n<b>{GameEntry.Config.GetString(m_BaseData.Category.ToString())}</b>\nLv{m_Lv}\n<color=#0000ff>MP: {costMp}</color>\nCD: {cd}回合\n  <color=#999999>{m_BaseData.Des}</color>";
		}

		public void Clear()
		{
			
		}

		public static CombatSkillInstanceData Create(CombatUnitInfo belong, SkillBaseData baseData, int lv)
        {
			var result = ReferencePool.Acquire<CombatSkillInstanceData>();
			result.m_Belong = belong;
			result.m_BaseData = baseData;
			result.m_Lv = lv;
			result.m_NowCD = 0;
			result.m_Register = false;
			result.SetDes();
			return result;
        }
	}
}