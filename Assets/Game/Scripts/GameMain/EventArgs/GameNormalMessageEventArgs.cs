using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    /// <summary>
    /// 游戏的普通消息，避免出现太多消息类导致key重复（虽然貌似不可能）
    /// </summary>
	public class GameNormalMessageEventArgs : GameEventArgs
	{
        public static int EventID { get { return GameFrameworkExtension.GetEventID(nameof(GameNormalMessageEventArgs)); } }
        public override int Id 
        { 
            get 
            {
                return EventID;
            } 
        }

        public GameNormalMsgCode MsgCode { get; private set; }
        public int IntValue { get; private set; }
        public float FloatValue { get; private set; }
        public bool BoolValue { get; private set; }
        public object ObjValue { get; private set; }

        public static GameNormalMessageEventArgs Create(GameNormalMsgCode msgCode, int intValue, float floatValue, bool boolValue, object objValue)
        {
            var result = ReferencePool.Acquire<GameNormalMessageEventArgs>();
            result.MsgCode = msgCode;
            result.IntValue = intValue;
            result.FloatValue = floatValue;
            result.BoolValue = boolValue;
            result.ObjValue = objValue;
            return result;
        }
        public override void Clear()
        {
            IntValue = 0;
            FloatValue = 0;
            BoolValue = false;
            ObjValue = null;
        }
    }
}