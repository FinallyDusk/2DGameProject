using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoringWorld.UI.CombatForm
{
	/// <summary>
	/// 底部玩家单位物件
	/// </summary>
	public class PlayerUnitItem : MonoBehaviour
	{
		private Image m_UnitIcon;
		private TextMeshProUGUI m_UnitName;
		private Slider m_HpSlider;
		private TextMeshProUGUI m_HpContent;
		private Slider m_MpSlider;
		private TextMeshProUGUI m_MpContent;

		public bool Active
        {
            get
            {
				return gameObject.activeSelf;
            }
        }

		public void OnInit()
        {
			m_UnitIcon = transform.Find("UnitIcon").GetComponent<Image>();
			m_UnitName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
			m_HpSlider = transform.Find("HpBar").GetComponent<Slider>();
			m_HpContent = transform.Find("HpBar/HpContent").GetComponent<TextMeshProUGUI>();
			m_MpSlider = transform.Find("MpBar").GetComponent<Slider>();
			m_MpContent = transform.Find("MpBar/MpContent").GetComponent<TextMeshProUGUI>();
        }

		public void OnOpen(CombatUnitInfo belong)
        {
			gameObject.SetActive(true);
			belong.Prop.RegisterPropChangeEvent(UnitPropType.NowHp, HpChange);
			belong.Prop.RegisterPropChangeEvent(UnitPropType.NowMp, MpChange);
			HpChange(belong.Prop);
			MpChange(belong.Prop);
			m_UnitIcon.sprite = GameEntry.Sprite.GetSprite(belong.BaseData.MapDisplayIconName, SpriteType.UnitPaint);
			m_UnitName.text = belong.BaseData.UnitName;
        }

		public void OnClose(Unit belong)
        {
			gameObject.SetActive(false);
			if (belong != null)
			{
				belong.Prop.UnRegisterPropChangeEvent(UnitPropType.NowHp, HpChange);
				belong.Prop.UnRegisterPropChangeEvent(UnitPropType.NowMp, MpChange);
			}
        }

		private void HpChange(UnitProp prop)
        {
			double hp = prop.GetProperty(UnitPropType.NowHp);
			double maxHp = prop.GetProperty(UnitPropType.MaxHp);
			m_HpSlider.value = (float)(hp / maxHp);
			m_HpContent.text = $"{hp.ToString("f0")}/{maxHp.ToString("f0")}";
        }

		private void MpChange(UnitProp prop)
        {
			double mp = prop.GetProperty(UnitPropType.NowMp);
			double maxMp = prop.GetProperty(UnitPropType.MaxMp);
			m_MpSlider.value = (float)(mp / maxMp);
			m_MpContent.text = $"{mp.ToString("f0")}/{maxMp.ToString("f0")}";
        }
	}
}