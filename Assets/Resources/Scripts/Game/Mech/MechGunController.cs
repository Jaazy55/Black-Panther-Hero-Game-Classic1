using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Vehicle;
using Game.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Mech
{
	public class MechGunController : MonoBehaviour
	{
		public void Init(DrivableMech drivableMech, HitEntity driver)
		{
			this.currentMech = drivableMech;
			this.vehStatus = this.currentMech.GetComponentInChildren<VehicleStatus>();
			this.RechargeIndicator = MechControlPanel.Instance.ReloadIndicator;
			this.InitLauncher();
			this.InitRangedWeapons();
		}

		public void DeInit()
		{
			this.rangedWeapon1.DeInit();
			this.rangedWeapon2.DeInit();
			this.rangeWeaponProjectile.DeInit();
			PoolManager.Instance.ReturnToPool(this.rangedWeapon1);
			PoolManager.Instance.ReturnToPool(this.rangedWeapon2);
			PoolManager.Instance.ReturnToPool(this.rangeWeaponProjectile);
		}

		private void Awake()
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}

		private void Update()
		{
			this.Shooting();
		}

		private void WeaponAttackEvent(Weapon weapon)
		{
			PointSoundManager.Instance.PlayCustomClipAtPoint(this.rangeWeaponProjectile.transform.position, this.rangeWeaponProjectile.SoundAttack, null);
			WeaponManager.Instance.StartShootSFX(this.rangeWeaponProjectile.transform, this.rangeWeaponProjectile.ShotSfx);
			this.rangeWeaponProjectile.RechargeCall();
		}

		private void Shooting()
		{
			if (Controls.GetButton("Fire2"))
			{
				this.ShootFromRangeWeaponProjectile();
			}
			if (Controls.GetButton("Fire"))
			{
				this.ShootFromRangedWeapons();
			}
			this.UpdateRechargeIndicator();
		}

		private void UpdateRechargeIndicator()
		{
			if (this.RechargeIndicator)
			{
				this.RechargeIndicator.fillAmount = this.rangeWeaponProjectile.GetRechargeStatus();
			}
		}

		private void ShootFromRangeWeaponProjectile()
		{
			bool flag = this.rangeWeaponProjectile.Projectile.GetComponent<HomingProjectile>();
			Vector3 direction;
			CastResult castResult = TargetManager.Instance.ShootFromCamera(this.vehStatus.GetVehicleDriver(), this.rangeWeaponProjectile.ScatterVector, out direction, this.rangeWeaponProjectile.AttackDistance, this.rangeWeaponProjectile, false);
			if (flag)
			{
				this.HomingMissileLaunch(castResult.HitEntity, castResult.HitPosition);
			}
			else
			{
				this.rangeWeaponProjectile.Attack(this.vehStatus.GetVehicleDriver(), direction);
			}
		}

		private void ShootFromRangedWeapons()
		{
			Vector3 vector;
			TargetManager.Instance.ShootFromCamera(this.vehStatus.GetVehicleDriver(), this.rangedWeapon1.ScatterVector, out vector, this.rangedWeapon1.AttackDistance, this.rangedWeapon1, false);
			Vector3 vector2;
			TargetManager.Instance.ShootFromCamera(this.vehStatus.GetVehicleDriver(), this.rangedWeapon2.ScatterVector, out vector2, this.rangedWeapon2.AttackDistance, this.rangedWeapon2, false);
			if (this.DirectionPermittedForShooting(vector))
			{
				this.rangedWeapon1.Attack(this.vehStatus.GetVehicleDriver(), vector);
			}
			if (this.DirectionPermittedForShooting(vector2))
			{
				this.rangedWeapon2.Attack(this.vehStatus.GetVehicleDriver(), vector2);
			}
		}

		public bool DirectionPermittedForShooting(Vector3 shootingDirection)
		{
			if (!this.LimitedShotDirections)
			{
				return true;
			}
			bool flag = (double)Vector3.Dot(this.currentMech.transform.forward, shootingDirection.normalized) <= -0.2;
			float num = Mathf.Asin(Vector3.Cross(shootingDirection.normalized, this.currentMech.transform.forward).y) * 57.29578f;
			return flag && num >= (float)(-(float)this.PermittedAngleForShooting) && num <= (float)this.PermittedAngleForShooting;
		}

		private void HomingMissileLaunch(HitEntity hitEntity, Vector3 targetPosition)
		{
			if (hitEntity)
			{
				this.rangeWeaponProjectile.HomingAttack(this.vehStatus.GetVehicleDriver(), hitEntity);
			}
			else
			{
				this.rangeWeaponProjectile.HomingAttack(this.vehStatus.GetVehicleDriver(), targetPosition);
			}
		}

		private void InflictDamageEvent(Weapon weapon, HitEntity owner, HitEntity victim, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			victim.OnHit(DamageType.Bullet, owner, weapon.Damage, hitPos, hitVector, defenceReduction);
		}

		private void OverallWeaponAttackEvent(Weapon weapon)
		{
			weapon.PlayAttackSound(this.audioSource);
		}

		private void InitRangedWeapons()
		{
			this.rangedWeapon1 = PoolManager.Instance.GetFromPool<RangedWeapon>(this.RangedWeaponPrefab);
			this.rangedWeapon2 = PoolManager.Instance.GetFromPool<RangedWeapon>(this.RangedWeaponPrefab);
			GameObject lasergunPoint = this.currentMech.LasergunPoint1;
			GameObject lasergunPoint2 = this.currentMech.LasergunPoint2;
			this.rangedWeapon1.transform.parent = lasergunPoint.transform;
			this.rangedWeapon1.transform.localPosition = Vector3.zero;
			this.rangedWeapon1.transform.localRotation = Quaternion.identity;
			this.rangedWeapon2.transform.parent = lasergunPoint2.transform;
			this.rangedWeapon2.transform.localPosition = Vector3.zero;
			this.rangedWeapon2.transform.localRotation = Quaternion.identity;
			this.rangedWeapon1.Init();
			this.rangedWeapon2.Init();
			this.rangedWeapon1.SetWeaponOwner(this.vehStatus.GetVehicleDriver());
			this.rangedWeapon2.SetWeaponOwner(this.vehStatus.GetVehicleDriver());
			this.rangedWeapon1.InflictDamageEvent = new Weapon.DamageEvent(this.InflictDamageEvent);
			this.rangedWeapon2.InflictDamageEvent = new Weapon.DamageEvent(this.InflictDamageEvent);
			this.rangedWeapon1.PerformAttackEvent = new Weapon.AttackEvent(this.OverallWeaponAttackEvent);
			this.rangedWeapon1.SetScatterCalculateFromMuzzle();
			this.rangedWeapon2.SetScatterCalculateFromMuzzle();
		}

		private void InitLauncher()
		{
			this.rangeWeaponProjectile = PoolManager.Instance.GetFromPool<RangeWeaponProjectile>(this.LauncherPrefab).GetComponent<RangeWeaponProjectile>();
			this.rangeWeaponProjectile.transform.parent = this.currentMech.LauncherPoint1.transform.parent;
			this.rangeWeaponProjectile.transform.localPosition = Vector3.zero;
			this.rangeWeaponProjectile.transform.localEulerAngles = Vector3.zero;
			this.rangeWeaponProjectile.Muzzle = this.currentMech.LauncherPoint1.transform;
			this.rangeWeaponProjectile.Init();
			this.rangeWeaponProjectile.SetWeaponOwner(this.vehStatus.GetVehicleDriver());
			this.rangeWeaponProjectile.PerformAttackEvent = new Weapon.AttackEvent(this.WeaponAttackEvent);
		}

		private const string FirstWeaponShootStateName = "Fire";

		private const string SecondWeaponShootStateName = "Fire2";

		[Separator("Core")]
		public bool LimitedShotDirections;

		public int PermittedAngleForShooting = 25;

		public LayerMask WeaponLayerMask = -1;

		public LayerMask GroundLayerMask = -1;

		[Separator("Weapons")]
		public RangedWeapon RangedWeaponPrefab;

		public RangeWeaponProjectile LauncherPrefab;

		private AudioSource audioSource;

		private RangedWeapon rangedWeapon1;

		private RangedWeapon rangedWeapon2;

		private RangeWeaponProjectile rangeWeaponProjectile;

		private Image RechargeIndicator;

		private VehicleStatus vehStatus;

		private DrivableMech currentMech;
	}
}
