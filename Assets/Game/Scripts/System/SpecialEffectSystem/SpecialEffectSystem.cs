using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework.DataTable;
using GameFramework.Resource;

namespace BoringWorld
{
    /// <summary>
    /// 特效系统
    /// </summary>
    public class SpecialEffectSystem : BaseSystem
    {
        /// <summary>
        /// 默认特效名
        /// </summary>
        private const string DEFAULT_SPECIAL_EFFECT_NAME = "DefaultSpecialEffectName";
        private CombatSystem m_CombatSystem;
        /// <summary>
        /// 所有特效数据
        /// </summary>
        private IDataTable<SpecialEffectDataRow> m_AllData;
        /// <summary>
        /// 所有特效实体模板
        /// </summary>
        private Dictionary<string, ParticleSystem> m_AllSpecialEffectObjTeamplate;
        /// <summary>
        /// 所有空闲的特效对象
        /// </summary>
        private Dictionary<string, List<ParticleSystem>> m_AllFreeSpecialEffectObj;

        private Transform m_SpecialEffectParent;

        #region 预加载特效实体模板

        /// <summary>
        /// 加载个数
        /// </summary>
        private int m_PreLoadCount;

        #endregion
        #region 初始化

        public override void OnInit()
        {
            base.OnInit();
            m_AllSpecialEffectObjTeamplate = new Dictionary<string, ParticleSystem>();
            m_AllFreeSpecialEffectObj = new Dictionary<string, List<ParticleSystem>>();
            m_CombatSystem = GameEntry.Combat;
        }

        protected override void InternalPreLoadResources()
        {
            m_SpecialEffectParent = GameObject.FindGameObjectWithTag(Tags.SPECIAL_EFFECT_PARENT).transform;
            m_AllData = GameEntry.DataTable.GetDataTable<SpecialEffectDataRow>(DataTableName.SPECIAL_EFFECT_DATA_NAME);
            //加载所有特效实体模板
            m_PreLoadCount = m_AllData.Count;
            var loadAssetCallback = new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback);
            foreach (var item in m_AllData)
            {
                GameEntry.Resource.LoadAsset(item.AssetPath, loadAssetCallback, item);
            }
        }

        private void LoadAssetSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            var data = userData as SpecialEffectDataRow;
            var obj = (asset as GameObject)?.GetComponent<ParticleSystem>();
            if (obj == null)
            {
                Log.Error($"<color=#ff0000>资源加载出错, assetName = {assetName}, asset.type = {asset.GetType()}</color>");
            }
            else
            {
                obj.name = data.AssetName;
                m_AllSpecialEffectObjTeamplate[data.AssetName] = obj;
                m_AllFreeSpecialEffectObj[data.AssetName] = new List<ParticleSystem>();
            }
            m_PreLoadCount--;
            if (m_PreLoadCount == 0)
            {
                PreLoadFinsh();
                //设置默认特效
                if (m_AllSpecialEffectObjTeamplate.ContainsKey(DEFAULT_SPECIAL_EFFECT_NAME) == false)
                {
                    Log.Error($"请设置默认特效");
                }
            }
        }

        private void LoadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Error($"<color=#ff0000>加载特效实体出错，assetName = {assetName}, LoadResourceStatus = {status}, errorMessage = {errorMessage}</color>");
        }
        #endregion

        public float PlaySpecialEffect(string effectName, double speed, Vector3Int startPoint, Vector3Int endPoint)
        {
            return PlaySpecialEffect(effectName, (float)speed, (Vector3)startPoint, (Vector3)endPoint);
        }

        /// <summary>
        /// 播放特效(返回特效结束时间)
        /// </summary>
        /// <param name="configID">特效配置ID</param>
        /// <param name="startPoint">特效起始位置</param>
        /// <param name="endPoint">结束位置</param>
        public float PlaySpecialEffect(string effectName, float speed, Vector3 startPoint, Vector3 endPoint)
        {
            float duration = 0f;
            var distance = Vector3.Distance(endPoint, startPoint);
            if (speed != 0)
            {
                duration = distance / speed;
            }
            var effectObj = GetSpecialEffect(effectName);
            //此处需要修改
            //todo
            GameEntry.Base.StartCoroutine(IESpecialEffectMoveAnim(effectObj, startPoint, endPoint, duration));
            return duration;
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="effectName"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public void PlaySpecialEffect(string effectName, Vector3Int pos, double duration)
        {
            var effectObj = GetSpecialEffect(effectName);
            effectObj.transform.localPosition = pos;
            GameEntry.Base.StartCoroutine(IESpecialEffectAnim(effectObj, (float)duration));
        }

        /// <summary>
        /// 获得特效
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ParticleSystem GetSpecialEffect(string name)
        {
            if (m_AllFreeSpecialEffectObj.TryGetValue(name, out var list))
            {
                if (list.Count > 0)
                {
                    var result = list[0];
                    list.RemoveAt(0);
                    return result;
                }
                else
                {
                    return GenerateSpecialEffectObj(name);
                }
            }
            Log.Warning($"不存在特效:{name}");
            //加载默认特效
            if (DEFAULT_SPECIAL_EFFECT_NAME == name)
            {
                Log.Error($"请设置默认特效");
                return null;
            }
            return GetSpecialEffect(DEFAULT_SPECIAL_EFFECT_NAME);
        }

        public void RecyleSpecialEffect(ParticleSystem obj)
        {
            RecyleSpecialEffect(obj.name, obj);
        }

        public void RecyleSpecialEffect(string name, ParticleSystem obj)
        {
            if (m_AllFreeSpecialEffectObj.TryGetValue(name, out var list))
            {
                list.Add(obj);
            }
            else
            {
                m_AllFreeSpecialEffectObj[name] = new List<ParticleSystem>();
                m_AllFreeSpecialEffectObj[name].Add(obj);
            }
            obj.transform.localPosition = new Vector3(9999, 9999, 9999);
            obj.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        /// <summary>
        /// 生成特效实体
        /// </summary>
        /// <returns></returns>
        private ParticleSystem GenerateSpecialEffectObj(string name)
        {
            if (m_AllSpecialEffectObjTeamplate.TryGetValue(name, out var template))
            {
                var result = Object.Instantiate(template, m_SpecialEffectParent);
                result.name = template.name;
                return result;
            }
            return GetSpecialEffect(DEFAULT_SPECIAL_EFFECT_NAME);
        }

        /// <summary>
        /// 特效动画移动动画
        /// </summary>
        /// <returns></returns>
        private IEnumerator IESpecialEffectMoveAnim(ParticleSystem effectObj, Vector3 startPoint, Vector3 endPoint, float duration)
        {
            //进行旋转，此处默认所有特效一致向右
            var rotate = effectObj.transform.rotation;
            effectObj.transform.localPosition = startPoint;
            effectObj.transform.LookAt(endPoint);
            effectObj.Play();
            float time = 0f;
            var interval = new WaitForSeconds(0.02f);
            while (time < duration)
            {
                effectObj.transform.localPosition = Vector3.Lerp(startPoint, endPoint, time / duration);
                yield return interval;
                //受战斗中的时间影响
                float timeScale = 1;
                if (m_CombatSystem.InCombat)
                {
                    timeScale = m_CombatSystem.TimeScale;
                }
                time += 0.02f * timeScale;
                if (timeScale == 0)
                {
                    if (effectObj.isPaused == false)
                    {
                        effectObj.Pause();
                    }
                }
                else
                {
                    if (effectObj.isPlaying == false && effectObj.isStopped == false)
                    {
                        effectObj.Play();
                    }
                }
            }
            effectObj.transform.localPosition = endPoint;
            yield return interval;
            RecyleSpecialEffect(effectObj.name, effectObj);
            effectObj.transform.rotation = rotate;
        }

        private IEnumerator IESpecialEffectAnim(ParticleSystem effectObj, float duration)
        {
            float time = 0;
            var interval = new WaitForSeconds(0.02f);
            effectObj.Play();
            while (time < duration)
            {
                yield return interval;
                //受战斗中的时间影响
                float timeScale = 1;
                if (m_CombatSystem.InCombat)
                {
                    timeScale = m_CombatSystem.TimeScale;
                }
                time += 0.02f * timeScale;
                if (timeScale == 0)
                {
                    if (effectObj.isPaused == false)
                    {
                        effectObj.Pause();
                    }
                }
                else
                {
                    if (effectObj.isPlaying == false && effectObj.isStopped == false)
                    {
                        effectObj.Play();
                    }
                }
            }
            RecyleSpecialEffect(effectObj);
        }
    }
}