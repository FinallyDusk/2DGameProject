using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    public class PlayerAI : AIBaseAction
    {
        public override void Clear()
        {

        }

        public override void EnterTurn(CombatUnitInfo unitInfo)
        {
            GameEntry.Combat.ShowActionGroup();
        }
    }
}