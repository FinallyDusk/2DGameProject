using GameFramework;
using GameFramework.DataTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    /// <summary>
    /// 单位系统
    /// </summary>
    public class UnitSystem : BaseSystem
    {
        public const int DEFAULT_UNIT_INSTANCE_ID = 0;

        private IDataTable<UnitDataRow> m_AllUnitData;
        /// <summary>
        /// 所有实例单位字典
        /// </summary>
        private Dictionary<int, Unit> m_AllInstanceUnitDict;
        /// <summary>
        /// 当前实例化ID
        /// </summary>
        private int m_CurrentInstanceId;

        public override void OnEnter()
        {
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventID, ShowEntitySuccessCallback);
        }

        public override void OnInit()
        {
            m_AllInstanceUnitDict = new Dictionary<int, Unit>();
            m_CurrentInstanceId = 0;
        }

        protected override void InternalPreLoadResources()
        {
            m_AllUnitData = GameEntry.DataTable.GetDataTable<UnitDataRow>(DataTableName.UNIT_DATA_NAME);
            GameEntry.Event.Subscribe(CreateUnitSuccessEventArgs.EventID, CreateDefaultUnitSuccessCallback);
            //加载默认单位
            CreateUnit(DEFAULT_UNIT_INSTANCE_ID, null);
        }

        public int GetCurrentEntityID()
        {
            return m_CurrentInstanceId++;
        }

        /// <summary>
        /// 创建单位实例
        /// </summary>
        /// <returns>返回一个实例ID</returns>
        /// todo
        /// 此处系统未对Unit进行回收（一般在单位死亡的地方）
        public int CreateUnit(int unitDataID, object userData)
        {
            var row = m_AllUnitData.GetDataRow(unitDataID);
            if (row == null)
            {
                Log.Error($"查找不到unitID = {unitDataID} 的数据，无法创建指定单位，创建默认单位");
                row = m_AllUnitData.GetDataRow(DEFAULT_UNIT_INSTANCE_ID);
            }
            int unitInstanceID = m_CurrentInstanceId;
            m_CurrentInstanceId++;
            //读取实体
            GameEntry.Entity.ShowEntity<UnitEntityLogic>(unitInstanceID, row.EntityAssetPath, EntityGroup.Unit, CreateUnitArgs.Create(this, row, userData));
            return unitInstanceID;
        }

        private void ShowEntitySuccessCallback(object sender, System.EventArgs e)
        {
            ShowEntitySuccessEventArgs se = e as ShowEntitySuccessEventArgs;
            if (se == null)
            {
                Log.Fatal($"显示实体出错，请检查");
                return;
            }
            CreateUnitArgs cu = se.UserData as CreateUnitArgs;
            if (cu == null || cu.Sender != this)
            {
                return;
            }
            Unit unit = ReferencePool.Acquire<Unit>();
            //todo,此处进行对unit初始化的一系列操作
            var logic = se.Entity.Logic as UnitEntityLogic;
            unit.InitUnit(se.Entity.Id, logic, cu.UnitBaseData);

            m_AllInstanceUnitDict.Add(se.Entity.Id, unit);
            GameEntry.Event.Fire(this, CreateUnitSuccessEventArgs.Create(se.Entity.Id, unit, cu.UserData));
        }

        private void CreateDefaultUnitSuccessCallback(object sender, System.EventArgs e)
        {
            CreateUnitSuccessEventArgs ce = e as CreateUnitSuccessEventArgs;
            if (ce.UnitInstanceID != DEFAULT_UNIT_INSTANCE_ID)
            {
                return;
            }
            //ce.UnitInstance.Hide();
            GameEntry.Event.Unsubscribe(CreateUnitSuccessEventArgs.EventID, CreateDefaultUnitSuccessCallback);
            PreLoadFinsh();
        }

        /// <summary>
        /// 获取单位
        /// </summary>
        /// <returns></returns>
        public Unit GetUnit(int unitInstanceID)
        {
            if (m_AllInstanceUnitDict.TryGetValue(unitInstanceID, out var result))
            {
                return result;
            }
            else
            {
                Log.Error($"获取不到InstanceID = {unitInstanceID} 的单位，请检查ID是否正确或者是否创建了实体");
                return m_AllInstanceUnitDict[DEFAULT_UNIT_INSTANCE_ID];
            }
        }

        public void Update()
        {
            foreach (var item in m_AllInstanceUnitDict)
            {
                item.Value.Updata();
            }
        }

        public override void OnExit()
        {
            m_AllUnitData = null;
            m_AllInstanceUnitDict.Clear();
            m_CurrentInstanceId = 0;
        }

        private class CreateUnitArgs : IReference
        {
            public object UserData;
            public UnitDataRow UnitBaseData;
            public UnitSystem Sender;

            public static CreateUnitArgs Create(UnitSystem sender, UnitDataRow baseData, object userData)
            {
                CreateUnitArgs result = ReferencePool.Acquire<CreateUnitArgs>();
                result.Sender = sender;
                result.UnitBaseData = baseData;
                result.UserData = userData;
                return result;
            }

            public void Clear()
            {
                UserData = null;
                Sender = null;
            }
        }
        
    }
}