using System;
using Game.GlobalComponent.HelpfulAds;
using Game.Items;
using Game.Weapons;
using UnityEngine;

public class BuyGemsDialogPanel : ShopDialogPanel
{
	public override void ProceedSlot(DialogSlotHelper helper)
	{
		DialogBuyGemsSlotHelper dialogBuyGemsSlotHelper = helper as DialogBuyGemsSlotHelper;
		if (dialogBuyGemsSlotHelper == null)
		{
			return;
		}
		if (dialogBuyGemsSlotHelper.ForFree)
		{
			HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.FreeGems, new Action<bool>(this.Refresh));
			dialogBuyGemsSlotHelper.UpdateSlot(false, this.GetImage(helper), this.CheckHighlighted(dialogBuyGemsSlotHelper, this.currItem));
		}
		else
		{
			//IAPController.Buy(this.InappsManager.GemPacks[dialogBuyGemsSlotHelper.SlotIndex].Item);
	

		}
	}

	public void Refresh(bool any)
	{
	}

	public override bool CheckAvailable(DialogSlotHelper helper, GameItem item)
	{
		DialogBuyGemsSlotHelper dialogBuyGemsSlotHelper = helper as DialogBuyGemsSlotHelper;
		return !(dialogBuyGemsSlotHelper == null) && (!dialogBuyGemsSlotHelper.ForFree || HelpfullAdsManager.Instance.IsReady);
	}

	public override Sprite GetImage(DialogSlotHelper helper)
	{
		DialogBuyGemsSlotHelper dialogBuyGemsSlotHelper = helper as DialogBuyGemsSlotHelper;
		//if (dialogBuyGemsSlotHelper == null)
		//{
			return null;
		//}
		//if (dialogBuyGemsSlotHelper.PriceText != null)
	//	{
		//	dialogBuyGemsSlotHelper.PriceText.text = ((!dialogBuyGemsSlotHelper.ForFree) ? IAPController.Items[this.InappsManager.GemPacks[dialogBuyGemsSlotHelper.SlotIndex].Item].FormattedPrice : "FREE!");
		//}
	//	if (dialogBuyGemsSlotHelper.ValueText != null)
	//	{
			//dialogBuyGemsSlotHelper.ValueText.text = ((!dialogBuyGemsSlotHelper.ForFree) ? ("X" + this.InappsManager.GemPacks[dialogBuyGemsSlotHelper.SlotIndex].GemValue) : ("X" + (HelpfullAdsManager.Instance.GetAdsByType(HelpfullAdsType.FreeGems) as MoneyAds).AddedMoney));
	//	}
		//return (!dialogBuyGemsSlotHelper.ForFree) ? this.InappsManager.GemPacks[dialogBuyGemsSlotHelper.SlotIndex].Icon : this.InappsManager.FreeGemsIcon;
	}

//	[Space(20f)]
//	public IAPMiamiManager InappsManager;
}
