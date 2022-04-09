using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	/// <summary>
	/// 单位属性数据
	/// </summary>
	public class UnitBasePropDataRow : DataRowBase
	{
		[JsonIgnore]
        public override int Id { get { return id; } }

		[JsonProperty][ShowInInspector][PropertyOrder(1)]
		private int id;
		/// <summary>
		/// 详细属性
		/// </summary>
		[JsonProperty][HideInInspector]
		public double[] UnitAllBaseProp { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
			var row = JsonTool.LoadJson<UnitBasePropDataRow>(dataRowString);
			id = row.id;
			UnitAllBaseProp = row.UnitAllBaseProp;
			return true;
        }

#if UNITY_EDITOR
		[OnInspectorInit("OnDisplayUnitAllBasePropInit")][JsonIgnore][OnCollectionChanged(After = "OnDisplayUnitAllBasePropValueChanged")]
		[PropertyOrder(2)][LabelText("所有属性")]
		public Dictionary<UnitPropType, double> DisplayUnitAllBaseProp;

		private void OnDisplayUnitAllBasePropInit()
        {
			if (DisplayUnitAllBaseProp != null) return;
			if (UnitAllBaseProp == null)
            {
				var values = System.Enum.GetValues(typeof(UnitPropType));
				DisplayUnitAllBaseProp = new Dictionary<UnitPropType, double>(values.Length);
				UnitAllBaseProp = new double[values.Length];
				for (int i = 0; i < values.Length; i++)
                {
					DisplayUnitAllBaseProp.Add((UnitPropType)i, 0);
					UnitAllBaseProp[i] = 0;
				}
            }
            else
            {
				DisplayUnitAllBaseProp = new Dictionary<UnitPropType, double>(UnitAllBaseProp.Length);
				for (int i = 0; i < UnitAllBaseProp.Length; i++)
				{
					DisplayUnitAllBaseProp.Add((UnitPropType)i, UnitAllBaseProp[i]);
				}
			}
        }

		private void OnDisplayUnitAllBasePropValueChanged()
        {
            for (int i = 0; i < UnitAllBaseProp.Length; i++)
            {
				UnitAllBaseProp[i] = DisplayUnitAllBaseProp[(UnitPropType)i];
            }
        }


#endif
	}
}