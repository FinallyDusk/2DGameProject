using BoringWorld.Combat;
using BoringWorld.UI.CombatForm;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using XLua;

namespace BoringWorld
{
    using AStarUtil;

    [CSharpCallLua]
    public delegate void CombatUpdateAction(string flag);
    /// <summary>
    /// 战斗系统
    /// </summary>
    public partial class CombatSystem : BaseSystem, ITimerController, IAstarHelper
    {
        private static CombatSystem _Instance;
        private GameCheckpointSystem m_Checkpoint;
        private UnitSystem m_UnitSystem;
        private BuffSystem m_BuffSystem;
        public BuffSystem BuffManger { get { return m_BuffSystem; } }

        /// <summary>
        /// 战斗地图
        /// </summary>
        private CombatSceneLogic m_CombatMap;

        /// <summary>
        /// 所有战斗单位，key为单位实例ID
        /// </summary>
        private Dictionary<int, CombatUnitInfo> m_AllCombatUnits;
        private CombatFormLogic m_UIInstance;
        private CombatStatus m_TempStatus;
        private CombatStatus m_Status;
        /// <summary>
        /// 准备行动的单位
        /// </summary>
        private List<CombatUnitInfo> m_PrepareActionUnits;
        /// <summary>
        /// 当前活动单位
        /// </summary>
        private CombatUnitInfo m_NowActiveUnit;
        /// <summary>
        /// 当前操作类型
        /// </summary>
        private UnitActionType m_NowActionType;

        #region 进度条相关参数

        /// <summary>
        /// 每一个单位回合结束后到下一个单位的行动条间隔最长时间
        /// </summary>
        private const float ACTIVE_INTERVAL = 0.7f;
        /// <summary>
        /// 是否重新计算行动条速度
        /// </summary>
        private bool m_RecalcActiveSpeed;
        /// <summary>
        /// 行动条速度
        /// </summary>
        private double m_ActiveSpeed;

        #endregion
        /// <summary>
        /// 当前操作所需的参数
        /// </summary>
        private CombatActionArgs m_NowActionArgs;

        //战斗技能逻辑相关循环
        private CombatUpdateAction m_CombatUpdateAction;
        /// <summary>
        /// 所有战斗技能标志
        /// </summary>
        private Stack<string> m_AllSkillFlag;
        private float m_TimeScale;
        public float TimeScale { get { return m_TimeScale; } }
        /// <summary>
        /// 是否在战斗中
        /// </summary>
        public bool InCombat { get { return m_Status != CombatStatus.Idle; } }

        public override void OnInit()
        {
            base.OnInit();
            m_Checkpoint = GameEntry.Checkpoint;
            m_UnitSystem = GameEntry.Unit;
            m_AllCombatUnits = new Dictionary<int, CombatUnitInfo>();
            m_Status = CombatStatus.Idle;
            m_PrepareActionUnits = new List<CombatUnitInfo>();
            m_AllSkillFlag = new Stack<string>();
            _Instance = this;

            m_BuffSystem = new BuffSystem();
            m_BuffSystem.OnInit();
        }

        protected override void InternalPreLoadResources()
        {
            m_CombatMap = GameObject.FindGameObjectWithTag("CombatMap").GetComponent<CombatSceneLogic>();
            //m_CombatMap.gameObject.SetActive(false);
            m_CombatMap.OnInit(DisplayMessage, PreLoadFinsh);
            m_CombatUpdateAction = GameEntry.Lua.Env.Global.GetInPath<CombatUpdateAction>("Util.LuaUpdate");
        }
        /// <summary>
        /// 展示消息
        /// </summary>
        /// <param name="msg"></param>
        private void DisplayMessage(string msg)
        {
            m_UIInstance.DisplayRadioMessage(msg);
        }

        /// <summary>
        /// 注册行动进度事件
        /// </summary>
        public void RegisterActionProgressEvent(int instanceID, System.Action<double> callback)
        {
            if (m_AllCombatUnits.TryGetValue(instanceID, out var result))
            {
                result.ActionProgressEvent += callback;
            }
        }

        public void UnRegisterActionProgressEvent(int instanceID, System.Action<double> callback)
        {
            if (m_AllCombatUnits.TryGetValue(instanceID, out var result))
            {
                result.ActionProgressEvent -= callback;
            }
        }



        /// <summary>
        /// 开启战斗
        /// </summary>
        /// <param name="unitGroupID">单位组ID</param>
        public void OpenCombat(int unitGroupID)
        {
            PrepareEnterCombat(unitGroupID);
        }

        /// <summary>
        /// 进入战斗的预备工作
        /// </summary>
        private void PrepareEnterCombat(int unitGroup)
        {
            m_Status = CombatStatus.Idle;
            GameEntry.Event.FireNow(this, CombatEventArgs.Create(EffectTriggerCode.PrepareCombatStart, null, null));
            //重置一些属性
            m_TimeScale = 1;

            //打开战斗地图
            m_CombatMap.Open();
            //隐藏当前地图
            m_Checkpoint.HideCurrentMap();
            //禁用鼠标寻路功能
            GameMain.MouseGuide.SetActive(false);
            //加载战斗单位
            m_AllCombatUnits.Clear();
            //敌方单位
            //位置随机生成
            var allUnitInstance = m_Checkpoint.GetMapUnitInstanceIDs(unitGroup);
            CombatUnitInfo info = null;
            for (int i = 0; i < allUnitInstance.Count; i++)
            {
                info = CombatUnitInfo.Create(m_UnitSystem.GetUnit(allUnitInstance[i]), null, this);
                Vector3Int pos = new Vector3Int(/*Random.Range(4, 9), Random.Range(-4, 5)*/-5 + i, 3, 0);
                info.Position = pos;
                m_AllCombatUnits.Add(allUnitInstance[i], info);
            }
            //我方单位
            for (int i = 0; i < GameMain.GetPlayerAllUnitCount(); i++)
            {
                info = CombatUnitInfo.Create(GameMain.GetPlayerUnitByIndex(i), AIBaseAction.Create<PlayerAI>(null), this);
                info.Position = new Vector3Int(-8, 2 + i, 0);
                m_AllCombatUnits.Add(info.InstanceID, info);
            }
            //禁用esc
            GameEntry.Input.AddDisableKeyCodeDown(KeyCode.Escape);
            //单位显示
            foreach (var item in m_AllCombatUnits)
            {
                item.Value.Show();
            }
            //打开战斗UI
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OpenUIFormSuccessCallback);
            GameEntry.UI.OpenUIFormByType(UIFormType.CombatForm, this);
        }

        private void OpenUIFormSuccessCallback(object sender, System.EventArgs e)
        {
            var ue = e as OpenUIFormSuccessEventArgs;
            if (e == null)
            {
                return;
            }
            if (ue.UserData != this)
            {
                return;
            }
            if (m_UIInstance == null)
            {
                m_UIInstance = ue.UIForm.Logic as CombatFormLogic;
                //战斗界面动画播放完毕后正式进入战斗流程
                m_UIInstance.RegisterStartCombatAnimFinshCallback(StartCombat);
            }
            //将各个单位注入UI中
            foreach (var item in m_AllCombatUnits)
            {
                m_UIInstance.RegisterUnitDisplay(item.Value, item.Value.BaseData.Camp == UnitCamp.Player, true);
            }
            
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OpenUIFormSuccessCallback);
        }

        /// <summary>
        /// 开启战斗
        /// </summary>
        private void StartCombat()
        {
            //发送战斗开始的消息
            GameEntry.Event.Fire(this, CombatEventArgs.Create(EffectTriggerCode.CombatStart, null, null));
            m_Status = CombatStatus.Running;
            //重新计算行动条速度
            m_RecalcActiveSpeed = true;
        }

        public void Update(float elapseSeconds)
        {
            if (m_Status == CombatStatus.Idle) return;
            if (CheckEnterDisplayUnit()) return;
            elapseSeconds *= m_TimeScale;
            if (m_AllSkillFlag.Count > 0)
            {
                m_CombatUpdateAction?.Invoke(m_AllSkillFlag.Peek());
            }
            UpdateAllUnit(elapseSeconds);
            if (m_Status == CombatStatus.Running)
            {
                AllUnitAddActionProgress(elapseSeconds);
            }
        }

        /// <summary>
        /// 检查是否进入查看单位
        /// </summary>
        private bool CheckEnterDisplayUnit()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (m_Status == CombatStatus.LookUnitInfo)
                {
                    HideUnitDetailInfo();
                    return false;
                }
                m_TempStatus = m_Status;
                m_Status = CombatStatus.LookUnitInfo;
                m_UIInstance.EnableActionCancelCallback(true, HideUnitDetailInfo);
                m_TimeScale = 0;
                m_UIInstance.PauseActionGroup();
                //启用鼠标点击查看单位属性功能
                m_CombatMap.EnableCheckUnitInfoFunc(true, DisplayUnitDetailInfo);
                return true;
            }
            return m_Status == CombatStatus.LookUnitInfo;
        }

        /// <summary>
        /// 轮询所有单位
        /// </summary>
        private void UpdateAllUnit(float elapseSeconds)
        {
            foreach (var item in m_AllCombatUnits)
            {
                item.Value.Update(elapseSeconds);
            }
        }

        /// <summary>
        /// 所有单位增加行动条
        /// </summary>
        /// <param name="elapseSeconds">时间段</param>
        private void AllUnitAddActionProgress(float elapseSeconds)
        {
            //检查是否还有可行动单位
            if (m_PrepareActionUnits.Count > 0)
            {
                m_Status = CombatStatus.WaitForUnitAction;
                UnitToEnterActionTurn(m_PrepareActionUnits[0]);
                m_PrepareActionUnits.RemoveAt(0);
                return;
            }
            m_PrepareActionUnits.Clear();
            //计算行动条速度
            if (m_RecalcActiveSpeed)
            {
                double minTime = double.MaxValue;
                foreach (var item in m_AllCombatUnits)
                {
                    double unitTime = 0;
                    var unitSpeed = item.Value.Prop.GetProperty(UnitPropType.CombatSpeed);
                    if (unitSpeed <= 0)
                    {
                        unitTime = double.MaxValue;
                    }
                    else
                    {
                        unitTime = (100 - item.Value.ActionProgress) / unitSpeed;
                    }
                    if (minTime > unitTime)
                    {
                        minTime = unitTime;
                    }
                }
                //求出最小到达时间之后，需要和标准最小到达时间进行比较，然后放大对应的倍数
                if (minTime == double.MaxValue)
                {
                    Log.Warning("所有单位的速度都不大于0，请检查");
                    m_ActiveSpeed = 0;
                }
                else if (minTime > ACTIVE_INTERVAL)
                {
                    m_ActiveSpeed = minTime / ACTIVE_INTERVAL;
                }
                else
                {
                    m_ActiveSpeed = 1;
                }
                m_RecalcActiveSpeed = false;
            }
            //增加行动条值
            foreach (var item in m_AllCombatUnits)
            {
                item.Value.ActionProgress += item.Value.Prop.GetProperty(UnitPropType.CombatSpeed) * elapseSeconds * m_ActiveSpeed;
                if (item.Value.ActionProgress >= 100f)
                {
                    m_PrepareActionUnits.Add(item.Value);
                }
            }
            if (m_PrepareActionUnits.Count == 0) return;
            m_PrepareActionUnits.Sort((item1, item2) =>
            {
                if (item1.ActionProgress > item2.ActionProgress)
                {
                    return 1;
                }
                else if (item1.ActionProgress < item2.ActionProgress)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });
            //处理单位的行动
            m_Status = CombatStatus.WaitForUnitAction;
            UnitToEnterActionTurn(m_PrepareActionUnits[0]);
            m_PrepareActionUnits.RemoveAt(0);
        }

        /// <summary>
        /// 单位进入行动回合
        /// </summary>
        private void UnitToEnterActionTurn(CombatUnitInfo unit)
        {
            m_Status = CombatStatus.WaitForUnitAction;
            m_RecalcActiveSpeed = true;
            m_NowActiveUnit = unit;
            GameEntry.Event.FireNow(this, CombatEventArgs.Create(EffectTriggerCode.TurnStart, m_NowActiveUnit, null));
            m_NowActiveUnit.EnterActionTurn();
        }

        /// <summary>
        /// 显示漂浮数字
        /// </summary>
        public static void DisplayFloatNum(string num, Vector3 pos)
        {
            _Instance.m_UIInstance.DisplayFloatText(num, pos);
        }

        private void DisplayUnitDetailInfo(Vector3Int pos)
        {
            var unit = GetUnitByPosition(pos);
            if (unit == null) return;
            var data = ReferencePool.Acquire<DisplayInfoData>();
            data.UnitName = unit.BaseData.UnitName;
            data.UnitLv = unit.BaseData.Lv;
            data.NowHp = unit.Prop.GetProperty(UnitPropType.NowHp);
            data.MaxHp = unit.Prop.GetProperty(UnitPropType.MaxHp);
            data.NowMp = unit.Prop.GetProperty(UnitPropType.NowMp);
            data.MaxMp = unit.Prop.GetProperty(UnitPropType.MaxMp);
            var objs = new object[4];
            objs[0] = unit.BaseData.UnitStory;
            objs[1] = unit.Prop;
            objs[2] = unit.Skill.GetAllSkill();
            data.UserDatas = objs;
            m_UIInstance.DisplayUnitInfo(data);
            data.Release();
        }

        private void HideUnitDetailInfo()
        {
            m_Status = m_TempStatus;
            m_TimeScale = 1;
            //回复操作组状态
            m_UIInstance.ResumeActionGroup();
            //禁用鼠标右键和esc
            m_UIInstance.EnableActionCancelCallback(false);
            m_UIInstance.HideUnitInfo();
            m_CombatMap.EnableCheckUnitInfoFunc(false);
        }


        #region 单位选取

        /// <summary>
        /// 圆形选取单位
        /// </summary>
        /// <returns></returns>
        public List<CombatUnitInfo> GetAllUnitsByCircle(Vector3Int center, int length, System.Func<CombatUnitInfo, bool> condition = null)
        {
            List<CombatUnitInfo> result = new List<CombatUnitInfo>();
            foreach (var item in m_AllCombatUnits)
            {
                var distance = Mathf.Abs(item.Value.Position.x - center.x) + Mathf.Abs(item.Value.Position.y - center.y);
                if (distance <= length)
                {
                    if (condition != null)
                    {
                        if (condition.Invoke(item.Value))
                        {
                            result.Add(item.Value);
                        }
                    }
                    else
                    {
                        result.Add(item.Value);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获得某一个阵营所有单位
        /// </summary>
        /// <returns></returns>
        public List<CombatUnitInfo> GetAllUnitsByCamp(UnitCamp camp)
        {
            List<CombatUnitInfo> result = new List<CombatUnitInfo>();
            foreach (var item in m_AllCombatUnits)
            {
                if (item.Value.BaseData.Camp == camp)
                {
                    result.Add(item.Value);
                }
            }
            return result;
        }

        #endregion

        #region A*寻路相关

        /// <summary>
        /// 坐标位置是否有效（能否通行）,true表示可以通行，false为不能通行
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool PositionIsValid(Vector3Int pos, bool ingoreUnit)
        {
            if (EndPathForward(pos))
            {
                return false;
            }
            //todo，暂时不管地形
            if (ingoreUnit == false)
            {
                var unit = GetUnitByPosition(pos);
                if (unit == null) return true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 前方路径是否已经结束,true为结束，false为未结束
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool EndPathForward(Vector3Int pos)
        {
            return m_CombatMap.EndPathForward(pos);
        }

        #endregion

        /// <summary>
        /// 获得一条线在场地的结束位置
        /// </summary>
        /// <returns></returns>
        public Vector3Int GetLineEndPosition(Vector3Int start, Vector3Int direction)
        {
            var pos = start;
            while (m_CombatMap.EndPathForward(pos) == false)
            {
                pos = pos + direction;
            }
            return pos;
        }

        #region 单位操作的实现流程，内部

        #region 攻击操作

        /// <summary>
        /// 开始攻击操作
        /// </summary>
        private void InternalStartAttackAction()
        {
            //隐藏操作面板
            m_UIInstance.PauseActionGroup();
            //开启esc和右键退出攻击选项功能
            m_UIInstance.EnableActionCancelCallback(true, CancelAttackAction);
            //显示可攻击范围
            //获取攻击距离
            int atkDistance = m_NowActiveUnit.BaseData.AtkRange;
            List<Vector3Int> allPoints = new List<Vector3Int>();
            Util.AStar.FindAllPositionByCirle(m_NowActiveUnit.Position, atkDistance, this, true, allPoints);
            m_CombatMap.DisplayChooseableRange(allPoints.ToArray());
            //开启鼠标指引器
            m_CombatMap.EnableChoosePointEvent(true, CheckAttackPosition);
        }

        /// <summary>
        /// 检查准备攻击的目标
        /// </summary>
        /// <param name="pos"></param>
        private void CheckAttackPosition(Vector3Int pos)
        {
            foreach (var item in m_AllCombatUnits)
            {
                if (item.Value.Position == pos)
                {
                    if (item.Value.BaseData.Camp == m_NowActiveUnit.BaseData.Camp)
                    {
                        m_UIInstance.DisplayRadioMessage("不允许选择友军目标");
                    }
                    else
                    {
                        //关闭操作面板
                        m_UIInstance.CloseActionGroup();
                        //关闭鼠标指引器
                        m_CombatMap.EnableChoosePointEvent(false);
                        //关闭选择范围绘制
                        m_CombatMap.HideChooseRange();
                        m_UIInstance.EnableActionCancelCallback(false);
                        //增加一个攻击操作
                        ExecuteAttackAction(m_NowActiveUnit, item.Value);
                        m_NowActionArgs = CombatActionArgs.Create(m_NowActiveUnit);
                        m_NowActionArgs.AddActionTarget(item.Value);
                    }
                    return;
                }
            }
            m_UIInstance.DisplayRadioMessage("请选择一个目标");
        }

        /// <summary>
        /// 执行攻击操作
        /// </summary>
        private void ExecuteAttackAction(CombatUnitInfo atkSource, CombatUnitInfo atkTarges)
        {
            //移动到最近的攻击范围
            var movePath = Util.AStar.StartAStar(atkSource.Position, atkTarges.Position, true, false, this);
            atkSource.Move(movePath, Mathf.Max(0, movePath.Count - atkSource.BaseData.AtkDistance), AttackPreMoveFinsh);
        }

        /// <summary>
        /// 攻击前移动完成后
        /// </summary>
        private void AttackPreMoveFinsh()
        {
            m_NowActionArgs.ActionSource.AppendAttack(m_NowActionArgs, AttackFinsh);
        }

        /// <summary>
        /// 攻击完成
        /// </summary>
        private void AttackFinsh()
        {
            //结束攻击操作
            m_UIInstance.StartCoroutine(IEEndAction());
        }

        /// <summary>
        /// 取消攻击操作
        /// </summary>
        private void CancelAttackAction()
        {
            //开启操作面板
            m_UIInstance.ResumeActionGroup();
            //禁用取消操作
            m_UIInstance.EnableActionCancelCallback(false);
            //取消显示可选择范围
            m_CombatMap.HideChooseRange();
            //禁用鼠标指引器
            m_CombatMap.EnableChoosePointEvent(false);
        }

        #endregion
        #region 移动操作

        /// <summary>
        /// 内部开始执行移动操作
        /// </summary>
        private void InternalStartMoveAction()
        {
            //地图显示可选取范围
            List<Vector3Int> result = new List<Vector3Int>();
            Util.AStar.FindAllPositionByCirle(m_NowActiveUnit.Position, m_NowActiveUnit.BaseData.MoveRange, this, false, result);
            m_CombatMap.DisplayChooseableRange(result.ToArray());
            //暂停操作按钮组
            m_UIInstance.PauseActionGroup();
            //开启esc和右键退出操作功能
            m_UIInstance.EnableActionCancelCallback(true, CancelMoveAction);
            //开启鼠标指引器
            m_CombatMap.EnableChoosePointEvent(true, MoveTargetCheck);
        }

        private void CancelMoveAction()
        {
            //关闭地图范围选择显示
            m_CombatMap.HideChooseRange();
            //恢复操作按钮组
            m_UIInstance.ResumeActionGroup();
            //禁用退出操作
            m_UIInstance.EnableActionCancelCallback(false);
            //关闭地图鼠标指引
            m_CombatMap.EnableChoosePointEvent(false);
        }

        /// <summary>
        /// 检查移动目标是否可行
        /// </summary>
        private void MoveTargetCheck(Vector3Int pos)
        {
            foreach (var item in m_AllCombatUnits)
            {
                if (item.Value.Position == pos)
                {
                    m_UIInstance.DisplayRadioMessage("此处已经存在单位，无法移动");
                    return;
                }
            }
            //关闭操作面板
            m_UIInstance.CloseActionGroup();
            //关闭地图范围选择显示
            m_CombatMap.HideChooseRange();
            //禁用退出操作
            m_UIInstance.EnableActionCancelCallback(false);
            //关闭地图鼠标指引
            m_CombatMap.EnableChoosePointEvent(false);
            //增加移动操作
            var path = Util.AStar.StartAStar(m_NowActiveUnit.Position, pos, false, false, this);
            m_NowActiveUnit.Move(path, path.Count, EndMoveAction);
        }

        /// <summary>
        /// 移动操作结束
        /// </summary>
        private void EndMoveAction()
        {
            m_UIInstance.StartCoroutine(IEEndAction());
        }

        #endregion

        #region 内部技能操作

        /// <summary>
        /// 内部开始技能操作
        /// </summary>
        private void InternalStartSkillAction()
        {
            var allInitiativeSkill = m_NowActiveUnit.GetUnitAllCategorySkillInfo(SkillCategory.Initiative);
            SkillChildrenItemData[] args = new SkillChildrenItemData[allInitiativeSkill.Length];
            for (int i = 0; i < allInitiativeSkill.Length; i++)
            {
                int nowCD = m_NowActiveUnit.Skill.GetSkillData(allInitiativeSkill[i].SkillID).NowCD;
                if (int.TryParse(allInitiativeSkill[i].CDTurn, out int maxCD) == false)
                {
                    maxCD = 99;
                }
                args[i] = SkillChildrenItemData.Create(allInitiativeSkill[i].SkillID, allInitiativeSkill[i].Name, allInitiativeSkill[i].Des, nowCD, maxCD);
            }
            //展开全部技能操作组
            m_UIInstance.DisplaySkillChildrenActionBtn(args, CheckInitiativeSkillCondition);
            //启用取消操作
            m_UIInstance.EnableActionCancelCallback(true, CancelChooseSkillAction);
        }

        /// <summary>
        /// 取消选择技能按钮
        /// </summary>
        private void CancelChooseSkillAction()
        {
            //回归第一层级操作按钮组
            m_UIInstance.ShowActionGroup();
            //取消操作禁用
            m_UIInstance.EnableActionCancelCallback(false);
        }

        /// <summary>
        /// 检查技能是否可以使用
        /// </summary>
        /// <param name="skillID"></param>
        private void CheckInitiativeSkillCondition(int skillID)
        {
            if (m_NowActiveUnit.Skill.GetSkillData(skillID).CheckSkillAvailable(out var msg))
            {
                //执行技能
                PrepareExecuteSkill(skillID);
            }
            else
            {
                //执行失败，显示失败消息
                m_UIInstance.DisplayRadioMessage(msg);
            }
        }

        /// <summary>
        /// 准备执行技能
        /// </summary>
        /// <param name="skillID"></param>
        private void PrepareExecuteSkill(int skillID)
        {
            //获得技能实例
            var skillInstance = m_NowActiveUnit.Skill.GetSkillData(skillID);
            m_NowActiveUnit.Skill.PrepareUseSkill = skillInstance;
            //创建技能信息参数
            //m_NowActionArgs = CombatSkillArgs.Create(m_NowActiveUnit, skillInstance, SkillCategory.Initiative, m_NowActiveUnit, ExecuteSkillFinsh, new EffectTriggerCode[] { EffectTriggerCode.None });
            if ((skillInstance.CastTarget & SkillCastTarget.NoTarget) == SkillCastTarget.NoTarget)
            {
                //添加技能执行
                //表示立即施法技能
                SetSkillActionArg(Vector3Int.zero);
            }
            else
            {
                List<Vector3Int> allPoints = new List<Vector3Int>();
                //根据技能范围参数进行操作
                switch (skillInstance.CastRangeCategory)
                {
                    case SkillCastRangeCategory.Square:
                        Util.AStar.FindAllPositionBySquare(m_NowActiveUnit.Position, skillInstance.CastDistance, this, true, allPoints);
                        break;
                    case SkillCastRangeCategory.Circle:
                        Util.AStar.FindAllPositionByCirle(m_NowActiveUnit.Position, skillInstance.CastDistance, this, true, allPoints);
                        break;
                    case SkillCastRangeCategory.Row:
                        Util.AStar.FindAllPositionByRow(m_NowActiveUnit.Position, skillInstance.CastDistance, this, true, allPoints);
                        break;
                    default:
                        throw new System.NotImplementedException("暂时还未实现该类型的施法范围 args.CastRangeCategory" + skillInstance.CastRangeCategory);
                }
                m_CombatMap.DisplayChooseableRange(allPoints.ToArray());
                //检测点击
                m_CombatMap.EnableChoosePointEvent(true, CheckCastTarget);
            }
            //暂停操作面板
            m_UIInstance.PauseActionGroup();
            //启用取消操作
            m_UIInstance.EnableActionCancelCallback(true, CancelExecuteSkill);
        }

        private void CancelExecuteSkill()
        {
            //回复操作面板
            m_UIInstance.ResumeActionGroup();
            //关闭范围选择
            m_CombatMap.HideChooseRange();
            //启用返回上一级的取消操作
            m_UIInstance.EnableActionCancelCallback(true, CancelChooseSkillAction);
            //禁用点击检测
            m_CombatMap.EnableChoosePointEvent(false);
        }

        /// <summary>
        /// 检查是否目标
        /// </summary>
        /// <param name="pos"></param>
        private void CheckCastTarget(Vector3Int pos)
        {
            var castTarget = m_NowActiveUnit.Skill.PrepareUseSkill.CastTarget;
            var unit = GetUnitByPosition(pos);
            if (unit == null)
            {
                if ((castTarget & SkillCastTarget.NoneUnit) == SkillCastTarget.NoneUnit)
                {
                    SetSkillActionArg(pos);
                    return;
                }
            }
            if ((castTarget & SkillCastTarget.Enemy) == SkillCastTarget.Enemy)
            {
                if (unit.BaseData.Camp != m_NowActiveUnit.BaseData.Camp)
                {
                    SetSkillActionArg(pos);
                    return;
                }
            }
            if ((castTarget & SkillCastTarget.Ally) == SkillCastTarget.Ally)
            {
                if (unit.BaseData.Camp == m_NowActiveUnit.BaseData.Camp && unit != m_NowActiveUnit)
                {
                    SetSkillActionArg(pos);
                    return;
                }
            }
            if ((castTarget & SkillCastTarget.Self) == SkillCastTarget.Self)
            {
                if (unit == m_NowActiveUnit)
                {
                    SetSkillActionArg(pos);
                    return;
                }
            }
            m_NowActiveUnit.Skill.PrepareUseSkill = null;
        }

        private void SetSkillActionArg(Vector3Int pos)
        {

            m_NowActionArgs = CombatActionArgs.Create(m_NowActiveUnit);
            m_NowActionArgs.ActionPoint = pos;
            var unit = GetUnitByPosition(pos);
            if (unit != null)
            {
                m_NowActionArgs.AddActionTarget(unit);
            }
            m_NowActionArgs.SkillInstance = m_NowActiveUnit.Skill.PrepareUseSkill;
            m_NowActionArgs.AddCompleteCallback(ExecuteSkillFinsh);

            //执行技能
            m_NowActiveUnit.ExecuteSkill(m_NowActionArgs);
            //关闭操作面板
            m_UIInstance.CloseActionGroup();
            //取消范围选择绘制
            m_CombatMap.HideChooseRange();
            //禁用取消操作
            m_UIInstance.EnableActionCancelCallback(false);
            //禁用鼠标点击
            m_CombatMap.EnableChoosePointEvent(false);
        }

        /// <summary>
        /// 技能执行完毕
        /// </summary>
        private void ExecuteSkillFinsh()
        {
            m_UIInstance.StartCoroutine(IEEndAction());
        }

        #endregion


        #endregion

        private CombatUnitInfo GetUnitByPosition(Vector3Int pos)
        {
            foreach (var item in m_AllCombatUnits)
            {
                if (item.Value.Position == pos)
                {
                    return item.Value;
                }
            }
            return null;
        }

        #region 战斗相关的公共方法

        /// <summary>
        /// 单位操作按钮点击
        /// </summary>
        /// <param name="type">操作类型</param>
        public void UnitActionBtnClick(UnitActionType type)
        {
            if (m_Status == CombatStatus.LookUnitInfo) return;
            m_NowActionType = type;
            switch (type)
            {
                case UnitActionType.Attack:
                    InternalStartAttackAction();
                    break;
                case UnitActionType.Move:
                    InternalStartMoveAction();
                    break;
                case UnitActionType.Skill:
                    InternalStartSkillAction();
                    break;
                case UnitActionType.Prop:
                    break;
                case UnitActionType.Runaway:
                    break;
                default:
                    throw new System.NotImplementedException($"还未实现{type.ToString()}类型的动作");
            }

        }

        /// <summary>
        /// 结束操作
        /// </summary>
        /// <returns></returns>
        private IEnumerator IEEndAction()
        {
            //根据对应的操作减少对应的行动值
            var value = GameEntry.Config.GetFloat($"CombatSystem.{m_NowActionType.ToString()}");
            m_NowActiveUnit.ActionProgress -= value;
            //等待行动值回退动画
            //此处应该使用回调来制作，暂时就这样
            //todo
            yield return new WaitForSeconds(GameEntry.Config.GetFloat("CombatSystem.EndActionWaitAnimTime"));
            //发送回合结束消息
            GameEntry.Event.FireNow(this, CombatEventArgs.Create(EffectTriggerCode.TurnEnd, m_NowActiveUnit, null));
            //状态变为普通
            m_Status = CombatStatus.Running;

            m_NowActionArgs.Release();
            m_NowActionArgs = null;
        }


        /// <summary>
        /// 显示可操作按钮组
        /// </summary>
        public void ShowActionGroup()
        {
            m_UIInstance.ShowActionGroup();
        }

        #endregion

        /// <summary>
        /// 战斗状态
        /// </summary>
        private enum CombatStatus
        {
            /// <summary>
            /// 待机状态
            /// </summary>
            Idle,
            /// <summary>
            /// 运行中
            /// </summary>
            Running,
            /// <summary>
            /// 等待单位行动
            /// </summary>
            WaitForUnitAction,
            /// <summary>
            /// 查看单位信息中
            /// </summary>
            LookUnitInfo,
        }


        #region 计时器

        public float UpdateDeltaTime(float deltaTime)
        {
            return deltaTime * m_TimeScale;
        }

        /// <summary>
        /// 创建计时器,已经初始化了
        /// </summary>
        /// <returns></returns>
        public static GameTimerItem CreateTimer(int count, float intervalTime, System.Action<GameTimerExecuteData> callback, bool startTrigger = false)
        {
            GameTimerItem timer = GameEntry.GameTime.GetTimer();
            timer.InitItem(count, intervalTime, callback, startTrigger, _Instance);
            return timer;
        }

        #endregion
    }

    /// <summary>
    /// 单位操作类型
    /// </summary>
    public enum UnitActionType
    {
        /// <summary>
        /// 攻击
        /// </summary>
        Attack,
        /// <summary>
        /// 移动
        /// </summary>
        Move,
        /// <summary>
        /// 技能
        /// </summary>
        Skill,
        /// <summary>
        /// 道具
        /// </summary>
        Prop,
        /// <summary>
        /// 逃跑
        /// </summary>
        Runaway,
    }
}