using BoringWorld.Combat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BoringWorld
{
	public class BuffItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private Image m_Icon;
		private TextMeshProUGUI m_TimeTurn;
		private int m_BuffId;
		private RectTransform m_RT;
		private string m_BuffDes;
		public int BuffId { get { return m_BuffId; } }

        public void OnInit()
        {
			m_Icon = transform.Find("Icon").GetComponent<Image>();
			m_TimeTurn = transform.Find("Turn").GetComponent<TextMeshProUGUI>();
			m_BuffId = -1;
			m_RT = GetComponent<RectTransform>();
        }

		public void OnOpen(int buffID)
        {
			gameObject.SetActive(true);
			m_BuffId = buffID;
        }

		public void OnClose()
        {
			gameObject.SetActive(false);
        }

		public void OnRefresh(BuffInstanceData buff)
        {
			m_Icon.sprite = GameEntry.Sprite.GetSprite(buff.IconName, SpriteType.Buff);
			m_TimeTurn.text = buff.GetDisplayInfo();
			m_BuffDes = buff.GetDes();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
			if (string.IsNullOrEmpty(m_BuffDes)) return;
			GameEntry.Event.Fire(this, SendTipMsgEventArgs.Create(m_BuffDes, null, new Rect(m_RT.position, m_RT.sizeDelta)));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
			if (string.IsNullOrEmpty(m_BuffDes)) return;
			GameEntry.Event.Fire(this, CloseTipMsgEventArgs.Create());
		}
    }
}