using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BoringWorld.UITabGroup
{
    [RequireComponent(typeof(Image))]
    /// <summary>
    /// 基础选项卡物体
    /// </summary>
    public class BaseTabItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        /// <summary>
        /// 选项卡Image组件
        /// </summary>
        private Image m_TabIcon;
        public TabItemStatus Status { get; private set; }
        private System.Action<TabItemStatus, int> m_StatusChangeCallback;
        private float m_AnimDuration;
        private Coroutine m_ColorFadeCo;
#pragma warning disable 0649
        private int m_Index; 
#pragma warning restore 0649

        public void OnInit(int index, Color normalColor, float animDuration, System.Action<TabItemStatus, int> statusChangeCallback)
        {
            m_Index = index;
            m_TabIcon = GetComponent<Image>();
            m_TabIcon.color = normalColor;
            m_AnimDuration = animDuration;
            m_StatusChangeCallback = statusChangeCallback;
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        public void ChangeStatus(TabItemStatus status, Color color)
        {
            if (Status == status) return;
            Status = status;
            if (m_ColorFadeCo != null)
            {
                StopCoroutine(m_ColorFadeCo);
            }
            m_ColorFadeCo = StartCoroutine(IEIconColorFadeAnim(color));
        }

        private IEnumerator IEIconColorFadeAnim(Color color)
        {
            if (m_AnimDuration <= 0)
            {
                m_TabIcon.color = color;
                yield break;
            }
            float time = 0f;
            var sourceColor = m_TabIcon.color;
            var interval = new WaitForSeconds(0.02f);
            while (time <= m_AnimDuration)
            {
                yield return interval;
                time += 0.02f;
                m_TabIcon.color = Color.Lerp(sourceColor, color, time / m_AnimDuration);
            }
            m_TabIcon.color = color;
            m_ColorFadeCo = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            m_StatusChangeCallback?.Invoke(TabItemStatus.Press, m_Index);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_StatusChangeCallback?.Invoke(TabItemStatus.Hover, m_Index);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_StatusChangeCallback?.Invoke(TabItemStatus.Normal, m_Index);
        }

        private void OnDisable()
        {
            if (m_ColorFadeCo != null)
            {
                StopCoroutine(m_ColorFadeCo);
            }
        }
    } 

    /// <summary>
    /// 选项卡状态
    /// </summary>
    public enum TabItemStatus
    {
        /// <summary>
        /// 普通状态
        /// </summary>
        Normal,
        /// <summary>
        /// 悬停状态
        /// </summary>
        Hover,
        /// <summary>
        /// 按压状态
        /// </summary>
        Press
    }
}