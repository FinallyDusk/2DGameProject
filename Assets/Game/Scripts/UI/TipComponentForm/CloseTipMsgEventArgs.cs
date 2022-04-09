using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	public class CloseTipMsgEventArgs : GameEventArgs
    {
        /// <summary>
        /// 提示信息
        /// </summary>
        public static readonly int EventId = typeof(CloseTipMsgEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static CloseTipMsgEventArgs Create()
        {
            return ReferencePool.Acquire<CloseTipMsgEventArgs>();
        }

        public override void Clear()
        {
            
        }
    }
}