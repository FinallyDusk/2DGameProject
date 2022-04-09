using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Event;
using GameFramework;

namespace BoringWorld
{
    /// <summary>
    /// 提示信息事件
    /// </summary>
	public class SendTipMsgEventArgs : GameEventArgs
    {
        /// <summary>
        /// 提示信息
        /// </summary>
        public static readonly int EventId = typeof(SendTipMsgEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public string NormalMsg
        {
            get;
            private set;
        }

        public string CompareMsg
        {
            get;
            private set;
        }

        public Rect Size
        {
            get;
            private set;
        }

        public override void Clear()
        {
            CompareMsg = NormalMsg = null;
            Size = Rect.zero;
        }

        /// <summary>
        /// 创建一个提示
        /// </summary>
        /// <param name="normalMsg">一般提示</param>
        /// <param name="compareMsg">需要进行对比的提示</param>
        /// <param name="size">一般是Rect(RectTransform.Position, RectTransform.SizeDelta)</param>
        /// <returns></returns>
        public static SendTipMsgEventArgs Create(string normalMsg, string compareMsg, Rect size)
        {
            var args = ReferencePool.Acquire<SendTipMsgEventArgs>();
            args.NormalMsg = normalMsg;
            args.CompareMsg = compareMsg;
            args.Size = size;
            return args;
        }
    }
}