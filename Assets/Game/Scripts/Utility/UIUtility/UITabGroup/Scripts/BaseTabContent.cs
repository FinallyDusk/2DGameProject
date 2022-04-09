using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.UITabGroup
{
    public abstract class BaseTabContent : MonoBehaviour, ITabContent
    {
        public void OnClose()
        {
            gameObject.SetActive(false);
        }

        public abstract void OnInit(object userData);

        public void OnOpen()
        {
            gameObject.SetActive(true);
        }

        public abstract void OnUpdateData(object userData);
    }
}