using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogSlotHelper : MonoBehaviour
{
	private void Start()
	{
		if (this.Button == null)
		{
			this.Button = base.GetComponent<Button>();
		}
		if (this.ButtonIcon == null)
		{
			this.ButtonIcon = base.GetComponent<Image>();
		}
	}

	public void OnClick()
	{
		if (!this.BuyFirst)
		{
			this.DialogPanel.ProceedSlot(this);
		}
		else
		{
			this.DialogPanel.BuySlot(this);
		}
	}

	public virtual void UpdateSlot(bool IsAvailable, Sprite sprite, bool highlighted)
	{
		if (IsAvailable)
		{
			this.Button.interactable = true;
			this.ButtonIcon.color = this.AvailavleColor;
		}
		else
		{
			this.Button.interactable = false;
			this.ButtonIcon.color = this.NotAvailavleColor;
		}
		if (highlighted)
		{
			this.ButtonIcon.color = this.HighlightedColor;
		}
		if (sprite)
		{
			this.ItemIcon.sprite = sprite;
			this.ItemIcon.gameObject.SetActive(true);
		}
		else
		{
			this.ItemIcon.gameObject.SetActive(true);
		}
	}

	public ShopDialogPanel DialogPanel;

	public bool BuyFirst;

	public int SlotIndex;

	[Space(10f)]
	public Button Button;

	public Image ButtonIcon;

	public Image ItemIcon;

	public Sprite EmptySlotIcon;

	[Space(10f)]
	public Color AvailavleColor;

	public Color NotAvailavleColor;

	public Color HighlightedColor;
}
