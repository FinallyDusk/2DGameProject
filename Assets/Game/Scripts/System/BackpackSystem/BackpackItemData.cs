using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 背包物品数据
	/// </summary>
	public class BackpackItemData : IReference
	{
		public int ItemID { get; private set; }
		public ItemDataRow ItemData { get; private set; }
		public bool HeapUp { get; private set; }
		public int Count;

		public void Init(ItemDataRow baseData)
        {
			ItemID = baseData.Id;
			ItemData = baseData;
			HeapUp = ItemData.HeapUp;
			Count = 0;
        }

		public void Clear()
        {
			ItemID = -1;
			ItemData = null;
			HeapUp = false;
			Count = 0;
        }
	}
}