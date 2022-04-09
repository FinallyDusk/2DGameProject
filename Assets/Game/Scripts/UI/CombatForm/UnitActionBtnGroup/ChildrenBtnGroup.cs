using DG.Tweening;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace BoringWorld.UI.CombatForm
{
    /// <summary>
    /// 子按钮组
    /// </summary>
	public class ChildrenBtnGroup : MonoBehaviour
	{
#pragma warning disable 0649
        [SerializeField] private SkillActionItem m_Template;
        [SerializeField] private Transform m_ItemParent;
#pragma warning restore 0649
        private Action<string> m_TemplateTipShowCallback;
        private Action m_TemplateTipHideCallback;

		private List<SkillActionItem> m_AllSkillChildActions;

        private DOTweenAnimation m_Anim;
        public void OnInit(Action<string> tipShowCallback, Action tipHideCallback)
        {
            m_TemplateTipShowCallback = tipShowCallback;
            m_TemplateTipHideCallback = tipHideCallback;

            m_AllSkillChildActions = new List<SkillActionItem>();
            m_Anim = GetComponent<DOTweenAnimation>();
        }

        public void OnOpenSkill(SkillChildrenItemData[] allItemDatas, System.Action<int> clickCallback)
        {
            gameObject.SetActive(true);
            m_Anim.DOPlayForward();
            for (int i = 0; i < allItemDatas.Length; i++)
            {
                if (m_AllSkillChildActions.Count <= i)
                {
                    var obj = Instantiate(m_Template, m_ItemParent);
                    obj.OnInit(m_TemplateTipShowCallback, m_TemplateTipHideCallback);
                    m_AllSkillChildActions.Add(obj);
                }
                m_AllSkillChildActions[i].OnOpen(allItemDatas[i], clickCallback);
            }
            for (int i = allItemDatas.Length; i < m_AllSkillChildActions.Count; i++)
            {
                m_AllSkillChildActions[i].OnClose();
            }
        }

        public void ResetOpen()
        {
            m_Anim.DORewind();
        }
		
        public void OnClose()
        {
            m_Anim.DOPlayBackwards();
        }
	}
}