using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld.UI.Backpack
{
	public class BackpackFormLogic : UIFormLogic
	{
#pragma warning disable 0649
        private AllItemTypeListArea m_AllItemTypeListArea;
        private DetailItemInfoArea m_DetailInfoArea;
#pragma warning restore 0649

        private BackpackSystem m_BackpackSystem;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_AllItemTypeListArea = this.GetComponentWithName<AllItemTypeListArea>("AllItemTypeListArea");
            m_DetailInfoArea = this.GetComponentWithName<DetailItemInfoArea>("DetailItemInfoArea");
            m_AllItemTypeListArea.OnInit(this);
            m_DetailInfoArea.OnInit();
            m_BackpackSystem = GameEntry.Backpack;
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_DetailInfoArea.Clear();
            ChangeShowItemType(ItemType.Equip);
        }

        /// <summary>
        /// 改变显示的物品类型
        /// </summary>
        public void ChangeShowItemType(ItemType itemType)
        {
            m_BackpackSystem.ChangeShowItemType(itemType);
            m_DetailInfoArea.Clear();
            m_AllItemTypeListArea.RefreshShowItems(m_BackpackSystem);
        }

        /// <summary>
        /// 显示道具详细信息
        /// </summary>
        public void ShowItemDetailInfo(ItemDataRow data)
        {
            m_DetailInfoArea.ShowData(data);
        }

        //todo，测试用，记得删除
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (Input.GetKeyDown(KeyCode.Q))
            {
                for (int i = 1003; i < 1005; i++)
                {
                    var row = EquipItemDataRow.GenerateInstance(i);
                    m_BackpackSystem.AddItemToBag(ItemType.Equip, row, 1);
                }
            }
        }
    }
}