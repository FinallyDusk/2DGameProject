using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace BoringWorld
{
    using MsgItem = GlobalMessageItemLogic;
	public class GlobalMessageFormLogic : UIFormLogic
	{
        private List<MsgItem> m_Items;
        private int m_CurrentShowCount;
        private List<string> m_MsgQueue;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            GameEntry.UI.SetUIFormInstanceLocked(UIForm, true);
            m_Items = new List<MsgItem>(GetComponentsInChildren<MsgItem>());
            for (int i = 0; i < m_Items.Count; i++)
            {
                m_Items[i].OnInit(MsgEndAction);
            }
            m_CurrentShowCount = 0;
            m_MsgQueue = new List<string>();

            GameEntry.Event.Subscribe(SendGlobalMessageEventArgs.EventId, SendGlobalMessageCallback);
        }

        private void MsgEndAction()
        {
            var first = m_Items[0];
            m_Items.RemoveAt(0);
            m_Items.Add(first);
            m_CurrentShowCount--;
            first.transform.SetAsLastSibling();
            first.SetActive(false);
            //ExecuteShowMsg();
        }

        private void ExecuteShowMsg()
        {
            m_CurrentShowCount++;
            string msg = m_MsgQueue[0];
            m_MsgQueue.RemoveAt(0);
            m_Items[m_CurrentShowCount - 1].ReceiveMsg(msg);
        }

        public void SendGlobalMessageCallback(object sender, System.EventArgs e)
        {
            var ge = e as SendGlobalMessageEventArgs;
            m_MsgQueue.Add(ge.Msg);
            //ExecuteShowMsg();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_MsgQueue.Count > 0)
            {
                if (m_CurrentShowCount < m_Items.Count)
                {
                    ExecuteShowMsg();
                }
            }
        }
    }
}