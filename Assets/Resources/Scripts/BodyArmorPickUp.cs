using System;
using Game.GlobalComponent;
using Game.PickUps;
using Game.Shop;
using Prefabs.Effects.PowerShield;

public class BodyArmorPickUp : PickUp
{
	protected override void TakePickUp()
	{
		InGameLogManager.Instance.RegisterNewMessage(MessageType.BodyArmor, "BodyArmor");
		int bivalue = ShopManager.Instance.GetBIValue(PickUpManager.Instance.BodyArmorGameItem.ID, PickUpManager.Instance.BodyArmorGameItem.ShopVariables.gemPrice);
		ShopManager.Instance.SetBIValue(PickUpManager.Instance.BodyArmorGameItem.ID, bivalue + 1, PickUpManager.Instance.BodyArmorGameItem.ShopVariables.gemPrice);
		PlayerEquipmentItemDisplay.CallEvent(PickUpManager.Instance.BodyArmorGameItem, bivalue + 1);
		BodyArmorManager.Instance.EnableSlider(true);
		base.TakePickUp();
	}
}
