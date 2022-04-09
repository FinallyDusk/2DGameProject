using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStarUtil;

namespace BoringWorld
{
	/// <summary>
	/// 工具平台
	/// </summary>
	public static class Util
	{
		private static AStar m_AStar;
		public static AStar AStar
        {
            get
            {
				if (m_AStar == null)
                {
					m_AStar = new AStar();
					m_AStar.Init();
                }
				return m_AStar;
            }
        }
	}
}