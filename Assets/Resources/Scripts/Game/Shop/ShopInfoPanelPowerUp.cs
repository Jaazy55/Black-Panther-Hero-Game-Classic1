using System;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopInfoPanelPowerUp : ShopInfoPanel
	{
		public override void ShowInfo(GameItem incItem)
		{
			base.ShowInfo(incItem);
			GameItemPowerUp gameItemPowerUp = incItem as GameItemPowerUp;
			if (gameItemPowerUp == null)
			{
				UnityEngine.Debug.LogWarning("Пытался показать инфо PowerUp'а для неизвестного типа предмета.");
				return;
			}
			this.RemainingTimeSlider.minValue = 0f;
			this.RemainingTimeSlider.maxValue = (float)(gameItemPowerUp.Duration * ShopManager.Instance.GetBIValue(gameItemPowerUp.ID, gameItemPowerUp.ShopVariables.gemPrice));
			this.RemainingTimeSlider.value = (float)gameItemPowerUp.RemainingDuration;
			this.RemainingCooldownSlider.minValue = 0f;
			this.RemainingCooldownSlider.maxValue = (float)gameItemPowerUp.Cooldawn;
			this.RemainingCooldownSlider.value = (float)gameItemPowerUp.RemainingCooldawn;
		}

		[Space(5f)]
		public Slider RemainingTimeSlider;

		public Slider RemainingCooldownSlider;
	}
}
