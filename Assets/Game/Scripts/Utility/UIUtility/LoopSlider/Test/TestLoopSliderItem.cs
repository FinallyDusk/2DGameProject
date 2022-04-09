using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace BoringWorld
{
	public class TestLoopSliderItem : BaseLoopSliderItem
	{
        public TextMeshProUGUI text;
        
        public override void RefreshData(object data)
        {
            base.RefreshData(data);
            text.text = data.ToString();
        }
	}
	
}