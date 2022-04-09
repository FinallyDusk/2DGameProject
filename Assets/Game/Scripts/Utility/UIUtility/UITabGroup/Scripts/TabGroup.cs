using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.UITabGroup
{
    /// <summary>
    /// 选项卡组组件
    /// </summary>
    public class TabGroup : MonoBehaviour
    {
#pragma warning disable 0649
        /// <summary>
        /// 普通状态颜色
        /// </summary>
        [SerializeField] private Color m_Normal;
        /// <summary>
        /// 悬停时的颜色
        /// </summary>
        [SerializeField] private Color m_Hover;
        /// <summary>
        /// 点击后的颜色
        /// </summary>
        [SerializeField] private Color m_Pressed;
        /// <summary>
        /// 动画持续时间
        /// </summary>
        [SerializeField] private float m_AnimDuration = 0.3f;
        [SerializeField] private BaseTabItem[] m_AllTabItem;
        private ITabContent[] m_AllTabContent;
#pragma warning restore 0649
        private bool m_Error;

        public void OnInit(object userData)
        {
            m_Error = false;
            m_AllTabContent = GetComponentsInChildren<ITabContent>(true);
            if ((m_AllTabContent == null && m_AllTabItem != null) ||
                (m_AllTabContent != null && m_AllTabItem == null) ||
                (m_AllTabItem.Length != m_AllTabContent.Length))
            {
                Debug.LogError("选项卡和内容数量不一致，请检查");
                m_Error = true;
                return;
            }
            if (m_AllTabContent == null && m_AllTabItem == null)
            {
                Debug.LogError("选项卡和内容都为空，选项卡组不起作用");
                m_Error = true;
                return;
            }
            for (int i = 0; i < m_AllTabItem.Length; i++)
            {
                m_AllTabItem[i].OnInit(i, m_Normal, m_AnimDuration, ItemStatusChange);
            }
            foreach (var item in m_AllTabContent)
            {
                item.OnInit(userData);
            }
            ItemStatusChange(TabItemStatus.Press, 0);
            for (int i = 1; i < m_AllTabContent.Length; i++)
            {
                m_AllTabContent[i].OnClose();
            }
        }

        public void OnUpdateContent(object[] userDatas)
        {
            if (m_Error)
            {
                Debug.LogError("选项卡组出错了，不能使用");
                return;
            }
            for (int i = 0; i < m_AllTabContent.Length; i++)
            {
                m_AllTabContent[i].OnUpdateData(userDatas?[i]);
            }
        }

        private void ItemStatusChange(TabItemStatus status, int index)
        {
            if (m_Error)
            {
                Debug.LogError("选项卡组出错了，不能使用");
                return;
            }
            var item = m_AllTabItem[index];
            switch (status)
            {
                case TabItemStatus.Normal:
                    if (item.Status == TabItemStatus.Press) return;
                    item.ChangeStatus(status, m_Normal);
                    break;
                case TabItemStatus.Hover:
                    if (item.Status == TabItemStatus.Press) return;
                    item.ChangeStatus(status, m_Hover);
                    break;
                case TabItemStatus.Press:
                    //如果其他物体有按压状态，改为普通状态
                    for (int i = 0; i < m_AllTabItem.Length; i++)
                    {
                        var tab = m_AllTabItem[i];
                        if (tab.Status == TabItemStatus.Press && tab != item)
                        {
                            tab.ChangeStatus(TabItemStatus.Normal, m_Normal);
                            m_AllTabContent[i].OnClose();
                        }
                    }
                    if (item.Status == status) return;
                    item.ChangeStatus(status, m_Pressed);
                    m_AllTabContent[index].OnOpen();
                    break;
                default:
                    throw new System.NotImplementedException($"还未实现Status = {status}的操作");
            }
        }
    } 
}