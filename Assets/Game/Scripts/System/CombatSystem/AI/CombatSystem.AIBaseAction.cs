using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    /// <summary>
    /// ai的基础行为
    /// </summary>
    public abstract class AIBaseAction : IReference
    {

        /// <summary>
        /// 进入单位的回合
        /// </summary>
        public abstract void EnterTurn(CombatUnitInfo unitInfo);

        public abstract void Clear();

        public static AIBaseAction Create<T>(object data) where T : AIBaseAction
        {
            AIBaseAction result = null;
            //此处做的比较粗糙
            if (typeof(T) == typeof(PlayerAI))
            {
                result = ReferencePool.Acquire<PlayerAI>();
            }
            else if (typeof(T) == typeof(EnemyAI))
            {
                result = ReferencePool.Acquire<EnemyAI>();
            }

            return result;
        }
    }
}