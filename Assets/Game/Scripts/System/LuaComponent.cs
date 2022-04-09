using BoringWorld;
using BoringWorld.Combat;
using GameFramework;
using GameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;
using XLua;


namespace BoringWorld
{
    public class LuaComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 全局唯一Lua虚拟环境
        /// </summary>
        private static LuaEnv m_LuaEnv;
        public LuaEnv Env { get { return m_LuaEnv; } }
        private ResourceComponent m_ResourceComponent;
        private EventComponent m_EventComponent;
        private Dictionary<string, string> m_CacheLuaDict;
        private List<string> m_AllLoadLuaFiles;

        private int m_LoadLuaCount;
        private System.Action m_LoadFinshCallback;

        protected override void Awake()
        {
            base.Awake();

        }

        private void Start()
        {
            m_ResourceComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
            m_EventComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
            if (m_ResourceComponent == null)
            {
                Log.Error(" m_ResourceComponent is null.");
                return;
            }

            m_CacheLuaDict = new Dictionary<string, string>();
            m_LuaEnv = new LuaEnv();
            m_LuaEnv.AddLoader((ref string filename) =>
            {
                byte[] result = null;
                if (m_CacheLuaDict.TryGetValue(filename, out string content))
                {
                    result = System.Text.Encoding.UTF8.GetBytes(content);
                }
                return result;
            });
            m_AllLoadLuaFiles = new List<string>();
        }

        private void Update()
        {
            if (m_LuaEnv != null)
            {
                m_LuaEnv.Tick();
            }
        }

        private void OnDestroy()
        {
            m_CacheLuaDict.Clear();
            m_AllLoadLuaFiles.Clear();

            if (m_LuaEnv != null)
            {
                m_LuaEnv.Dispose();
                m_LuaEnv = null;
            }
        }

        /// <summary>
        /// 加载lua文件列表
        /// </summary>
        public void LoadLuaFilesConfig(System.Action callback)
        {
            m_LoadFinshCallback = callback;
            LoadAssetCallbacks callBacks = new LoadAssetCallbacks(OnLoadLuaFilesConfigSuccess, OnLoadLuaFilesConfigFailure);
            string assetName = "Assets/Game/GameResource/Config/LuaConfig.txt";
            m_ResourceComponent.LoadAsset(assetName, callBacks);
        }

        /// <summary>
        /// 解析Lua文件配置列表
        /// </summary>
        /// <param name="content">配置列表内容</param>
        public void ParseLuaFilesConfig(string content)
        {
            string[] contentLines = content.Split('\n');
            int len = contentLines.Length;
            m_LoadLuaCount = 0;
            for (int i = 0; i < len; i++)
            {
                if (!string.IsNullOrEmpty(contentLines[i]) && contentLines[i][0] != '#')
                {
                    m_LoadLuaCount++;
                    string[] args = contentLines[i].Split('\t');
                    LoadAssetCallbacks callBacks = new LoadAssetCallbacks(OnLoadLuaAssetSuccess, OnLoadLuaAssetFailure);
                    string assetName = args[2];
                    m_ResourceComponent.LoadAsset(assetName, callBacks, args[1]);
                }
            }

        }

        /// <summary>
        /// 执行战斗效果方法
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="arg"></param>
        public void ExecuteCombatEffectAction(string functionName, CombatEffectData arg)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                Log.Warning("方法为空，请检查");
                arg?.Callback?.Invoke();
                return;
            }
            //return;
            lock (m_LuaEnv)
            {
                CombatEffectAction action = m_LuaEnv.Global.GetInPath<CombatEffectAction>(functionName);
                if (action == null)
                {
                    Log.Error($"查找不到脚本文件{functionName}");
                    return;
                }
                action.Invoke(arg);
            }
        }

        private void OnLoadLuaFilesConfigFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Info("Load LuaFilesConfig: '{0}' failure.", assetName);
        }

        private void OnLoadLuaFilesConfigSuccess(string assetName, object asset, float duration, object userData)
        {
            TextAsset textAsset = (TextAsset)asset;
            Log.Info("Load LuaFilesConfig: '{0}' success.", assetName);

            string content = textAsset.text;
            //开始解析Lua配置文件列表
            ParseLuaFilesConfig(content);
        }

        private void OnLoadLuaAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            string luaName = (string)userData;

            if (m_CacheLuaDict.ContainsKey(luaName))
            {
                Log.Warning("CacheLuaDict has exist lua file '{0}'.", luaName);
                return;
            }

            TextAsset textAsset = (TextAsset)asset;
            m_CacheLuaDict.Add(luaName, textAsset.text);

            Log.Info("Load lua '{0}' success.", luaName);
            m_LoadLuaCount--;
            if (m_LoadLuaCount <= 0)
            {
                //加载全部的字符串
                foreach (var item in m_CacheLuaDict)
                {
                    m_LuaEnv.DoString(item.Value);
                }
                m_LoadFinshCallback?.Invoke();
            }
        }

        private void OnLoadLuaAssetFailure(string assetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            string luaName = (string)userData;
            string errorMessage = string.Format("Load lua file failed. The file is {0}. ", assetName);
        }
    } 
}