using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.Combat
{
    /// <summary>
    /// buff效果参数
    /// </summary>
    public class BuffEffectArgs : IReference
    {
        public void Clear()
        {
            
        }

        public static BuffEffectArgs Create()
        {
            BuffEffectArgs result = ReferencePool.Acquire<BuffEffectArgs>();

            return result;
        }
    }
}