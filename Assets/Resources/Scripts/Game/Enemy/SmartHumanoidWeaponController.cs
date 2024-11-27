using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using Game.GlobalComponent;
using Game.Weapons;
using UnityEngine;

namespace Game.Enemy
{
	public class SmartHumanoidWeaponController : MonoBehaviour
	{
		public Weapon CurrentWeapon
		{
			get
			{
				return this.currentWeapon;
			}
		}

		public WeaponArchetype CurrentArchetype
		{
			get
			{
				return this.currentWeapon.Archetype;
			}
		}

		private void Update()
		{
			if (this.DebugLog)
			{
				UnityEngine.Debug.Log(this.CurrentWeapon.WeaponNameInList);
			}
		}

		public void Init(BaseNPC controlledNPC)
		{
			this.controllerOwner = controlledNPC.StatusNpc;
			this.usingWeapons = controlledNPC.SpecificNpcLinks.UsingWeapons;
			this.currentInitedWeapons = PoolManager.Instance.GetFromPool<WeaponForSmartHumanoidNPC>(this.usingWeapons);
			this.currentInitedWeapons.transform.parent = base.transform;
			this.currentInitedWeapons.transform.localPosition = Vector3.zero;
			this.currentInitedWeapons.transform.localEulerAngles = Vector3.zero;
			this.currentInitedWeapons.Init(controlledNPC, this);
			if (this.Weapons.Count == 1)
			{
				this.ChangeWeapon(this.Weapons[0]);
			}
			else if (!this.ActivateWeaponByType(WeaponArchetype.Ranged))
			{
				this.ActivateWeaponByType(WeaponArchetype.Melee);
			}
		}

		public void DeInit()
		{
			this.controllerOwner = null;
			this.DeInitWeapon();
			this.currentInitedWeapons.DeInit();
			PoolManager.Instance.ReturnToPool(this.currentInitedWeapons);
			this.currentInitedWeapons = null;
		}

		public void Attack(HitEntity entity)
		{
			this.attackTargetParameters.AttackMethod = AttackMethod.AttackEntity;
			this.attackTargetParameters.Victim = entity;
		}

		public void MeleeWeaponAttack(int attackState)
		{
			MeleeWeapon meleeWeapon = this.currentWeapon as MeleeWeapon;
			if (meleeWeapon != null)
			{
				meleeWeapon.MeleeAttack(attackState);
			}
		}

		public void AttackWithWeapon()
		{
			this.InvokedAttack();
		}

		public bool ActivateWeaponByType(WeaponArchetype newWeaponArchetype)
		{
			if (this.currentWeapon != null && this.CurrentArchetype == newWeaponArchetype)
			{
				return true;
			}
			if (this.Weapons.Count <= 1)
			{
				return false;
			}
			int num = -1;
			for (int i = 0; i < this.Weapons.Count; i++)
			{
				if (this.Weapons[i].Archetype == newWeaponArchetype)
				{
					num = i;
					RangedWeapon rangedWeapon = this.Weapons[i] as RangedWeapon;
					if (!(rangedWeapon != null))
					{
						break;
					}
					if (rangedWeapon.AmmoCount > 0)
					{
						break;
					}
					num = -1;
				}
			}
			if (num == -1)
			{
				return false;
			}
			this.ChangeWeapon(this.Weapons[num]);
			return true;
		}

		public bool CheckCurrentWeaponAmmoExist()
		{
			RangedWeapon rangedWeapon = this.currentWeapon as RangedWeapon;
			return !(rangedWeapon != null) || rangedWeapon.AmmoCount > 0;
		}

		public AttackState UpdateAttackState(bool attack)
		{
			this.attackState.MeleeAttackState = MeleeAttackState.None;
			this.attackState.RangedAttackState = RangedAttackState.None;
			this.attackState.RangedWeaponType = RangedWeaponType.None;
			this.attackState.MeleeWeaponType = MeleeWeapon.MeleeWeaponType.Hand;
			this.attackState.Aim = false;
			this.attackState.CanAttack = false;
			RangedWeapon rangedWeapon = this.currentWeapon as RangedWeapon;
			if (rangedWeapon)
			{
				this.attackState.RangedWeaponType = rangedWeapon.RangedWeaponType;
			}
			if (!attack)
			{
				return this.attackState;
			}
			MeleeWeapon meleeWeapon = this.currentWeapon as MeleeWeapon;
			if (rangedWeapon)
			{
				this.attackState.Aim = true;
				this.attackState.RangedWeaponType = rangedWeapon.RangedWeaponType;
				this.attackState.RangedAttackState = rangedWeapon.GetRangedAttackState();
				if (this.attackState.RangedAttackState == RangedAttackState.Shoot)
				{
					this.attackState.CanAttack = true;
				}
			}
			else if (meleeWeapon)
			{
				this.attackState.MeleeWeaponType = meleeWeapon.MeleeType;
				this.attackState.Aim = false;
				if (meleeWeapon.IsOnCooldown)
				{
					this.attackState.MeleeAttackState = MeleeAttackState.Idle;
				}
				else
				{
					this.attackState.MeleeAttackState = meleeWeapon.GetMeleeAttackState();
					if (this.attackState.MeleeAttackState != MeleeAttackState.None && this.attackState.MeleeAttackState != MeleeAttackState.Idle)
					{
						this.attackState.CanAttack = true;
					}
				}
			}
			return this.attackState;
		}

		public void HideWeapon()
		{
			if (this.currentWeapon)
			{
				this.currentWeapon.gameObject.SetActive(false);
			}
		}

		public void ShowWeapon()
		{
			if (this.currentWeapon)
			{
				this.currentWeapon.gameObject.SetActive(true);
			}
		}

		private void InvokedAttack()
		{
			if (this.currentWeapon)
			{
				this.currentWeapon.Attack(this.controllerOwner, this.attackTargetParameters.Victim);
			}
		}

		private void OnEnable()
		{
			this.CheckReloadOnWakeUp();
		}

		private void CheckReloadOnWakeUp()
		{
			RangedWeapon rangedWeapon = this.currentWeapon as RangedWeapon;
			if (rangedWeapon != null && rangedWeapon.AmmoInCartridgeCount == 0)
			{
				rangedWeapon.RechargeFinish();
			}
		}

		private void ChangeWeapon(Weapon newWeapon)
		{
			if (this.currentWeapon == newWeapon)
			{
				return;
			}
			this.DeInitWeapon();
			this.InitWeapon(newWeapon);
		}

		private void InitWeapon(Weapon newWeapon)
		{
			this.currentWeapon = newWeapon;
			this.currentWeapon.gameObject.SetActive(true);
			this.currentWeapon.Init();
			this.currentWeapon.PerformAttackEvent = new Weapon.AttackEvent(this.PerformAttackEvent);
			this.currentWeapon.InflictDamageEvent = new Weapon.DamageEvent(this.InflictDamageEvent);
			RangedWeapon rangedWeapon = this.currentWeapon as RangedWeapon;
			if (rangedWeapon != null)
			{
				rangedWeapon.RechargeStartedEvent = new Weapon.AttackEvent(this.RechargeStartedEvent);
			}
		}

		private void DeInitWeapon()
		{
			if (this.currentWeapon == null)
			{
				return;
			}
			this.currentWeapon.PerformAttackEvent = null;
			this.currentWeapon.InflictDamageEvent = null;
			this.currentWeapon.DeInit();
			RangedWeapon rangedWeapon = this.currentWeapon as RangedWeapon;
			if (rangedWeapon)
			{
				rangedWeapon.RechargeStartedEvent = null;
			}
			this.currentWeapon.gameObject.SetActive(false);
			this.currentWeapon = null;
		}

		private void PerformAttackEvent(Weapon weapon)
		{
			weapon.PlayAttackSound(this.WeaponAudio);
			RangedWeapon rangedWeapon = weapon as RangedWeapon;
			if (rangedWeapon != null)
			{
				this.MakeShotFlash(rangedWeapon.Muzzle, rangedWeapon.ShotSfx);
			}
		}

		private void MakeShotFlash(Transform muzzle, ShotSFXType type)
		{
			if (WeaponManager.Instance && muzzle)
			{
				WeaponManager.Instance.StartShootSFX(muzzle, type);
			}
		}

		private void InflictDamageEvent(Weapon weapon, HitEntity owner, HitEntity victim, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			victim.OnHit(weapon.WeaponDamageType, owner, weapon.Damage, hitPos, hitVector, defenceReduction);
		}

		private void RechargeStartedEvent(Weapon weapon)
		{
			RangedWeapon rangedWeapon = weapon as RangedWeapon;
			if (rangedWeapon)
			{
				rangedWeapon.PlayRechargeSound(this.WeaponAudio);
			}
		}

		public bool DebugLog;

		public AudioSource WeaponAudio;

		public List<Weapon> Weapons = new List<Weapon>();

		private HitEntity controllerOwner;

		private WeaponForSmartHumanoidNPC usingWeapons;

		private WeaponForSmartHumanoidNPC currentInitedWeapons;

		private Weapon currentWeapon;

		private readonly AttackTargetParameters attackTargetParameters = new AttackTargetParameters();

		private AttackState attackState = new AttackState();
	}
}
