using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld
{
    /// <summary>
    /// 背包系统
    /// </summary>
    public class BackpackSystem : BaseSystem, ILoopSliderDataSource
    {
        /// <summary>
        /// 背包中所有物品
        /// </summary>
        private Dictionary<ItemType, List<BackpackItemData>> m_AllBagItems;
        private List<BackpackItemData> m_NowShowItems;
        private BackpackFindResult m_FindResults;


        public override void OnInit()
        {
            base.OnInit();
            m_AllBagItems = new Dictionary<ItemType, List<BackpackItemData>>();
            m_FindResults = new BackpackFindResult();
        }

        /// <summary>
        /// 改变显示的道具类型
        /// </summary>
        /// <param name="itemType"></param>
        public void ChangeShowItemType(ItemType itemType)
        {
            if (m_AllBagItems.TryGetValue(itemType, out var result) == false)
            {
                result = new List<BackpackItemData>();
                m_AllBagItems.Add(itemType, result);
            }
            m_NowShowItems = result;
        }

        public void AddItemToBag(ItemType itemType, int itemID, int count)
        {
            throw new System.NotImplementedException($"这个方法还未实现");
        }

        public ILoopSliderDataSource FindReplaceEquipItem(EquipType equipType)
        {
            m_FindResults.Clear();
            if (m_AllBagItems.ContainsKey(ItemType.Equip) == false)
            {
                return m_FindResults;
            }
            var allEquips = m_AllBagItems[ItemType.Equip];
            for (int i = 0; i < allEquips.Count; i++)
            {
                var equipData = allEquips[i].ItemData as EquipItemDataRow;
                if (equipData.EquipType == equipType)
                {
                    m_FindResults.AddResult(allEquips[i]);
                }
            }
            return m_FindResults;
        }

        /// <summary>
        /// 回收物品（将物品从背包中移除）
        /// </summary>
        /// <param name="obj"></param>
        public void RecyleItem(ItemType item, BackpackItemData obj)
        {
            m_AllBagItems[item].Remove(obj);
            obj.Release();
        }


        public void AddItemToBag(ItemType itemType, ItemDataRow itemData, int count)
        {
            if (m_AllBagItems.TryGetValue(itemType, out var list) == false)
            {
                list = new List<BackpackItemData>();
                m_AllBagItems.Add(itemType, list);
            }
            //检测道具是否可以堆叠
            if (itemData.HeapUp)
            {
                //检测列表中是否有相同的道具
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].ItemID == itemData.Id)
                    {
                        list[i].Count += count;
                        return;
                    }
                }
                //如果不存在
                BackpackItemData data = ReferencePool.Acquire<BackpackItemData>();
                data.Init(itemData);
                data.Count = count;
                list.Add(data);
                return;
            }
            else
            {
                //如果不可以堆叠
                BackpackItemData data = ReferencePool.Acquire<BackpackItemData>();
                data.Init(itemData);
                data.Count = count;
                list.Add(data);
                return;
            }
        }

        protected override void InternalPreLoadResources()
        {
            PreLoadFinsh();
        }

        public object GetFirstData()
        {
            return m_NowShowItems[0];
        }

        public object GetPrevData(int nowCursor)
        {
            return m_NowShowItems[nowCursor];
        }

        public object GetNextData(int nowCursor)
        {
            return m_NowShowItems[nowCursor];
        }

        public int GetDataMaxCount()
        {
            return m_NowShowItems.Count;
        }
    }

    public class BackpackFindResult : ILoopSliderDataSource
    {
        private List<BackpackItemData> m_Results;
        
        public BackpackFindResult()
        {
            m_Results = new List<BackpackItemData>();
        }

        public void AddResult(BackpackItemData data)
        {
            m_Results.Add(data);
        }

        public int GetDataMaxCount()
        {
            return m_Results.Count;
        }

        public object GetFirstData()
        {
            return m_Results[0];
        }

        public object GetNextData(int nowCursor)
        {
            return m_Results[nowCursor];
        }

        public object GetPrevData(int nowCursor)
        {
            return m_Results[nowCursor];
        }

        public void Clear()
        {
            m_Results.Clear();
        }
    }
}