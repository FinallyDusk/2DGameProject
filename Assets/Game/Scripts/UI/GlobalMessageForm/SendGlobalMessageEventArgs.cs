using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    /// <summary>
    /// 全局消息事件
    /// </summary>
	public class SendGlobalMessageEventArgs : GameEventArgs
    {
        /// <summary>
        /// 全局消息
        /// </summary>
        public static readonly int EventId = typeof(SendGlobalMessageEventArgs).GetHashCode();
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public string Msg
        {
            get;
            set;
        }

        public static SendGlobalMessageEventArgs Create(string msg)
        {
            SendGlobalMessageEventArgs args = ReferencePool.Acquire<SendGlobalMessageEventArgs>();
            args.Msg = msg;
            return args;
        }
        
        public override void Clear()
        {
            Msg = null;
        }
    }
}