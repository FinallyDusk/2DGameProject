using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace BoringWorld.UI.CombatForm
{
    public class SliderBar : MonoBehaviour
    {
        private RectTransform m_FadeOut;
        private RectTransform m_ValueSlider;
        private Image m_ValueColorGradient;

        private float m_FillValue;
#pragma warning disable 0649
        [SerializeField]
        private float m_AnimTime;
        [SerializeField]
        private float m_AnimTimeInterval;
        [SerializeField]
        private AnimationCurve m_AnimCurve;
        [SerializeField]
        private Gradient m_ColorGradient;
        [SerializeField]
        private bool m_EnableAnim;
        [SerializeField]
        private bool m_EnableGradient;
#pragma warning restore 0649

        private Coroutine m_Anim;
        public bool EnableAnim
        {
            set
            {
                if (m_EnableAnim == value) return;
                m_EnableAnim = value;
                if (m_EnableAnim == false)
                {
                    m_FadeOut.anchorMax = new Vector2(0, 1);
                }
                else
                {
                    m_FadeOut.anchorMax = m_ValueSlider.anchorMax;
                }
            }
        }
        public bool EnableGradient
        {
            set
            {
                if (m_EnableGradient == value) return;
                m_EnableGradient = value;
                if (m_EnableGradient)
                {
                    m_ValueColorGradient.color = m_ColorGradient.Evaluate(m_FillValue);
                }
                else
                {
                    m_ValueColorGradient.color = Color.white;
                }
            }
        }

        public void OnInit()
        {
            m_FadeOut = transform.Find("FadeOut").GetComponent<RectTransform>();
            m_ValueSlider = transform.Find("ValueSlider").GetComponent<RectTransform>();
            m_ValueColorGradient = m_ValueSlider.GetComponent<Image>();
            m_FillValue = 0;
        }

        public void OnOpen(float fillValue)
        {
            m_FillValue = fillValue;
            m_ValueSlider.anchorMax = new Vector2(m_FillValue, 1);
            if (m_EnableAnim)
            {
                m_FadeOut.anchorMax = m_ValueSlider.anchorMax;
            }
            else
            {
                m_FadeOut.anchorMax = new Vector2(0, 1);
            }
        }

        public void ValueChange(double nowValue, double maxValue)
        {
            float endValue = (float)(nowValue / maxValue);
            if (m_Anim != null)
            {
                StopCoroutine(m_Anim);
                m_FillValue = m_FadeOut.anchorMax.x;
            }
            m_Anim = StartCoroutine(ValueChangeAnim(endValue));
        }

        private IEnumerator ValueChangeAnim(float endValue)
        {
            m_ValueSlider.anchorMax = new Vector2(endValue, 1);
            if (m_EnableGradient)
            {
                m_ValueColorGradient.color = m_ColorGradient.Evaluate(endValue);
            }
            if (m_EnableAnim)
            {
                float time = 0;
                var waitSeconds = new WaitForSeconds(m_AnimTimeInterval);
                while (time <= m_AnimTime)
                {
                    yield return waitSeconds;
                    time += m_AnimTimeInterval;
                    var prob = m_AnimCurve.Evaluate(time / m_AnimTime);
                    m_FadeOut.anchorMax = new Vector2(m_FillValue - (m_FillValue - endValue) * prob, 1);
                }
                m_FadeOut.anchorMax = m_ValueSlider.anchorMax;
            }
            m_FillValue = endValue;
            m_Anim = null;
        }

    } 
}
