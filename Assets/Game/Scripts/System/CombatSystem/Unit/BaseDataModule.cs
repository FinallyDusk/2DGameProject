using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    public partial class CombatUnitInfo
    {
        /// <summary>
        /// 基础属性
        /// </summary>
        public class BaseDataModule
        {
            /// <summary>
            /// 单位名
            /// </summary>
            public string UnitName { get; private set; }
            /// <summary>
            /// 单位故事背景
            /// </summary>
            public string UnitStory { get; private set; }
            /// <summary>
            /// 单位阵营
            /// </summary>
            public UnitCamp Camp { get; private set; }
            /// <summary>
            /// 可以攻击的范围
            /// </summary>
            public int AtkRange { get; private set; }
            /// <summary>
            /// 攻击距离
            /// </summary>
            public int AtkDistance { get; private set; }
            /// <summary>
            /// 可移动范围
            /// </summary>
            public int MoveRange { get; private set; }
            /// <summary>
            /// 攻击伤害类型
            /// </summary>
            public HarmType AtkHarmType { get; private set; }
            /// <summary>
            /// 攻击特效
            /// </summary>
            public SpecialEffectConfig AtkSpecialEffect { get; private set; }
            /// <summary>
            /// 战斗图标名
            /// </summary>
            public string CombatActionIcon { get; private set; }
            /// <summary>
            /// 地图显示图标名
            /// </summary>
            public string MapDisplayIconName { get; private set; }
            /// <summary>
            /// 等级
            /// </summary>
            public int Lv { get; private set; }
            private CombatUnitInfo m_Belong;

            public void UpdateData(CombatUnitInfo belong)
            {
                m_Belong = belong;
                UnitName = m_Belong.m_Instance.BaseData.UnitName;
                UnitStory = m_Belong.m_Instance.BaseData.UnitStory;
                Camp = m_Belong.m_Instance.BaseData.Camp;
                AtkDistance = m_Belong.m_Instance.BaseData.AtkDistance;
                MoveRange = m_Belong.m_Instance.BaseData.MoveDistance;
                AtkRange = AtkDistance + MoveRange;
                AtkSpecialEffect = m_Belong.m_Instance.BaseData.AtkSpecialEffect;
                CombatActionIcon = m_Belong.m_Instance.BaseData.CombatActionIcon;
                MapDisplayIconName = m_Belong.m_Instance.BaseData.MapDisplayIconName;
                Lv = m_Belong.m_Instance.Lv;
            }

            public void Clear()
            {

            }
        }
    }
}