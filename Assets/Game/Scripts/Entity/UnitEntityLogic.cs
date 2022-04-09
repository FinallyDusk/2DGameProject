using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    /// <summary>
    /// 单位实体逻辑
    /// </summary>
    public class UnitEntityLogic : EntityLogic
    {
        private Transform m_HeadToolBar;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_HeadToolBar = transform.Find("HeadToolBar").transform;
        }

        public Transform GetUnitHeadObject()
        {
            return m_HeadToolBar;
        }
    }

    
}