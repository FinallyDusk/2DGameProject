using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BoringWorld
{
	public interface ILoopSliderDataSource
	{
        object GetFirstData();
        object GetPrevData(int nowCursor);
        object GetNextData(int nowCursor);

		int GetDataMaxCount();
	}
	
}