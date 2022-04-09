using BoringWorld.UITabGroup;
using DG.Tweening;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BoringWorld.UI.CombatForm
{
	/// <summary>
	/// 显示各种信息用的界面
	/// </summary>
	public class DisplayInfo : MonoBehaviour
	{
		private DOTweenAnimation m_Anim;
		private TextMeshProUGUI m_UnitName;
		private TextMeshProUGUI m_UnitLv;
		private TextMeshProUGUI m_HpContent;
		private SliderBar m_HpBar;
		private TextMeshProUGUI m_MpContent;
		private SliderBar m_MpBar;
		private TabGroup m_TabGroup;

		public void OnInit()
        {
			m_Anim = GetComponent<DOTweenAnimation>();
			m_UnitName = this.GetComponentWithName<TextMeshProUGUI>("UnitName");
			m_UnitLv = this.GetComponentWithName<TextMeshProUGUI>("UnitLv");
			m_HpContent = this.GetComponentWithName<TextMeshProUGUI>("HpContent");
			m_HpBar = this.GetComponentWithName<SliderBar>("HpBar");
			m_HpBar.OnInit();
			m_MpContent = this.GetComponentWithName<TextMeshProUGUI>("MpContent");
			m_MpBar = this.GetComponentWithName<SliderBar>("MpBar");
			m_MpBar.OnInit();
			m_TabGroup = this.GetComponentWithName<TabGroup>("TabGroup");
			m_TabGroup.OnInit(null);
        }

		public void OnOpen(DisplayInfoData data)
        {
			m_Anim.DOPlayForward();
			m_UnitName.text = data.UnitName;
			m_UnitLv.text = data.UnitLv.ToString();
			m_HpContent.text = $"{PropertyToString.GetStrOnlyValue(UnitPropType.NowHp, data.NowHp)}/{PropertyToString.GetStrOnlyValue(UnitPropType.MaxHp, data.MaxHp)}";
			m_HpBar.OnOpen((float)(data.NowHp / data.MaxHp));
			m_MpContent.text = $"{PropertyToString.GetStrOnlyValue(UnitPropType.NowMp, data.NowMp)}/{PropertyToString.GetStrOnlyValue(UnitPropType.MaxMp, data.MaxMp)}";
			m_MpBar.OnOpen((float)(data.NowMp / data.MaxMp));
			m_TabGroup.OnUpdateContent(data.UserDatas);
		}

		public void OnHide()
        {
			m_Anim.DOPlayBackwards();
        }

		public void OnClose()
        {
			m_Anim.DORewind();
        }

	}

	public class DisplayInfoData : IReference
    {
		public string UnitName;
		public int UnitLv;
		public double NowHp;
		public double MaxHp;
		public double NowMp;
		public double MaxMp;
		public object[] UserDatas;

        public void Clear()
        {
			UnitName = string.Empty;
			UnitLv = -1;
			NowHp = -1;
			MaxHp = -1;
			NowMp = -1;
			MaxMp = -1;
			UserDatas = null;
        }
    }
}