using BoringWorld.UI.RoleSkill;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 单位
	/// </summary>
	public partial class Unit : GameFramework.IReference
    {
        /// <summary>
        /// 单位原始属性数据
        /// </summary>
#pragma warning disable 0649
        private UnitDataRow m_SourceData;
#pragma warning restore 0649
        /// <summary>
        /// 单位属性
        /// </summary>
        private UnitProp m_UnitProp;
        /// <summary>
        /// 单位装备
        /// </summary>
        private UnitEquip m_UnitEquip;
        /// <summary>
        /// 单位实体逻辑
        /// </summary>
        private UnitEntityLogic m_EntityLogic;

        #region 对外展示属性

		public UnitDataRow BaseData { get { return m_SourceData; } }
		public int Lv { get { return m_Lv; } }
		public int NowExp { get { return m_NowExp; } }
		public int ExpLimit { get { return m_ExpLimit; } }
        public int SkillPoint { get { return m_SkillPoint; } }
        public int InstanceID { get { return m_InstanceID; } }
        public UnitProp Prop { get { return m_UnitProp; } }
        #endregion

#pragma warning disable 0649
        /// <summary>
        /// 单位等级，注意，在升级后需要检查技能是否有解锁的
        /// </summary>
        private int m_Lv;
        /// <summary>
        /// 当前等级经验值
        /// </summary>
        private int m_NowExp;
        /// <summary>
        /// 当前等级经验值上限
        /// </summary>
        private int m_ExpLimit;

        //以下为技能部分
        /// <summary>
        /// 单位技能数据
        /// </summary>
        private IUnitSkillMoudle m_UnitSkill;
        /// <summary>
        /// 人物技能点数
        /// </summary>
        private int m_SkillPoint;

        private int m_InstanceID;

#pragma warning restore 0649

        #region 移动相关

        private Stack<Vector3Int> m_MovePathStack;
        private bool m_Moveing;
        private float m_MoveTime;
        /// <summary>
        /// 移动长度（不一定要走完整个路径，基本上就是为战斗使用的）
        /// </summary>
        private int m_MoveLength;
        /// <summary>
        /// 移动完成之后的回调
        /// </summary>
        private System.Action m_MoveFinshCallback;

        #endregion

        /// <summary>
        /// 装备属性需要重新计算
        /// </summary>
        private bool m_EquipPropDirty;

        #region 私有方法

        /// <summary>
        /// 重新计算当前等级经验上限
        /// </summary>
        private void RecalculateExpLimit()
        {
            //todo,此处暂时使用这个格式
            m_ExpLimit = (m_Lv + 1) * 100;
        }

        /// <summary>
        /// 重新计算装备属性
        /// </summary>
        private void EquipPropertyRecalc()
        {
            var allProps = m_UnitEquip.RecalcProperty();
            for (int i = 0; i < allProps.Count; i++)
            {
                m_UnitProp.SetProperty(allProps[i].PropType, allProps[i].Category, allProps[i].ValueType, allProps[i].Value);
            }
            for (int i = 0; i < allProps.Count; i++)
            {
                allProps[i].Release();
            }
        }

        #endregion

        #region 公有方法

        public Unit()
        {
            
        }

        /// <summary>
        /// 隐藏单位（不同于隐身）,暂时做法
        /// </summary>
        public void Hide()
        {
            m_EntityLogic.gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示单位，暂时做法
        /// </summary>
        public void Show()
        {
            m_EntityLogic.gameObject.SetActive(true);
        }

        /// <summary>
        /// 创建单位时自动调用
        /// </summary>
        public void InitUnit(int instanceID, UnitEntityLogic entity, UnitDataRow baseData)
        {
            m_EntityLogic = entity;
            m_Moveing = false;
            m_InstanceID = instanceID;
            //todo，此处为基础做法，还有需要读档时的操作
            m_SourceData = baseData;
            m_Lv = m_SourceData.BaseLv;
            m_NowExp = 0;
            RecalculateExpLimit();
            m_UnitProp = GameEntry.UnitProp.GenerateUnitProp(BaseData.PropertyDataID);
            var row = GameEntry.DataTable.GetDataRow<UnitBasePropDataRow>(DataTableName.UNIT_BASE_PROPERTY_DATA_NAME, baseData.PropertyDataID);
            m_UnitEquip = ReferencePool.Acquire<UnitEquip>();
            m_UnitEquip.Init(this, m_SourceData.WeaponType);
            if (m_SourceData.BaseEquip != null)
            {
                foreach (var item in m_SourceData.BaseEquip)
                {
                    var newEquipData = EquipItemDataRow.GenerateInstance(item.Value);
                    ReplaceTheEquipment(item.Key, newEquipData, out var oldEquipData);
                }
            }
            //处理技能
            m_SkillPoint = m_SourceData.BaseSkillPoint;
            m_UnitSkill = UnitSkillModule.Create(this, m_SourceData.BaseSkill);
            //隐藏自身
            Hide();
        }

        /// <summary>
        /// 更改单位坐标
        /// </summary>
        /// <param name="newPosition"></param>
        public void TransformPosition(Vector3 newPosition)
        {
            m_EntityLogic.transform.localPosition = newPosition;
        }

        /// <summary>
        /// 获得单位坐标
        /// </summary>
        /// <returns></returns>
        public Vector3 GetUnitPosition()
        {
            return m_EntityLogic.transform.localPosition;
        }

        /// <summary>
        /// 获得单位头顶工具栏
        /// </summary>
        /// <returns></returns>
        public Transform GetUnitHeadObject()
        {
            return m_EntityLogic.GetUnitHeadObject();
        }

        /// <summary>
        /// 按照路径移动
        /// </summary>
        /// <param name="path"></param>
        public void Move(Stack<Vector3Int> path, int moveLength = -1, System.Action finshCallback = null)
        {
            if (moveLength == 0)
            {
                finshCallback?.Invoke();
                return;
            }
            m_MovePathStack = path;
            m_Moveing = true;
            m_MoveLength = moveLength;
            m_MoveFinshCallback = finshCallback;
        }

        /// <summary>
        /// 获得单位属性值的显示字符串
        /// </summary>
        /// <returns></returns>
        public string GetUnitPropValueDes(UnitPropType propType)
        {
			return m_UnitProp.GetPropValueDes(propType);
        }
        
        #region 技能相关

        /// <summary>
        /// 获得一个种类的全部技能信息
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public UnitSkillShowInfo[] GetUnitAllCategorySkillInfo(SkillCategory category)
        {
            return m_UnitSkill.GetSkillShowInfos(category);
        }

        /// <summary>
        /// 获得技能用于显示用信息
        /// </summary>
        /// <param name="category"></param>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public UnitSkillShowInfo GetSkillShowInfo(int skillID)
        {
            return m_UnitSkill.GetSkillShowInfo(skillID);
        }

        /// <summary>
        /// 获得技能实例
        /// </summary>
        /// <param name="category"></param>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public ISkillUIDisplayData GetSkillInstance(int skillID)
        {
            return m_UnitSkill.GetSkillInstance(skillID);
        }

        public ISkillUIDisplayData[] GetSkillInstances()
        {
            return m_UnitSkill.GetSkillInstances();
        }

        /// <summary>
        /// 尝试提升技能等级
        /// </summary>
        /// <param name="category"></param>
        /// <param name="skillID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool TryUpgradeSkillLevel(int skillID, out string message)
        {
            var instanceInfo = GetSkillInstance(skillID);
            if (instanceInfo.TryUpgradeSkillLevel(out message))
            {
                //减少技能点数
                var baseData = instanceInfo.BaseData;
                m_SkillPoint -= BaseData.LvUpgradeSkillPoint;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试降低技能等级
        /// </summary>
        /// <returns></returns>
        public bool TryLowerSkillLevel(int skillID, out string message)
        {
            var instanceInfo = GetSkillInstance(skillID);
            if (instanceInfo.TryLowerSkillLevel(out message))
            {
                //增加技能点数
                var baseData = instanceInfo.BaseData;
                m_SkillPoint += BaseData.LvUpgradeSkillPoint;
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 获得某个部位的穿戴装备
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public EquipItemDataRow GetUnitDressEquip(UnitEquipPartType part)
        {
            return m_UnitEquip.GetPartEquip(part);
        }

        /// <summary>
        /// 获得单位可以穿戴的装备类型
        /// </summary>
        /// <returns></returns>
        public EquipType GetUnitDressEquipType(UnitEquipPartType part)
        {
            return m_UnitEquip.GetDressEquipType(part);
        }

        /// <summary>
        /// 替换穿戴装备
        /// </summary>
        public bool ReplaceTheEquipment(UnitEquipPartType part, EquipItemDataRow equip, out EquipItemDataRow replaceEquip)
        {
            if (m_UnitEquip.DressEquip(part, equip, out replaceEquip))
            {
                m_EquipPropDirty = true;
                return true;
            }
            return false;
        }

        public void Updata()
        {
            m_UnitProp.UpdateData();
            if (m_EquipPropDirty)
            {
                EquipPropertyRecalc();
                m_EquipPropDirty = false;
            }
            if (m_Moveing)
            {
                m_MoveTime += Time.deltaTime;
                if (m_MoveTime >= GameMain.Config.MoveIntervalTime)
                {
                    m_MoveTime -= GameMain.Config.MoveIntervalTime;
                    TransformPosition(m_MovePathStack.Pop());
                    m_MoveLength--;
                    if (m_MovePathStack.Count <= 0 || m_MoveLength == 0)
                    {
                        m_Moveing = false;
                        m_MoveTime = 0;
                        m_MoveFinshCallback?.Invoke();
                    }
                }
            }
        }

        public void StartCoroutine(IEnumerator routine)
        {
            m_EntityLogic.StartCoroutine(routine);
        }

        #endregion

        public void Clear()
        {
            m_UnitProp.Release();
            m_UnitEquip.Release();
            m_UnitSkill.Release();
            m_UnitEquip = null;
            m_UnitProp = null;
            m_UnitSkill = null;
        }
	}
}