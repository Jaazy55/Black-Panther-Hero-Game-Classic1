using System;
using Game.GlobalComponent;
using Game.Items;
using Game.Shop;
using UnityEngine;

namespace Game.PickUps
{
	public class WeaponPickup : PickUp
	{
		protected override void TakePickUp()
		{
			this.WeaponItem = (GameItemWeapon)ItemsManager.Instance.GetItem(this.GameItemWeaponId);
			if (ShopManager.Instance.BoughtAlredy(this.WeaponItem))
			{
				AmmoManager.Instance.AddAmmo(this.WeaponItem.Weapon.AmmoType);
			}
			else
			{
				ShopManager.Instance.Give(this.WeaponItem, false);
			}
			ShopManager.Instance.Equip(this.WeaponItem, true);
			InGameLogManager.Instance.RegisterNewMessage(MessageType.Item, this.WeaponItem.ShopVariables.Name);
			PointSoundManager.Instance.PlayCustomClipAtPoint(base.transform.position, PickUpManager.Instance.WeaponItemPickupSound, null);
			CollectionPickUpsManager.Instance.ElementWasTaken(base.gameObject);
			base.TakePickUp();
		}

		[Tooltip("Итемы ТОЛЬКО лежащие под GameItems НА СЦЕНЕ")]
		public int GameItemWeaponId;

		private GameItemWeapon WeaponItem;
	}
}
