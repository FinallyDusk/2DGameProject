using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	public class CreateUnitSuccessEventArgs : GameEventArgs
	{
        public static int EventID { get { return GameFrameworkExtension.GetEventID(nameof(CreateUnitSuccessEventArgs)); } }
        public override int Id
        {
            get
            {
                return EventID;
            }
        }

        /// <summary>
        /// 单位实例ID（<see cref="Unit"/>）
        /// </summary>
        public int UnitInstanceID { get; private set; }

        /// <summary>
        /// 显示的单位实例
        /// </summary>
        public Unit UnitInstance { get; private set; }

        public object UserData { get; private set; }

        public static CreateUnitSuccessEventArgs Create(int entityInstanceID, Unit unitInstance, object userData)
        {
            CreateUnitSuccessEventArgs result = ReferencePool.Acquire<CreateUnitSuccessEventArgs>();
            result.UnitInstanceID = entityInstanceID;
            result.UnitInstance = unitInstance;
            result.UserData = userData;
            return result;
        }

        public override void Clear()
        {
            UnitInstanceID = 0;
        }
    }
}