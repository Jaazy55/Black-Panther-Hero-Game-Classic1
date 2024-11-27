using System;
using Game.Weapons;
using UnityEngine;

namespace Game.Enemy
{
	public class WeaponForSmartHumanoidNPC : MonoBehaviour
	{
		public void Init(BaseNPC controlledNPC, SmartHumanoidWeaponController parentWeaponController)
		{
			this.WeaponsInitialize();
			this.MovePlaceholder(this.RightHandWeaponPlaceholder, controlledNPC.SpecificNpcLinks.RightHand.transform);
			this.MovePlaceholder(this.LeftHandWeaponPlaceholder, controlledNPC.SpecificNpcLinks.LeftHand.transform);
			this.currentWeaponController = parentWeaponController;
			this.currentWeaponController.Weapons.AddRange(this.weaponsInCase);
		}

		public void DeInit()
		{
			this.currentWeaponController.Weapons.Clear();
			this.currentWeaponController = null;
			this.MovePlaceholder(this.RightHandWeaponPlaceholder, base.transform);
			this.MovePlaceholder(this.LeftHandWeaponPlaceholder, base.transform);
		}

		private void WeaponsInitialize()
		{
			if (this.weaponsInCase == null)
			{
				this.weaponsInCase = base.GetComponentsInChildren<Weapon>();
				this.weaponsStartAmmo = new int[this.weaponsInCase.Length];
				for (int i = 0; i < this.weaponsInCase.Length; i++)
				{
					RangedWeapon rangedWeapon = this.weaponsInCase[i] as RangedWeapon;
					this.weaponsStartAmmo[i] = ((!(rangedWeapon != null)) ? 0 : rangedWeapon.AmmoOutOfCartridgeCount);
					this.weaponsInCase[i].gameObject.SetActive(false);
				}
			}
			else
			{
				for (int j = 0; j < this.weaponsInCase.Length; j++)
				{
					RangedWeapon rangedWeapon2 = this.weaponsInCase[j] as RangedWeapon;
					if (rangedWeapon2 != null)
					{
						rangedWeapon2.AmmoOutOfCartridgeCount = this.weaponsStartAmmo[j];
					}
				}
			}
		}

		private void MovePlaceholder(GameObject placeHolder, Transform toTransform)
		{
			placeHolder.transform.SetParent(toTransform, false);
		}

		public GameObject RightHandWeaponPlaceholder;

		public GameObject LeftHandWeaponPlaceholder;

		private Weapon[] weaponsInCase;

		private int[] weaponsStartAmmo;

		private SmartHumanoidWeaponController currentWeaponController;
	}
}
