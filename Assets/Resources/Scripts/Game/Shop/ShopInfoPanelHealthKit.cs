using System;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopInfoPanelHealthKit : ShopInfoPanel
	{
		public override void ShowInfo(GameItem incItem)
		{
			base.ShowInfo(incItem);
			GameItemHealth gameItemHealth = incItem as GameItemHealth;
			if (gameItemHealth == null)
			{
				UnityEngine.Debug.LogWarning("Пытался показать инфо о здоровье для неизвестного типа предмета.");
				return;
			}
			int bivalue = ShopManager.Instance.GetBIValue(gameItemHealth.ID, gameItemHealth.ShopVariables.gemPrice);
			string text = string.Format("Current amount: <color={0}>{1}</color>", ColorUtility.ToHtmlStringRGB(this.m_AmountColor), bivalue);
			this.CurrHealthKitAmountText.text = text;
		}

		private const string Title = "Current amount: <color={0}>{1}</color>";

		[Space(5f)]
		public Text CurrHealthKitAmountText;

		[SerializeField]
		private Color m_AmountColor;
	}
}
