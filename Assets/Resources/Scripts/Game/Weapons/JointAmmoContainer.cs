using System;
using Game.Character.CharacterController;
using Game.Items;
using Game.Shop;
using UnityEngine;

namespace Game.Weapons
{
	public class JointAmmoContainer : MonoBehaviour
	{
		public int AmmoCount
		{
			get
			{
				return this.GetAmmoCount();
			}
			set
			{
				this.ammoCount = value;
			}
		}

		public virtual int GetAmmoCount()
		{
			return this.ammoCount;
		}

		public void SaveAmmo()
		{
			ShopManager.Instance.SetBIValue(this.GameItemAmmo.ID, this.AmmoCount, false);
		}

		public void UpdateAmmo()
		{
			this.AmmoCount = ShopManager.Instance.GetBIValue(this.GameItemAmmo.ID, false);
			RangedWeapon rangedWeapon = PlayerManager.Instance.WeaponController.CurrentWeapon as RangedWeapon;
			if (rangedWeapon != null && rangedWeapon.AmmoContainer == this)
			{
				PlayerManager.Instance.WeaponController.UpdateAmmoText(rangedWeapon);
			}
		}

		public GameItemAmmo GameItemAmmo;

		[SerializeField]
		private int ammoCount;
	}
}
