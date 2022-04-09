using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoringWorld.UI.CombatForm
{
    using TMPro;
    using UITabGroup;
    public class IntroContent : BaseTabContent
    {
        private TextMeshProUGUI m_Content;
        public override void OnInit(object userData)
        {
            m_Content = transform.Find("Content").GetComponent<TextMeshProUGUI>();
        }

        public override void OnUpdateData(object userData)
        {
            if (userData == null) m_Content.text = string.Empty;
            m_Content.text = userData.ToString();
        }
    }
}