using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;

namespace BoringWorld
{
    public class GlobalMessageItemLogic : MonoBehaviour
	{
        private TextMeshProUGUI msgContent;
        private DOTweenAnimation dotweenAnim;

        public void OnInit(UnityAction msgEndEvent)
        {
            msgContent = this.GetComponentWithName<TextMeshProUGUI>("MessageText");
            dotweenAnim = GetComponent<DOTweenAnimation>();
            dotweenAnim.onComplete.AddListener(msgEndEvent);
            gameObject.SetActive(false);
        }

        public void ReceiveMsg(string msg)
        {
            msgContent.text = msg;
            gameObject.SetActive(true);
            //transform.DOPlayForward();
        }
    }
}