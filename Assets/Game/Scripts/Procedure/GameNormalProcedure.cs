using GameFramework.Fsm;
using GameFramework.Procedure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
	public class GameNormalProcedure : ProcedureBase
    {
        private Stack<UIForm> m_LastOpenUI;

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            m_LastOpenUI = new Stack<UIForm>();
        }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OpenUIFormSuccessCallback);
            GameEntry.Event.Subscribe(CloseUIFormCompleteEventArgs.EventId, CloseUIFormCompleteCallback);
            //注册快捷键
            GameEntry.Input.RegisterKeyCodeDownAction(KeyCode.Escape, KeyCodeByEscapeAction);
            //打开普通界面UI
            GameEntry.UI.OpenUIFormByType(UIFormType.GameMainInterface, null);
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            GameEntry.Input.Update();
            GameEntry.Unit.Update();
            GameEntry.Combat.Update(elapseSeconds);
        }

        private void KeyCodeByEscapeAction()
        {
            if (m_LastOpenUI.Count == 0)
            {
                GameEntry.UI.OpenUIFormByType(UIFormType.GameMenu);
            }
            else
            {
                var uiForm = m_LastOpenUI.Pop();
                GameEntry.UI.CloseUIForm(uiForm);
            }
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OpenUIFormSuccessCallback);
            GameEntry.Event.Unsubscribe(CloseUIFormCompleteEventArgs.EventId, CloseUIFormCompleteCallback);
            //解除快捷键
            GameEntry.Input.UnRegisterKeyCodeDownAction(KeyCode.Escape, KeyCodeByEscapeAction);
        }

        private void OpenUIFormSuccessCallback(object sender, System.EventArgs e)
        {
            var oe = e as OpenUIFormSuccessEventArgs;
            if (oe.UIForm.GetComponent<MainInterfaceFormLogic>() == null)
            {
                m_LastOpenUI.Push(oe.UIForm);
            }
        }

        private void CloseUIFormCompleteCallback(object sender, System.EventArgs e)
        {
            //GameEntry.Event.Fire(this, CloseTipMsgEventArgs.Create());
            if (m_LastOpenUI.Count == 0) return;
            var ce = e as CloseUIFormCompleteEventArgs;
            if (ce.UIFormAssetName == m_LastOpenUI.Peek().UIFormAssetName)
            {
                m_LastOpenUI.Pop();
            }
        }
    }
}