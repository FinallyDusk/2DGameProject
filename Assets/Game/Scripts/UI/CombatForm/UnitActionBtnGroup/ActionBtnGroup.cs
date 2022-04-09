using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoringWorld.UI.CombatForm
{
    /// <summary>
    /// 按钮组
    /// </summary>
	public class ActionBtnGroup : MonoBehaviour
	{
		private ActionBtnItem m_AtkActionBtn;
		private ActionBtnItem m_MoveActionBtn;
		private ActionBtnItem m_SkillActionBtn;
		private ActionBtnItem m_UsePropActionBtn;
		private ActionBtnItem m_RunawayActionBtn;

        private DOTweenAnimation m_Anim;

        private Action<UnitActionType> m_ActionBtnCallback;

		public void OnInit(Action<UnitActionType> actionBtnCallback)
        {
            m_Anim = GetComponent<DOTweenAnimation>();

			m_AtkActionBtn = transform.Find("AtkAction").GetComponent<ActionBtnItem>();
			m_MoveActionBtn = transform.Find("MoveAction").GetComponent<ActionBtnItem>();
			m_SkillActionBtn = transform.Find("SkillAction").GetComponent<ActionBtnItem>();
			m_UsePropActionBtn = transform.Find("UsePropAction").GetComponent<ActionBtnItem>();
			m_RunawayActionBtn = transform.Find("RunawayAction").GetComponent<ActionBtnItem>();

			m_AtkActionBtn.OnInit(AtkActionBtnClick);
			m_MoveActionBtn.OnInit(MoveActionBtnClick);
			m_SkillActionBtn.OnInit(SkillActionBtnClick);
			m_UsePropActionBtn.OnInit(UsePropActionBtnClick);
			m_RunawayActionBtn.OnInit(RunawayActionBtnClick);

            m_ActionBtnCallback = actionBtnCallback;

        }

        private void RunawayActionBtnClick()
        {
            m_ActionBtnCallback(UnitActionType.Runaway);
        }

        private void UsePropActionBtnClick()
        {
            m_ActionBtnCallback(UnitActionType.Prop);
        }

        private void SkillActionBtnClick()
        {
            m_ActionBtnCallback(UnitActionType.Skill);
        }

        private void MoveActionBtnClick()
        {
            m_ActionBtnCallback(UnitActionType.Move);
        }

        private void AtkActionBtnClick()
        {
            m_ActionBtnCallback(UnitActionType.Attack);
        }

        public void OnOpen()
        {
            m_Anim.DOPlayBackwards();
            m_AtkActionBtn.OnHide();
            m_MoveActionBtn.OnHide();
            m_SkillActionBtn.OnHide();
            m_UsePropActionBtn.OnHide();
            m_RunawayActionBtn.OnHide();
        }

        public void ResetOpen()
        {
            m_Anim.DORewind();
        }

        public void OnHide()
        {
            m_Anim.DOPlayForward();
        }
	}
}