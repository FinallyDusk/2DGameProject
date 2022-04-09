using BoringWorld.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{

    public partial class CombatSystem
    {
        /// <summary>
        /// buff系统，一般只有战斗会用
        /// </summary>
        /// 注意：
        /// 1. buff没有等级
        /// 2. buff持续时间以回合数为标准，-1为无限持续时间
        /// 3. buff有特有标签
        /// 4. buff的大部分数据可以由技能进行重载
        public class BuffSystem
        {
            private int m_InstanceID;

            public void OnInit()
            {
                m_InstanceID = 0;
            }

            public BuffInstanceData GenerateBuffInstance(int buffID)
            {
                var row = GameEntry.DataTable.GetDataRow<BuffDataRow>(DataTableName.BUFF_DATA_NAME, buffID);
                return BuffInstanceData.Create(m_InstanceID++, row);
            }
        }
    }
}