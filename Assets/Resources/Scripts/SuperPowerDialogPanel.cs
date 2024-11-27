using System;
using Game.Character.Superpowers;
using Game.Items;
using Game.Shop;
using Game.Weapons;
using UnityEngine;

public class SuperPowerDialogPanel : ShopDialogPanel
{
	public override void ProceedSlot(DialogSlotHelper helper)
	{
		DialogSuperpowerSlotHelper dialogSuperpowerSlotHelper = helper as DialogSuperpowerSlotHelper;
		GameItemAbility gameItemAbility = this.currItem as GameItemAbility;
		if (dialogSuperpowerSlotHelper == null || gameItemAbility == null)
		{
			return;
		}
		StuffManager.Instance.EquipAbility(gameItemAbility, dialogSuperpowerSlotHelper.SlotIndex);
		ShopManager.Instance.UpdateInfo();
		base.Deinit();
	}

	public override void BuySlot(DialogSlotHelper helper)
	{
	}

	public override bool CheckAvailable(DialogSlotHelper helper, GameItem item)
	{
		DialogSuperpowerSlotHelper dialogSuperpowerSlotHelper = helper as DialogSuperpowerSlotHelper;
		GameItemAbility gameItemAbility = item as GameItemAbility;
		return !(dialogSuperpowerSlotHelper == null) && !(gameItemAbility == null) && !(dialogSuperpowerSlotHelper.Active ^ gameItemAbility.IsActive);
	}

	public override Sprite GetImage(DialogSlotHelper helper)
	{
		DialogSuperpowerSlotHelper dialogSuperpowerSlotHelper = helper as DialogSuperpowerSlotHelper;
		if (dialogSuperpowerSlotHelper == null)
		{
			return null;
		}
		Sprite result = null;
		GameItemAbility gameItemAbility;
		if (dialogSuperpowerSlotHelper.Active)
		{
			gameItemAbility = PlayerAbilityManager.ActiveAbilities[dialogSuperpowerSlotHelper.SlotIndex];
		}
		else
		{
			gameItemAbility = PlayerAbilityManager.PasiveAbilities[dialogSuperpowerSlotHelper.SlotIndex];
		}
		if (gameItemAbility != null)
		{
			result = gameItemAbility.ShopVariables.ItemIcon;
		}
		return result;
	}
}
