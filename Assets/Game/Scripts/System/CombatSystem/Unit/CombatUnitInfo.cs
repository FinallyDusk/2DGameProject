using BoringWorld.Combat;
using BoringWorld.UI.RoleSkill;
using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{

    /// <summary>
    /// 战斗中的战斗单位信息
    /// </summary>
    public partial class CombatUnitInfo : IReference
    {
        /// <summary>
        /// 实例ID（<see cref="Unit"/>的ID）
        /// </summary>
        public int InstanceID { get; private set; }
        /// <summary>
        /// 实例
        /// </summary>
        private Unit m_Instance { get; set; }
        private UnitCamp m_Camp;
        private double m_ActionProgress;
        public System.Action<double> ActionProgressEvent;
        /// <summary>
        /// 单位的AI
        /// </summary>
        private AIBaseAction m_AIController;
        private CombatSystem m_CombatSystem;
        /// <summary>
        /// 行动进度（到达100时即可进行操作）
        /// </summary>
        public double ActionProgress
        {
            get
            {
                return m_ActionProgress;
            }
            set
            {
                m_ActionProgress = value;
                ActionProgressEvent?.Invoke(m_ActionProgress);
            }
        }
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3Int Position
        {
            get
            {
                return m_Instance.GetUnitPosition().ToInt();
            }
            set
            {
                m_Instance.TransformPosition(value);
            }
        }
        /// <summary>
        /// 获得攻击伤害
        /// </summary>
        public double AttackHarm
        {
            get
            {
                switch (m_BaseData.AtkHarmType)
                {
                    case HarmType.Physical:
                        return m_UnitProp.GetProperty(UnitPropType.PhysicAtk);
                    case HarmType.Magic:
                        return m_UnitProp.GetProperty(UnitPropType.MagicAtk);
                    case HarmType.Real:
                        return (m_UnitProp.GetProperty(UnitPropType.PhysicAtk) + m_UnitProp.GetProperty(UnitPropType.MagicAtk)) * 0.5;
                }
                return 0;
            }
        }
        /// <summary>
        /// 所有效果标签
        /// </summary>
        private int[] m_AllEffectTags;
        /// <summary>
        /// 是否为自己的活动回合
        /// </summary>
        private bool m_ActiveTurn;
        public bool ActiveTurn { get { return m_ActiveTurn; } }
        /// <summary>
        /// buff模块
        /// </summary>
        private BuffModule m_Buff;
        public BuffModule Buff { get { return m_Buff; } }
        /// <summary>
        /// 基础数据模块
        /// </summary>
        private BaseDataModule m_BaseData;
        public BaseDataModule BaseData { get { return m_BaseData; } }
        /// <summary>
        /// 单位属性
        /// </summary>
        private UnitProp m_UnitProp;
        public UnitProp Prop { get { return m_UnitProp; } }
        private SkillModule m_Skill;
        public SkillModule Skill { get { return m_Skill; } }


        public CombatUnitInfo()
        {
            var allEffects = System.Enum.GetValues(typeof(UnitEffectTags));
            m_AllEffectTags = new int[allEffects.Length];
            for (int i = 0; i < m_AllEffectTags.Length; i++)
            {
                m_AllEffectTags[i] = 0;
            }
            m_Buff = new BuffModule(this);
            m_BaseData = new BaseDataModule();
            m_Skill = new SkillModule();
        }

        /// <summary>
        /// 发生攻击
        /// </summary>
        /// <returns></returns>
        public void AppendAttack(CombatActionArgs arg, Action complete)
        {
            var se = m_BaseData.AtkSpecialEffect;
            float interval = 0.5f;
            if (se != null)
            {
                if (se.Duration == 0)
                {
                    interval = GameEntry.SpecialEffect.PlaySpecialEffect(se.EffectName, (double)se.Speed, Position, arg.ActionTargets[0].Position);
                }
                else
                {
                    interval = se.Duration;
                    GameEntry.SpecialEffect.PlaySpecialEffect(se.EffectName, arg.ActionTargets[0].Position, se.Duration);
                }
            }
            //使用计时器
            var item = CombatSystem.CreateTimer(1, interval, AttackAnimComplete);
            item.InitExecuteData(arg);
            item.SetEndCallback(complete);
            item.Start();
        }

        /// <summary>
        /// 攻击动画完成后
        /// </summary>
        /// <param name="data"></param>
        private void AttackAnimComplete(GameTimerExecuteData data)
        {
            CombatActionArgs arg = data.UserData as CombatActionArgs;
            if (arg == null)
            {
                Log.Error($"不能转为操作参数-->data.UserData = {data.UserData}");
                return;
            }
            //增加一个伤害操作
            for (int i = 0; i < arg.ActionTargets.Count; i++)
            {
                var harmData = HarmData.Create(arg.ActionSource, arg.ActionTargets[i], AttackHarm, m_BaseData.AtkHarmType);
                harmData.Execute();
                harmData.Release();
            }
        }

        /// <summary>
        /// 获得伤害
        /// </summary>
        /// <param name="data">伤害数据</param>
        /// <param name="baseValue">基础伤害</param>
        /// <param name="type">伤害类型，注意，如果为真实伤害，则enableCrit和enableAddition是否启用都没关系</param>
        /// <param name="enableCrit">是否发生暴击</param>
        /// <param name="enableAddition">是否经过伤害加成</param>
        public void ProcessHarm(HarmData data)
        {
            double value = data.BaseValue;
            var type = data.HarmType;
            bool crit = false;
            if (data.AllowExtraAdd)
            {
                switch (type)
                {
                    case HarmType.Physical:
                        value = value * (1 + m_UnitProp.GetProperty(UnitPropType.PhysicHarmAdd));
                        break;
                    case HarmType.Magic:
                        value = value * (1 + m_UnitProp.GetProperty(UnitPropType.MagicHarmAdd));
                        break;
                }
                value = value * (1 + m_UnitProp.GetProperty(UnitPropType.FinallyHarmAdd));
            }
            if (data.AllowCrit)
            {
                //计算是否暴击
                double prob = UnityEngine.Random.Range(0, 1f);
                if (prob <= m_UnitProp.GetProperty(UnitPropType.Crit))
                {
                    value = value * m_UnitProp.GetProperty(UnitPropType.CritMulti);
                    crit = true;
                }
            }
            //伤害不允许为负数
            if (value < 0)
            {
                value = 0;
            }
            data.SetOriginalData(value, crit, data.AllowExtraAdd);
        }

        /// <summary>
        /// 承受伤害
        /// </summary>
        /// <param name="data"></param>
        public void TakeHarm(HarmData data)
        {
            double value = data.AfterAdditonValue;
            bool evade = false;
            //判断是否已经闪避了
            if (data.Evade)
            {
                evade = true;
                value = 0;
            }
            //判断是否闪避
            else if (data.AllowEvade)
            {
                double prob = UnityEngine.Random.Range(0, 1f);
                if (prob <= m_UnitProp.GetProperty(UnitPropType.Evade))
                {
                    evade = true;
                    value = 0;
                }
            }
            if (evade == false && data.AllowExtraReduce)
            {
                double defRate = 1 - data.IgnoreDefRate;
                if (defRate > 1)
                {
                    defRate = 1;
                }
                switch (data.HarmType)
                {
                    case HarmType.Physical:
                        var physicDef = m_UnitProp.GetProperty(UnitPropType.PhysicDef);
                        value = value * (1 - physicDef * (1 - defRate));
                        break;
                    case HarmType.Magic:
                        value = value * (1 - m_UnitProp.GetProperty(UnitPropType.MagicDef) * (1 - defRate));
                        break;
                }
                value = value * (1 - m_UnitProp.GetProperty(UnitPropType.FinallyHarmReduce));
            }
            if (value < 0)
            {
                value = 0;
            }
            //此处暂时写法，有可能有的伤害不会发生闪避或者减免之类的
            //todo
            data.SetFinallyData(value, evade, true);
            ReduceHp(data);
        }

        /// <summary>
        /// 减少血量，此处不进行各种计算，只是掉hp
        /// </summary>
        /// 这些应该写入配置表
        /// todo
        /// 闪避的字体颜色为 #00ffff
        /// 暴击的字体大小为原字体的 150%
        /// 真实伤害为 #FB00FF
        /// 物理伤害为 #ff0000
        /// 魔法伤害为 #0000ff
        /// 回复血量为 #00ff00
        public void ReduceHp(HarmData data)
        {
            //减少血量
            m_UnitProp.ReduceProperty(UnitPropType.NowHp, UnitPropCategory.Base, UnitPropValueType.Fixed, data.FinallyValue);
            //显示漂浮数字
            var pos = m_Instance.GetUnitHeadObject().position;
            if (data.Evade)
            {
                CombatSystem.DisplayFloatNum("<color=#00ffff>Miss</color>", pos);
            }
            else
            {
                int size = 100;
                string color = string.Empty;
                if (data.Crit)
                {
                    size = 150;
                }
                switch (data.HarmType)
                {
                    case HarmType.Physical:
                        color = "#ff0000";
                        break;
                    case HarmType.Magic:
                        color = "#0000ff";
                        break;
                    case HarmType.Real:
                        color = "#FB00FF";
                        break;
                }
                CombatSystem.DisplayFloatNum($"<size={size}%>{(data.FinallyValue == 0 ? string.Empty : "-")}<color={color}>{data.FinallyValue.ToString("f0")}</color></size>", pos);
            }
        }

        /// <summary>
        /// 进入战斗
        /// </summary>
        public void EnterCombat()
        {
            //注册技能效果触发条件
        }

        /// <summary>
        /// 进入行动回合
        /// </summary>
        public void EnterActionTurn()
        {
            //此处应有的操作：
            //回合开始型buff的效果生效
            //回复血量和魔法值
            ReplyHpAndMpByTurnStart();
            //技能cd减少
            m_Skill.EnterTurn();
            //实际进入操作（由AI进行掌控）
            m_AIController.EnterTurn(this);
            //此处目前存在的问题
            //技能类型-反制
            //需要打断当前技能施法，强行插入一段技能
            //目前的想到的解决办法，通过协程来执行，在一个技能准备释放前（如果存在可以被反击的条件）就调用CombatSystem的A方法（暂定），传入反击条件（目前决定为字符串）然后返回一个int值（目前处于反制连锁中的第几个位置）
            //在A方法中，发送反击条件的消息，然后返回一个反击位置的int值
            //技能在调用CombatSystem的A方法后，接收当前技能的反制连锁位置，然后每帧去调用CombatSystem的B方法（需要A的返回值）
            //CombatSystem的B方法先判断是否处于等待单位选择反制技能的阶段，然后将传入的int参数和当前的连锁位置进行对比
            //反制技能施法完成后需要调用CombatSystem的C方法（将反制连锁的int-1，不小于0）
        }

        /// <summary>
        /// 回合开始恢复血量和蓝量
        /// </summary>
        private void ReplyHpAndMpByTurnStart()
        {
            RestoreHp(m_UnitProp.GetProperty(UnitPropType.HpReply));
            RestoreMp(m_UnitProp.GetProperty(UnitPropType.MpReply));
        }

        /// <summary>
        /// 执行技能，此处已经满足技能的释放条件
        /// </summary>
        /// <param name="skillArg">技能参数</param>
        public void ExecuteSkill(CombatActionArgs arg)
        {
            arg.SkillInstance.ExecuteSkill(arg.ActionSource, arg.ActionTargets.ToArray(), null, arg.TriggerCode, arg.ActionPoint, arg.Complete);
        }

        /// <summary>
        /// 回复血量
        /// </summary>
        public void RestoreHp(double value)
        {
            CombatSystem.DisplayFloatNum($"<color={GameMain.Config.RestoreHpColor}>+ {value.ToString("f0")}</color>", GetUnitHeadObject().position);
            m_UnitProp.AddProperty(UnitPropType.NowHp, UnitPropCategory.Base, UnitPropValueType.Fixed, value);
        }

        /// <summary>
        /// 回复蓝量
        /// </summary>
        /// <param name="value"></param>
        public void RestoreMp(double value)
        {
            m_UnitProp.AddProperty(UnitPropType.NowMp, UnitPropCategory.Base, UnitPropValueType.Fixed, value);
        }
        /// <summary>
        /// 注册buff显示
        /// </summary>
        public void RegisterBuffDisplay(System.Func<int, Action<BuffInstanceData>> addBuff, Action<int> removeBuff)
        {
            m_Buff.RegisterBuffDisplay(addBuff, removeBuff);
        }

        /// <summary>
        /// 增加buff
        /// </summary>
        public void AddBuff(int buffId)
        {
            var buffInstance = m_CombatSystem.BuffManger.GenerateBuffInstance(buffId);
            m_Buff.AddBuff(buffInstance);
        }

        /// <summary>
        /// 移除buff
        /// </summary>
        /// <param name="buff"></param>
        public void RemoveBuff(BuffInstanceData buff)
        {
            m_Buff.RemoveBuff(buff);
        }

        /// <summary>
        /// 增加标签
        /// </summary>
        /// <param name="tag"></param>
        public void AddTag(UnitEffectTags tag)
        {
            m_AllEffectTags[(int)tag] += 1;
        }

        /// <summary>
        /// 增加标签
        /// </summary>
        /// <param name="tags"></param>
        public void AddTag(IEnumerable<UnitEffectTags> tags)
        {
            foreach (var item in tags)
            {
                m_AllEffectTags[(int)item] += 1;
            }
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="tag"></param>
        public void RemoveTag(UnitEffectTags tag)
        {
            int index = (int)tag;
            m_AllEffectTags[index] -= 1;
            if (m_AllEffectTags[index] < 0)
            {
                m_AllEffectTags[index] = 0;
            }
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        public void RemoveTag(IEnumerable<UnitEffectTags> tags)
        {
            foreach (var item in tags)
            {
                RemoveTag(item);
            }
        }

        /// <summary>
        /// 是否存在标签
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTag(UnitEffectTags tag)
        {
            return m_AllEffectTags[(int)tag] > 0;
        }

        /// <summary>
        /// 显示单位
        /// </summary>
        public void Show()
        {
            m_Instance.Show();
        }

        /// <summary>
        /// 移动的方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="moveLength"></param>
        /// <param name="finshCallback"></param>
        public void Move(Stack<Vector3Int> path, int moveLength = -1, System.Action finshCallback = null)
        {
            //此处暂时使用基础数据的移动
            var timer = CombatSystem.CreateTimer(moveLength, GameMain.Config.MoveIntervalTime, InternalMove);
            timer.InitExecuteData(path);
            timer.SetEndCallback(finshCallback);
            timer.Start();
        }

        private void InternalMove(GameTimerExecuteData data)
        {
            var path = data.UserData as Stack<Vector3Int>;
            m_Instance.TransformPosition(path.Pop());
        }

        #region 技能相关

        /// <summary>
        /// 获得一个种类的全部技能信息
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public UnitSkillShowInfo[] GetUnitAllCategorySkillInfo(SkillCategory category)
        {
            //此处暂时使用基础数据
            return m_Instance.GetUnitAllCategorySkillInfo(category);
        }

        /// <summary>
        /// 获得技能用于显示用信息
        /// </summary>
        /// <param name="category"></param>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public UnitSkillShowInfo GetSkillInfo(int skillID)
        {
            //此处暂时使用基础数据
            return m_Instance.GetSkillShowInfo(skillID);
        }

        /// <summary>
        /// 获得技能实例
        /// </summary>
        /// <param name="category"></param>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public CombatSkillInstanceData GetSkillInstance(int skillID)
        {
            //此处暂时使用基础数据
            return m_Skill.GetSkillData(skillID);
        }

        /// <summary>
        /// 获得技能等级
        /// </summary>
        /// <returns></returns>
        public int GetSkillLv(int skillID)
        {
            return GetSkillInstance(skillID).Lv;
        }

        /// <summary>
        /// 获得技能效果数据(文件)
        /// </summary>
        /// <param name="category"></param>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public string GetSkillEffectScriptFile(int skillID)
        {
            return GetSkillInstance(skillID).EffectFunctionName;
        }

        /// <summary>
        /// 检查技能是否可用
        /// </summary>
        /// <param name="category"></param>
        /// <param name="skillID"></param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public bool CheckSkillAvailable(int skillID, out string msg)
        {
            return GetSkillInstance(skillID).CheckSkillAvailable(out msg);
        }

        /// <summary>
        /// 获得技能当前cd
        /// </summary>
        /// <returns></returns>
        public int GetSkillNowCDTurn(int skillID)
        {
            return GetSkillInstance(skillID).NowCD;
        }

        #endregion
        /// <summary>
        /// 单位更新
        /// </summary>
        /// <param name="elapseSeconds"></param>
        public void Update(float elapseSeconds)
        {
            m_UnitProp.UpdateData();
        }

        #region UI显示相关

        /// <summary>
        /// 获得单位头顶工具栏
        /// </summary>
        /// <returns></returns>
        public Transform GetUnitHeadObject()
        {
            //此处暂时使用基础数据
            return m_Instance.GetUnitHeadObject();
        }

        #endregion

        #region 不是功能性的方法
        public void Clear()
        {
            ActionProgressEvent = null;
            for (int i = 0; i < m_AllEffectTags.Length; i++)
            {
                m_AllEffectTags[i] = 0;
            }
            m_AIController.Release();
            m_Buff.Clear();
            m_BaseData.Clear();
            m_Skill.Clear();
        }

        public static CombatUnitInfo Create(Unit instance, AIBaseAction aiController, CombatSystem system)
        {
            CombatUnitInfo result = ReferencePool.Acquire<CombatUnitInfo>();
            result.m_Instance = instance;
            result.m_UnitProp = GameEntry.UnitProp.ClonePropData(instance.Prop);
            result.InstanceID = instance.InstanceID;
            result.m_ActionProgress = 0;
            result.m_Camp = instance.BaseData.Camp;
            result.m_AIController = aiController;
            result.m_CombatSystem = system;
            result.m_ActiveTurn = false;
            result.m_Buff.OnInit();
            result.m_BaseData.UpdateData(result);
            result.m_Skill.OnInit(result);
            return result;
        }

        public override string ToString()
        {
            return $"<color=#00ffff>InstanceID = {InstanceID}</color> --- <color=#ff00ff>UnitName = {m_BaseData.UnitName}</color>";
        }
        #endregion
    }
}