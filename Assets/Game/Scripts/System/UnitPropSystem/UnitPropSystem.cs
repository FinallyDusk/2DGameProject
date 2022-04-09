using GameFramework;
using GameFramework.DataTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    /// <summary>
    /// 单位属性系统
    /// </summary>
    public class UnitPropSystem : BaseSystem
    {
        private const int UNIT_PROP_TEMPLATE_ID = 0;
        private DataTableComponent m_DataTableManager;

        public override void OnEnter()
        {
            m_DataTableManager = GameEntry.DataTable;
        }

        public override void OnInit()
        {

        }

        /// <summary>
        /// 生成单位属性
        /// </summary>
        /// <returns></returns>
        public UnitProp GenerateUnitProp(int propID)
        {
            var row = GameEntry.DataTable.GetDataRow<UnitBasePropDataRow>(DataTableName.UNIT_BASE_PROPERTY_DATA_NAME, propID);
            UnitProp result = ReferencePool.Acquire<UnitProp>();
            result.Init(row.UnitAllBaseProp);
            return result;
        }

        /// <summary>
        /// 克隆一个数据，不会克隆事件
        /// </summary>
        /// <returns></returns>
        public UnitProp ClonePropData(UnitProp ori)
        {
            UnitProp result = ReferencePool.Acquire<UnitProp>();
            result.Init(ori);
            return result;
        }

        protected override void InternalPreLoadResources()
        {
            //执行预读完成后增加进度
            PreLoadFinsh();
        }

        public override void OnExit()
        {
            m_DataTableManager = null;
        }
    }
}