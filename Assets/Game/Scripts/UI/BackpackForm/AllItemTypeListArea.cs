using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoringWorld.UI.Backpack
{
	public class AllItemTypeListArea : MonoBehaviour
	{
		private Button m_EquipTypeBtn;
		private Button m_ConsumablesTypeBtn;
		private Button m_MatTypeBtn;
		private Button m_OtherTypeBtn;

		/// <summary>
		/// 二级分类按钮组
		/// </summary>
		private List<Button> m_SecnodTypeBtns;
		private Dictionary<Button, int> m_SecondBtnsArgs;

#pragma warning disable 0649
        private LoopSlider m_AllShowItems;
#pragma warning restore 0649

        private BackpackFormLogic m_FormLogic;

		public void OnInit(BackpackFormLogic formLogic)
        {
			this.m_FormLogic = formLogic;
			m_EquipTypeBtn = this.GetComponentWithName<Button>("EquipTypeBtn");
			m_ConsumablesTypeBtn = this.GetComponentWithName<Button>("ConsumablesTypeBtn");
			m_MatTypeBtn = this.GetComponentWithName<Button>("MatTypeBtn");
			m_OtherTypeBtn = this.GetComponentWithName<Button>("OtherTypeBtn");
			m_EquipTypeBtn.onClick.AddListener(EquipTypeBtnClick);
			m_ConsumablesTypeBtn.onClick.AddListener(ConsumablesTypeBtnClick);
			m_MatTypeBtn.onClick.AddListener(MatTypeBtnClick);
			m_OtherTypeBtn.onClick.AddListener(OtherTypeBtnClick);

			var tempBtn = transform.Find("DetailTypeTabArea/TabsArea/Viewport/Content/EquipTypeBtn").GetComponent<Button>();
			m_SecnodTypeBtns = new List<Button>();
			m_SecondBtnsArgs = new Dictionary<Button, int>();
			m_SecnodTypeBtns.Add(tempBtn);

			m_AllShowItems = this.GetComponentWithName<LoopSlider>("DetailAllItemArea");

		}

		public void RefreshShowItems(ILoopSliderDataSource dataSource)
        {
			m_AllShowItems.InitLoopSlider(dataSource, m_FormLogic);
        }

		private void EquipTypeBtnClick()
        {
			m_FormLogic.ChangeShowItemType(ItemType.Equip);
			//更改二级菜单
			//var allEquipPartTypes = System.Enum.GetValues(typeof(UnitEquipPartType)) as UnitEquipPartType[];
            //for (int i = 0; i < allEquipPartTypes.; i++)
            //{
			//
            //}
	}

		private void ConsumablesTypeBtnClick()
        {
			m_FormLogic.ChangeShowItemType(ItemType.Consumables);
		}

		private void MatTypeBtnClick()
        {
			m_FormLogic.ChangeShowItemType(ItemType.Mat);
		}

		private void OtherTypeBtnClick()
        {
			m_FormLogic.ChangeShowItemType(ItemType.Other);
		}
	}
}