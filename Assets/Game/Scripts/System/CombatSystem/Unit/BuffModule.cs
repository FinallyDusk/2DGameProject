using BoringWorld.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    public partial class CombatUnitInfo
    {
        /// <summary>
            /// 单位的buff模块（管理）
            /// </summary>
        public class BuffModule
        {
            /// <summary>
            /// 所有buff数据
            /// </summary>
            private List<BuffInstanceData> m_AllBuff;
            private CombatUnitInfo m_Belong;
            private Func<int, Action<BuffInstanceData>> m_AddBuffUIAction;
            private Action<int> m_RemoveBuffUIAction;

            public BuffModule(CombatUnitInfo belong)
            {
                m_AllBuff = new List<BuffInstanceData>();
                m_Belong = belong;
            }

            public void OnInit()
            {
                m_AllBuff.Clear();
            }

            public void RegisterBuffDisplay(System.Func<int, Action<BuffInstanceData>> addBuff, Action<int> removeBuff)
            {
                m_AddBuffUIAction = addBuff;
                m_RemoveBuffUIAction = removeBuff;
            }

            /// <summary>
            /// 添加buff
            /// </summary>
            public void AddBuff(BuffInstanceData buff)
            {
                var action = m_AddBuffUIAction.Invoke(buff.InstanceID);
                buff.AddToUnit(m_Belong, action);
            }

            public void RemoveBuff(BuffInstanceData buff)
            {
                m_RemoveBuffUIAction?.Invoke(buff.InstanceID);
                buff.Release();
            }

            public void Clear()
            {
                foreach (var item in m_AllBuff)
                {
                    item.Release();
                }
                m_AllBuff.Clear();
            }
        }
    }
}