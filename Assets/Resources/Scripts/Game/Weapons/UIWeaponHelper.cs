using System;
using Game.Character.CharacterController;
using Game.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Weapons
{
	public class UIWeaponHelper : MonoBehaviour
	{
		public WeaponSlot RelatedSlot
		{
			get
			{
				WeaponController weaponController = PlayerManager.Instance.WeaponController;
				return weaponController.WeaponSet.Slots[this.RelatedSlotIndex];
			}
		}

		private bool SlotIsLocked
		{
			get
			{
				return this.RelatedSlot.BuyToUnlock != null && !ShopManager.Instance.BoughtAlredy(this.RelatedSlot.BuyToUnlock);
			}
		}

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
			if (this.SlotIsLocked)
			{
				WeaponDialogPanel.BuyWeaponSlot(this.RelatedSlot.BuyToUnlock, new Action(this.UpdateImage));
			}
			else
			{
				PlayerManager.Instance.WeaponController.ChooseSlot(this.RelatedSlotIndex);
				WeaponManager.Instance.CloseWeaponPanel();
			}
		}

		private void OnEnable()
		{
			this.UpdateImage();
		}

		private void UpdateImage()
		{
			Weapon weaponPrefab = PlayerManager.Instance.WeaponController.WeaponSet.Slots[this.RelatedSlotIndex].WeaponPrefab;
			if (weaponPrefab == null)
			{
				this.Button.interactable = this.SlotIsLocked;
				this.WeaponIcon.sprite = ((!this.SlotIsLocked) ? this.EmptySlotIcon : ShopManager.Instance.ShopIcons.LockedSlotSprite);
				if (this.WaterMark)
				{
					this.WaterMark.SetActive(true);
				}
			}
			else
			{
				this.Button.interactable = true;
				this.WeaponIcon.sprite = weaponPrefab.image;
				if (this.WaterMark)
				{
					this.WaterMark.SetActive(false);
				}
			}
		}

		public int RelatedSlotIndex;

		public Button Button;

		public Image ButtonIcon;

		public Image WeaponIcon;

		public Sprite EmptySlotIcon;

		public GameObject WaterMark;
	}
}
