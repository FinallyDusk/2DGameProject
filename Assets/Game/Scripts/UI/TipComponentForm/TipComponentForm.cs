using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	public class TipComponentForm : UIFormLogic
	{
        private Vector2 m_CanvasSizeDelta;
        private RectTransform m_TipParent;
        private CanvasGroup m_TipParentGroup;
        private RectTransform m_NormalTip;
        private RectTransform m_CompareTip;
        private TextMeshProUGUI m_NormalContent;
        private TextMeshProUGUI m_CompareContent;

        private int m_PositionDirty;
        private Rect m_OldRect;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            GameEntry.UI.SetUIFormInstanceLocked(UIForm, true);

            m_TipParent = this.GetComponentWithName<RectTransform>("TipParent");
            m_TipParentGroup = m_TipParent.GetComponent<CanvasGroup>();
            m_NormalTip = m_TipParent.GetComponentWithName<RectTransform>("NormalTip");
            m_CompareTip = m_TipParent.GetComponentWithName<RectTransform>("CompareTip");
            m_NormalContent = m_NormalTip.GetComponentWithName<TextMeshProUGUI>("Content");
            m_CompareContent = m_CompareTip.GetComponentWithName<TextMeshProUGUI>("Content");

            //TODO此处需要修改
            m_CanvasSizeDelta = transform.parent.parent.GetComponent<RectTransform>().sizeDelta;

            GameEntry.Event.Subscribe(SendTipMsgEventArgs.EventId, SendTipMsgCallback);
            GameEntry.Event.Subscribe(CloseTipMsgEventArgs.EventId, CloseTipMsgCallback);
            m_TipParent.SetActive(false);
            m_PositionDirty = 0;
            m_OldRect = Rect.zero;
        }

        private void LateUpdate()
        {
            if (m_PositionDirty == 2)
            {
                ChangePosition();
                m_PositionDirty = 0;
            }
            if (m_PositionDirty == 1)
            {

                m_PositionDirty = 2;
            }
        }

        private void SendTipMsgCallback(object sender, System.EventArgs e)
        {
            m_TipParent.SetActive(true);
            var se = e as SendTipMsgEventArgs;
            m_NormalContent.text = se.NormalMsg;
            if (string.IsNullOrEmpty(se.CompareMsg))
            {
                m_CompareTip.SetActive(false);
            }
            else
            {
                m_CompareTip.SetActive(true);
                m_CompareContent.text = se.CompareMsg;
            }
            m_PositionDirty = 1;
            m_OldRect = se.Size;
            m_OldRect.position = UnityUtility.CalcLocalPositionForCanvas(m_CanvasSizeDelta, m_OldRect.position);
            m_TipParentGroup.alpha = 0;
        }

        private void ChangePosition()
        {
            //初始坐标以左下角为准,注：此处的提示框pivot和锚点都必须为0,0
            float t_ChangeX = m_OldRect.x + m_OldRect.width / 2f;
            float t_ChangeY = m_OldRect.y - m_OldRect.height / 2f;
            //判断鼠标左右
            if (Input.mousePosition.x > Screen.width / 2f)
            {
                //鼠标在屏幕右半边
                t_ChangeX = m_OldRect.x - m_OldRect.width / 2f - m_TipParent.sizeDelta.x;
            }
            //判断鼠标上下
            if (Input.mousePosition.y > Screen.height / 2f)
            {
                //鼠标在屏幕上半边
                t_ChangeY = m_OldRect.y + m_OldRect.height / 2f - m_TipParent.sizeDelta.y;
            }

            m_TipParent.localPosition = new Vector3(t_ChangeX, t_ChangeY, 0);
            m_TipParentGroup.alpha = 1;
        }

        private void CloseTipMsgCallback(object sender, System.EventArgs e)
        {
            m_TipParent.SetActive(false);
        }
    }
}