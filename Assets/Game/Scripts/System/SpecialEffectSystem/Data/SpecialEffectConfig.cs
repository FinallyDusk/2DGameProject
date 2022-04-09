using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 特效配置，注<see cref="Speed"/>与<see cref="Duration"/>请择一使用
	/// </summary>
	public class SpecialEffectConfig
	{
		[JsonProperty][ShowInInspector]
		[LabelText("特效名")]
		public string EffectName { get; private set; }
		[JsonProperty]
		[ShowInInspector]
		[LabelText("特效速度")]
		public float Speed { get; private set; }
		[JsonProperty]
		[ShowInInspector]
		[LabelText("持续时间")]
		public float Duration { get; private set; }
	}
}