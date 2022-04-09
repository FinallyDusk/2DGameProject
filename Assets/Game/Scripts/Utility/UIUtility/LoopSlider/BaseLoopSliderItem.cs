using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BoringWorld
{
    public class BaseLoopSliderItem : MonoBehaviour, ILoopSliderItem
    {
        protected RectTransform m_RT;
        private Vector2 widthAndHeight;

        public virtual Vector2 GetMoveableWidthAndHeight()
        {
            return widthAndHeight;
        }

        public virtual void Hide()
        {
            transform.SetActive(false);
        }

        public virtual void InitItem(object userData)
        {
            m_RT = GetComponent<RectTransform>();
            if (m_RT == null)
            {
                throw new System.NullReferenceException("此物体必须要有RectTransform");
            }
            else
            {
                widthAndHeight = m_RT.sizeDelta;
            }
        }

        public virtual void NextMove()
        {
            transform.SetAsLastSibling();
        }

        public virtual void PrevMove()
        {
            transform.SetAsFirstSibling();
        }

        public virtual void RefreshData(object data)
        {
            transform.SetActive(true);
        }
    }

}