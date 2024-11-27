using System;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopInfoPanelAmmo : ShopInfoPanel
	{
		public override void ShowInfo(GameItem incItem)
		{
			base.ShowInfo(incItem);
			GameItemAmmo gameItemAmmo = incItem as GameItemAmmo;
			if (gameItemAmmo == null)
			{
				UnityEngine.Debug.LogWarning("Пытался показать инфо о патронах для неизвестного типа предмета.");
				return;
			}
			this.AmmoTypeText.text = "AmmoType: " + gameItemAmmo.AmmoType;
			this.CurrAmmoAmountText.text = "Current amount: " + ShopManager.Instance.GetBIValue(gameItemAmmo.ID, gameItemAmmo.ShopVariables.gemPrice);
		}

		[Space(5f)]
		public Text AmmoTypeText;

		public Text CurrAmmoAmountText;
	}
}
