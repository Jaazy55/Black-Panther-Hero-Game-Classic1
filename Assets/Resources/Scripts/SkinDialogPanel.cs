using System;
using Game.Items;
using Game.Shop;
using UnityEngine;

public class SkinDialogPanel : ShopDialogPanel
{
	public override void ProceedSlot(DialogSlotHelper helper)
	{
	}

	public override void BuySlot(DialogSlotHelper helper)
	{
	}

	public override bool CheckAvailable(DialogSlotHelper helper, GameItem item)
	{
		return false;
	}

	public override bool CheckHighlighted(DialogSlotHelper helper, GameItem item)
	{
		return false;
	}

	public override Sprite GetImage(DialogSlotHelper helper)
	{
		DialogSkinSlotHelper dialogSkinSlotHelper = helper as DialogSkinSlotHelper;
		if (dialogSkinSlotHelper == null)
		{
			return null;
		}
		Sprite result = null;
		GameItem gameItem = StuffManager.ItemInSkinSlot(dialogSkinSlotHelper.SlotType);
		if (gameItem != null)
		{
			result = gameItem.ShopVariables.ItemIcon;
		}
		return result;
	}
}
