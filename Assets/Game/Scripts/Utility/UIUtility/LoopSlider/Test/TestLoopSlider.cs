using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BoringWorld
{
	public class TestLoopSlider : MonoBehaviour
	{
        public int maxCount;
        public LoopSlider ls;
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ls.InitLoopSlider(new TestDataSource(), maxCount);
            }
	    }
	}

    public class TestDataSource : ILoopSliderDataSource
    {
        public int GetDataMaxCount()
        {
            return 1;
        }

        public object GetFirstData()
        {
            return 0;
        }

        public object GetNextData(int nowCursor)
        {
            return ++nowCursor;
        }

        public object GetPrevData(int nowCursor)
        {
            return --nowCursor;
        }

       
    }

}