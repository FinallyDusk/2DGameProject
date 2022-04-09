using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Resource;
using UnityEngine.U2D;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    public class SpriteSystem : BaseSystem
    {
        private Dictionary<SpriteType, List<SpriteAtlas>> m_AllSprites;
        private int m_PreLoadCount;


        public Sprite GetSprite(string spriteName, SpriteType type)
        {
            if (m_AllSprites.TryGetValue(type, out var list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var result = list[i].GetSprite(spriteName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            Log.Error($"查找不到类型为<color=#00ff00>{type}</color>，图片名为<color=#0000ff>{spriteName}</color>的图片，请检查");
            return null;
        }

        public override void OnEnter()
        {
            m_AllSprites.Clear();
        }

        public override void OnInit()
        {
            m_AllSprites = new Dictionary<SpriteType, List<SpriteAtlas>>();
        }

        protected override void InternalPreLoadResources()
        {
            //加载图集
            //此处未加载默认图片
            m_PreLoadCount = 1;
            LoadAssetCallbacks callbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback);
            GameEntry.Resource.LoadAsset("Assets/Game/GameResource/Sprites/UnitPaint/UnitPaint.spriteatlas", callbacks, new SpriteLoadData(this, SpriteType.UnitPaint));
            GameEntry.Resource.LoadAsset("Assets/Game/GameResource/Sprites/CombatActionIcon/CombatActionIcon.spriteatlas", callbacks, new SpriteLoadData(this, SpriteType.CombatActionIcon));
            GameEntry.Resource.LoadAsset("Assets/Game/GameResource/Sprites/BuffIcon/BuffAtlas.spriteatlas", callbacks, new SpriteLoadData(this, SpriteType.Buff));
        }

        private void LoadAssetSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            SpriteAtlas sa = asset as SpriteAtlas;
            if (sa == null)
            {
                Log.Error("加载图片错误");
                return;
            }
            SpriteLoadData t = userData as SpriteLoadData;
            if (t == null || t.UserData != this)
            {
                return;
            }
            if (m_AllSprites.TryGetValue(t.SpriteType, out var list) == false)
            {
                m_AllSprites.Add(t.SpriteType, new List<SpriteAtlas>());
            }
            m_AllSprites[t.SpriteType].Add(sa);
            m_PreLoadCount--;
            if (m_PreLoadCount == 0)
            {
                PreLoadFinsh();
            }
        }

        private void LoadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Error($"assetName = <color=#ff0000>{assetName}</color>, status = <color=#ff0000>{status}</color> errorMessage = <color=#ff0000>{errorMessage}</color>");
        }

        private class SpriteLoadData
        {
            public object UserData;
            public SpriteType SpriteType;
            public SpriteLoadData(object userData, SpriteType st)
            {
                UserData = userData;
                SpriteType = st;
            }
        }
    }
}