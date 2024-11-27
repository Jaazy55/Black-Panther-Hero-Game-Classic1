using System;
using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using UnityEngine;

namespace Game.Weapons
{
	public class EnergyAmmoRangedWeapon : RangedWeapon
	{
		public override int AmmoInCartridgeCount
		{
			get
			{
				if (base.WeaponOwner != null)
				{
					Player player = base.WeaponOwner as Player;
					if (player)
					{
						return (int)(player.stats.stamina.Current / this.EnergyPerShot);
					}
				}
				return this.ammoInCartridgeCount;
			}
			set
			{
				this.ammoInCartridgeCount = value;
			}
		}

		public override int AmmoCount
		{
			get
			{
				return this.AmmoInCartridgeCount;
			}
		}

		public override string AmmoCountText
		{
			get
			{
				return this.AmmoInCartridgeCount.ToString();
			}
		}

		public override void DeInit()
		{
		}

		public override RangedAttackState GetRangedAttackState()
		{
			this.LastGetStateTime = Time.time;
			if (base.WeaponOwner != null)
			{
				bool flag = false;
				Ray ray;
				if (base.WeaponOwner as Player)
				{
					ray = Camera.main.ScreenPointToRay(TargetManager.Instance.CrosshairStart.position);
				}
				else
				{
					ray = new Ray(this.Muzzle.transform.position, base.WeaponOwner.transform.forward);
				}
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, this.AttackDistance, RangedWeapon.characterLayerMask))
				{
					Human component = raycastHit.collider.GetComponent<Human>();
					if (component != null && FactionsManager.Instance.GetRelations(component.Faction, base.WeaponOwner.Faction) == Relations.Friendly)
					{
						flag = true;
					}
				}
				if (flag)
				{
					return RangedAttackState.ShootInFriendly;
				}
			}
			if (base.IsOnCooldown)
			{
				return RangedAttackState.Idle;
			}
			if (this.IsRecharging)
			{
				return RangedAttackState.Recharge;
			}
			if (base.IsCartridgeEmpty)
			{
				return RangedAttackState.Idle;
			}
			return RangedAttackState.Shoot;
		}

		protected override void ChangeAmmo(int amount)
		{
			if (base.WeaponOwner != null && amount < 0)
			{
				Player player = base.WeaponOwner as Player;
				if (player)
				{
					player.stats.stamina.Change(-this.EnergyPerShot);
				}
			}
			else
			{
				this.AmmoInCartridgeCount += amount;
			}
			if (this.AmmoChangedEvent != null)
			{
				this.AmmoChangedEvent(this);
			}
		}

		public float EnergyPerShot = 1f;
	}
}
