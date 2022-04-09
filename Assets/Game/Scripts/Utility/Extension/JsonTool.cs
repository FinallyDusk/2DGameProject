using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoringWorld
{
	public class JsonTool
	{
	    /// <summary>
	    /// 序列化设置
	    /// </summary>
	    protected readonly static JsonSerializerSettings m_Settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
	    

		public static T LoadJson<T>(string content, params JsonConverter[] converters)
        {
			return JsonConvert.DeserializeObject<T>(content,/* m_Settings,*/ converters);
        }
	}
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        protected abstract T Create(Type objectType, JObject jsonObject);
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var target = Create(objectType, jsonObject);
            serializer.Populate(jsonObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    //public class JsonSkillEffectConverter : JsonCreationConverter<SkillEffectData>
    //{
    //    protected override SkillEffectData Create(Type objectType, JObject jsonObject)
    //    {
    //        var typeName = jsonObject["EffectType"].ToString();
    //        switch (typeName)
    //        {
    //            case "0"://位移
    //                return new SkillEffect_DisplacementData();
    //            case "1"://伤害
    //                return new SkillEffect_TakeDamageData();
    //            default:
    //                throw new System.NotImplementedException($"还未制作EffectType = {typeName}的解析");
    //        }
    //    }
    //}

    //public class JsonSkillEffectConditionConverter : JsonCreationConverter<SkillEffectTriggerConditionData>
    //{
    //    protected override SkillEffectTriggerConditionData Create(Type objectType, JObject jsonObject)
    //    {
    //        var typeName = jsonObject["ConditionType"].ToString();
    //        switch (typeName)
    //        {
    //            case "0"://计时器
    //                return new SkillEffect_TimerConditionData();
    //            default:
    //                throw new System.NotImplementedException($"还未制作ConditionType = {typeName}的解析");
    //        }
    //    }
    //}
}