using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BoringWorld
{
	/// <summary>
	/// 漂浮数字
	/// </summary>
	public class FloatNum : MonoBehaviour
	{
		/// <summary>
		/// 持续时间
		/// </summary>
		[Min(0.1f)]
		public float Duration;
		/// <summary>
		/// 缩放倍数
		/// </summary>
		public Vector3 ScaleTarget;
		/// <summary>
		/// 中间点高度范围
		/// </summary>
		public Vector2 MidPointHeight;
		/// <summary>
		/// 中间点宽度范围
		/// </summary>
		public Vector2 MidPointWidth;
		/// <summary>
		/// 移动目标宽度（在MidPointWidth基础上）
		/// </summary>
		public Vector2 MoveTargetWidth;
		/// <summary>
		/// 移动目标高度（在MidPointHeight下面）
		/// </summary>
		public Vector2 MoveTargetHeight;
		/// <summary>
		/// 缩放动画曲线
		/// </summary>
		public AnimationCurve ScaleAnimCurve;
		/// <summary>
		/// 移动动画曲线
		/// </summary>
		public AnimationCurve MoveAnimCurve;

		private TextMeshProUGUI m_ShowText;
#pragma warning disable 0414
        private float m_OriScale;
#pragma warning restore 0414
        private Vector3 m_HidePos;
		private FloatNumArea m_Parent;
		private RectTransform m_RT;

        public void OnInit(FloatNumArea parent)
		{
			m_OriScale = 1;
			m_ShowText = GetComponent<TextMeshProUGUI>();
			m_HidePos = new Vector3(999, 999, 999);
			m_Parent = parent;
			m_RT = GetComponent<RectTransform>();
		}

        public void ShowText(string content, Vector3 startPos)
        {
			m_ShowText.text = content;
			StopAllCoroutines();
			//gameObject.SetActive(true);
			Vector3 middlePoint = new Vector3(GetValue(MidPointWidth), GetValue(MidPointHeight), 0);
			Vector3 moveTarget = Vector3.zero;
			if (middlePoint.x > 0)
            {
				moveTarget = new Vector3(middlePoint.x + GetValue(MoveTargetWidth), middlePoint.y - GetValue(MoveTargetHeight), 0);
            }
            else
            {
				moveTarget = new Vector3(middlePoint.x - GetValue(MoveTargetWidth), middlePoint.y - GetValue(MoveTargetHeight), 0);
			}
			//坐标转换为UI坐标系
			startPos = Camera.main.WorldToScreenPoint(startPos);
			StartCoroutine(TextAnim(startPos, startPos + middlePoint, startPos + moveTarget));
        }

		private IEnumerator TextAnim(Vector3 start, Vector3 mid, Vector3 end)
        {
			m_RT.position = start;
			transform.localScale = Vector3.one;
			yield return 0;
			float time = 0f;
			while (time <= Duration)
            {
				yield return new WaitForSeconds(0.02f);
				time += 0.02f;
				float prob = time / Duration;
				var scaleValue = ScaleAnimCurve.Evaluate(prob);
				transform.localScale = ScaleTarget * scaleValue;
				var moveValue = MoveAnimCurve.Evaluate(prob);
				m_RT.position = CalculateCubicBezierPoint(moveValue, start, mid, end);
            }
			transform.localScale = Vector3.one;
			m_RT.anchoredPosition = m_HidePos;
			m_Parent.RecyleItem(this);
        }

		private float GetValue(Vector2 range)
        {
			return Random.Range(range.x, range.y);
        }


		/// <summary>
		/// 根据T值，计算贝塞尔曲线上面相对应的点
		/// </summary>
		/// <param name="t"></param>T值
		/// <param name="p0"></param>起始点
		/// <param name="p1"></param>控制点
		/// <param name="p2"></param>目标点
		/// <returns></returns>根据T值计算出来的贝赛尔曲线点
		private static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
		{
			float u = 1 - t;
			float tt = t * t;
			float uu = u * u;

			Vector3 p = uu * p0;
			p += 2 * u * t * p1;
			p += tt * p2;

			return p;
		}
	}
}