using System;
using Game.Items;
using Game.Shop;
using UnityEngine;

public class ShopDialogPanel : MonoBehaviour
{
	public void UpdatePanel()
	{
		this.UpdatePanel(null);
	}

	public void UpdatePanel(GameItem item)
	{
		if (item != null)
		{
			this.currItem = item;
		}
		foreach (DialogSlotHelper dialogSlotHelper in this.DialogHelpers)
		{
			dialogSlotHelper.UpdateSlot(this.CheckAvailable(dialogSlotHelper, this.currItem), this.GetImage(dialogSlotHelper), this.CheckHighlighted(dialogSlotHelper, this.currItem));
		}
	}

	public void Deinit()
	{
		ShopManager.Instance.CloseDialogPanel();
	}

	public virtual void ProceedSlot(DialogSlotHelper helper)
	{
	}

	public virtual void BuySlot(DialogSlotHelper helper)
	{
	}

	public virtual bool CheckAvailable(DialogSlotHelper helper, GameItem item)
	{
		return true;
	}

	public virtual bool CheckHighlighted(DialogSlotHelper helper, GameItem item)
	{
		return false;
	}

	public virtual Sprite GetImage(DialogSlotHelper helper)
	{
		return null;
	}

	public ItemsTypes[] DialogPanelTypes;

	public DialogSlotHelper[] DialogHelpers;

	protected GameItem currItem;
}
