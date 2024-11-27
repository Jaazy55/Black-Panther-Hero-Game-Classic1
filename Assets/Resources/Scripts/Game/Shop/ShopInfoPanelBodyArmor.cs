using System;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopInfoPanelBodyArmor : ShopInfoPanel
	{
		public override void ShowInfo(GameItem incItem)
		{
			base.ShowInfo(incItem);
			GameItemBodyArmor gameItemBodyArmor = incItem as GameItemBodyArmor;
			if (gameItemBodyArmor == null)
			{
				UnityEngine.Debug.LogWarning("Пытался показать инфо о бронике для неизвестного типа предмета.");
				return;
			}
			int bivalue = ShopManager.Instance.GetBIValue(gameItemBodyArmor.ID, gameItemBodyArmor.ShopVariables.gemPrice);
			string text = string.Format("Current amount: <color={0}>{1}</color>", ColorUtility.ToHtmlStringRGB(this.m_AmountColor), bivalue);
			this.CurrBodyArmorAmountText.text = text;
		}

		private const string Title = "Current amount: <color={0}>{1}</color>";

		[Space(5f)]
		public Text CurrBodyArmorAmountText;

		[SerializeField]
		private Color m_AmountColor;
	}
}
