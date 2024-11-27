using System;
using System.Diagnostics;
using Game.Items;
using Game.PickUps;
using Game.Shop;
using Prefabs.Effects.PowerShield;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipmentItemDisplay : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event AmountChanged OnAmountChanged;

	public static void CallEvent(GameItem gameItem, int amount)
	{
		if (PlayerEquipmentItemDisplay.OnAmountChanged != null)
		{
			PlayerEquipmentItemDisplay.OnAmountChanged(gameItem, amount);
		}
	}

	public void UseEquipmentItem()
	{
		int bivalue = ShopManager.Instance.GetBIValue(this.gameItem.ID, this.gameItem.ShopVariables.gemPrice);
		if (PickUpManager.Instance.BodyArmorGameItem.ID == this.gameItem.ID)
		{
			if (bivalue <= 0)
			{
				this.shopButton.onClick.Invoke();
				ShopManager.Instance.JumpToCategory<GameItemBodyArmor>(ItemsTypes.HealthKit);
			}
			else
			{
				BodyArmorManager.Instance.ResurrectBodyArmor();
			}
		}
		if (PickUpManager.Instance.HealthKitGameItem.ID == this.gameItem.ID)
		{
			if (bivalue <= 0)
			{
				this.shopButton.onClick.Invoke();
				ShopManager.Instance.JumpToCategory<GameItemHealth>(ItemsTypes.HealthKit);
			}
			else
			{
				BodyArmorManager.Instance.ResurrectHealth();
			}
		}
	}

	private void OnEnable()
	{
		PlayerEquipmentItemDisplay.OnAmountChanged += this.ChangeText;
		int bivalue = ShopManager.Instance.GetBIValue(this.gameItem.ID, this.gameItem.ShopVariables.gemPrice);
		this.amountText.text = bivalue.ToString();
		if (this.initialized && PickUpManager.Instance != null && bivalue > 0 && PickUpManager.Instance.BodyArmorGameItem.ID == this.gameItem.ID)
		{
			BodyArmorManager.Instance.EnableSlider(true);
		}
		this.initialized = true;
	}

	private void OnDisable()
	{
		PlayerEquipmentItemDisplay.OnAmountChanged -= this.ChangeText;
	}

	private void ChangeText(GameItem gameItem, int amount)
	{
		if (this.gameItem.ID == gameItem.ID)
		{
			this.amountText.text = amount.ToString();
		}
	}

	[SerializeField]
	private GameItem gameItem;

	[SerializeField]
	private Text amountText;

	[SerializeField]
	private Button shopButton;

	private bool initialized;
}
