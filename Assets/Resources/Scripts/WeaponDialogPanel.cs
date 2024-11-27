using System;
using Game.Character.CharacterController;
using Game.Items;
using Game.Shop;
using Game.UI;
using Game.Weapons;
using UnityEngine;

public class WeaponDialogPanel : ShopDialogPanel
{
	public static void BuyWeaponSlot(GameItem weaponSlot, Action additionalAction = null)
	{
		bool disableYesBitton = !ShopManager.Instance.ItemAvailableForBuy(weaponSlot) || !ShopManager.Instance.EnoughMoneyToBuyItem(weaponSlot);
		string message = string.Concat(new object[]
		{
			"Do you realy want to buy ",
			weaponSlot.ShopVariables.Name,
			" for ",
			weaponSlot.ShopVariables.price,
			" gems?"
		});
		UniversalYesNoPanel.Instance.DisplayOffer("Buy slot?", message, delegate()
		{
			ShopManager.Instance.Buy(weaponSlot);
			ShopManager.Instance.UpdateDialogPanel(null);
			if (additionalAction != null)
			{
				additionalAction();
			}
		}, null, disableYesBitton);
	}

	public override void ProceedSlot(DialogSlotHelper helper)
	{
		DialogWeaponSlotHelper dialogWeaponSlotHelper = helper as DialogWeaponSlotHelper;
		if (dialogWeaponSlotHelper == null)
		{
			return;
		}
		StuffManager.Instance.EquipWeapon(this.currItem as GameItemWeapon, dialogWeaponSlotHelper.SlotIndex, false);
		base.Deinit();
	}

	public override void BuySlot(DialogSlotHelper helper)
	{
		DialogWeaponSlotHelper dialogWeaponSlotHelper = helper as DialogWeaponSlotHelper;
		if (dialogWeaponSlotHelper == null)
		{
			return;
		}
		WeaponDialogPanel.BuyWeaponSlot(dialogWeaponSlotHelper.RelatedSlot.BuyToUnlock, null);
	}

	public override bool CheckAvailable(DialogSlotHelper helper, GameItem item)
	{
		DialogWeaponSlotHelper dialogWeaponSlotHelper = helper as DialogWeaponSlotHelper;
		if (dialogWeaponSlotHelper == null)
		{
			return false;
		}
		GameItemWeapon gameItemWeapon = item as GameItemWeapon;
		if (gameItemWeapon == null)
		{
			return false;
		}
		WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
		dialogWeaponSlotHelper.BuyFirst = (dialogWeaponSlotHelper.RelatedSlot.BuyToUnlock != null && !ShopManager.Instance.BoughtAlredy(dialogWeaponSlotHelper.RelatedSlot.BuyToUnlock));
		return dialogWeaponSlotHelper.RelatedSlot.WeaponSlotType == defaultWeaponController.GetTargetSlot(gameItemWeapon.Weapon) || dialogWeaponSlotHelper.RelatedSlot.WeaponSlotType == WeaponSlotTypes.Universal;
	}

	public override Sprite GetImage(DialogSlotHelper helper)
	{
		DialogWeaponSlotHelper dialogWeaponSlotHelper = helper as DialogWeaponSlotHelper;
		if (dialogWeaponSlotHelper == null)
		{
			return null;
		}
		WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
		Sprite result = null;
		if (dialogWeaponSlotHelper.BuyFirst)
		{
			result = ShopManager.Instance.ShopIcons.LockedSlotSprite;
		}
		else
		{
			Weapon weapon = defaultWeaponController.WeaponSet.WeaponInSlot(dialogWeaponSlotHelper.SlotIndex);
			if (weapon != null)
			{
				result = weapon.image;
			}
		}
		return result;
	}
}
