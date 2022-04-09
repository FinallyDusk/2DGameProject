using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    [XLua.LuaCallCSharp]
    public delegate void CombatEffectAction(Combat.CombatEffectData arg);
}