using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoringWorld.UI.CombatForm
{
	/// <summary>
	/// 行动条上的物体
	/// </summary>
	public class ActionItem : MonoBehaviour
	{
		private Image m_UnitIcon;
		private RectTransform m_RectTransform;
		private float m_OldValue;

		private float m_AnimInterval = 0.02f;
		private float m_AnimTime = 0.2f;

		public void OnInit()
        {
			m_UnitIcon = transform.Find("UnitIcon").GetComponent<Image>();
			m_RectTransform = GetComponent<RectTransform>();
        }

		public void OnOpen(CombatUnitInfo unit)
        {
			gameObject.SetActive(true);
			m_RectTransform.anchoredPosition = Vector2.zero;
			ChangeActionProgress(0f);
			m_UnitIcon.sprite = GameEntry.Sprite.GetSprite(unit.BaseData.CombatActionIcon, SpriteType.CombatActionIcon);
			GameEntry.Combat.RegisterActionProgressEvent(unit.InstanceID, ChangeActionProgress);
        }

		public void OnClose(Unit unit)
        {
			gameObject.SetActive(false);
			if (unit != null)
            {
				GameEntry.Combat.UnRegisterActionProgressEvent(unit.InstanceID, ChangeActionProgress);
            }
        }

		private void ChangeActionProgress(double value)
        {
			float fValue = (float)value * 0.01f;
			fValue = Mathf.Clamp(fValue, 0, 1);
			if (m_OldValue > fValue)
            {
				StartCoroutine(IEBackAnim(fValue));
				return;
            }
			m_RectTransform.anchorMin = new Vector2(fValue, 0.5f);
			m_RectTransform.anchorMax = new Vector2(fValue, 0.5f);
			m_OldValue = fValue;
        }

		/// <summary>
		/// 回退动画
		/// </summary>
		/// <returns></returns>
		private IEnumerator IEBackAnim(float targetValue)
		{
			float time = 0f;
			var interval = new WaitForSeconds(m_AnimInterval);
			while (time < m_AnimTime)
            {
				yield return interval;
				var fValue = m_OldValue + (targetValue - m_OldValue) * (time / m_AnimTime);
				time += m_AnimInterval;
				m_RectTransform.anchorMin = new Vector2(fValue, 0.5f);
				m_RectTransform.anchorMax = new Vector2(fValue, 0.5f);
			}

			m_RectTransform.anchorMin = new Vector2(targetValue, 0.5f);
			m_RectTransform.anchorMax = new Vector2(targetValue, 0.5f);
			m_OldValue = targetValue;
		}
	}
}