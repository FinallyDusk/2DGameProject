using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace BoringWorld.UI.CombatForm
{
    /// <summary>
    /// 具体技能操作按钮
    /// </summary>
	public class SkillActionItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
        private TextMeshProUGUI m_Content;
		private Button m_Btn;
        private Image m_CDMask;
        private Image m_PitchOn;

        private SkillChildrenItemData m_ItemData;
        /// <summary>
        /// 提示触发的回调
        /// </summary>
        private Action<string> m_TipShowCallback;
        /// <summary>
        /// 提示隐藏的回调
        /// </summary>
        private Action m_TipHideCallback;
        /// <summary>
        /// 按钮点击后的回调
        /// </summary>
        private Action<int> m_BtnClickCallback;


		public void OnInit(Action<string> tipShowCallback, Action tipHideCallback)
        {
			m_Btn = GetComponent<Button>();
            m_Content = GetComponentInChildren<TextMeshProUGUI>();
            m_CDMask = transform.Find("CDMask").GetComponent<Image>();
            m_PitchOn = transform.Find("PitchOn").GetComponent<Image>();
            m_Btn.onClick.AddListener(BtnClick);
            m_TipShowCallback = tipShowCallback;
            m_TipHideCallback = tipHideCallback;
        }

        public void OnOpen(SkillChildrenItemData itemData, System.Action<int> clickCallback)
        {
            m_PitchOn.enabled = false;
            gameObject.SetActive(true);
            m_ItemData.Release();
            m_ItemData = itemData;
            m_Content.text = itemData.Name;
            //显示cd
            float prob = 0;
            if (itemData.MaxCDTurn != 0)
            {
                prob = itemData.NowCDTurn / (itemData.MaxCDTurn * 1.0f);
            }
            m_CDMask.fillAmount = prob;
            m_BtnClickCallback = clickCallback;
        }

        public void OnClose()
        {
            gameObject.SetActive(false);
        }

        private void BtnClick()
        {
            m_BtnClickCallback(m_ItemData.ID);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_PitchOn.enabled = false;
            m_TipHideCallback();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_PitchOn.enabled = true;
            m_TipShowCallback(m_ItemData.Des);
        }
    }

    /// <summary>
    /// 子操作数据
    /// </summary>
    public class SkillChildrenItemData : IReference
    {
        public int ID { get; private set; }

        public string Name { get; private set; }

        public string Des { get; private set; }

        public int NowCDTurn { get; private set; }

        public int MaxCDTurn { get; private set; }

        public void Clear()
        {
            
        }

        public static SkillChildrenItemData Create(int id, string name, string des, int nowCDTurn, int maxCDTurn)
        {
            SkillChildrenItemData result = ReferencePool.Acquire<SkillChildrenItemData>();
            result.ID = id;
            result.Name = name;
            result.Des = des;
            result.NowCDTurn = nowCDTurn;
            result.MaxCDTurn = maxCDTurn;
            return result;
        }
    }
}