using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    public class GameTimerComponent : GameFrameworkComponent
    {
        private GameTimerSystem m_TimerSystem;
        public void SetGameTimerSystem(GameTimerSystem timerSystem)
        {
            m_TimerSystem = timerSystem;
        }

        private void Update()
        {
            m_TimerSystem?.OnUpdate(UnityEngine.Time.deltaTime);
        }

    }
}