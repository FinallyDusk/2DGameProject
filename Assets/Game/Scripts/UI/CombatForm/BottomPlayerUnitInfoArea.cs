using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.UI.CombatForm
{
	/// <summary>
	/// 底部玩家单位详细信息
	/// </summary>
	public class BottomPlayerUnitInfoArea : MonoBehaviour
	{
		private PlayerUnitItem[] m_AllUnitItems;

		public void OnInit()
        {
			m_AllUnitItems = transform.GetComponentsInChildren<PlayerUnitItem>();

            for (int i = 0; i < m_AllUnitItems.Length; i++)
            {
				m_AllUnitItems[i].OnInit();
            }
        }

		public void OnResetAllItem()
        {
            for (int i = 0; i < m_AllUnitItems.Length; i++)
            {
                m_AllUnitItems[i].OnClose(null);
            }
        }

        /// <summary>
        /// 绑定显示单位
        /// </summary>
        public void BindDisplayUnit(CombatUnitInfo unit)
        {
            for (int i = 0; i < m_AllUnitItems.Length; i++)
            {
                if (m_AllUnitItems[i].Active == false)
                {
                    m_AllUnitItems[i].OnOpen(unit);
                    return;
                }
            }
        }
	}
}