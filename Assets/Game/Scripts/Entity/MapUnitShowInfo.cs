using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    /// <summary>
    /// 地图单位显示信息
    /// </summary>
    public class MapUnitShowInfo : IReference
    {
        public int UnitGroupID;

        public void Clear()
        {

        }

        public static MapUnitShowInfo Create(int unitGroupID)
        {
            MapUnitShowInfo result = ReferencePool.Acquire<MapUnitShowInfo>();
            result.UnitGroupID = unitGroupID;
            return result;
        }
    }
}