using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vehicle
{
	public class HelicopterGunController : MonoBehaviour
	{
		public void Init(DrivableHelicopter drivableHelicopter, HitEntity driver)
		{
			this.launcherPoints = drivableHelicopter.RocketsPoints;
			this.currentHelicopter = drivableHelicopter;
			this.helicopterController = (drivableHelicopter.controller as HelicopterController);
			this.vehStatus = drivableHelicopter.GetComponentInChildren<VehicleStatus>();
			this.minigunInstance = PoolManager.Instance.GetFromPool<RangedWeapon>(this.MinigunPrefab);
			GameObject minigunPoint = drivableHelicopter.MinigunPoint;
			this.minigunInstance.transform.position = minigunPoint.transform.position;
			this.minigunInstance.transform.parent = minigunPoint.transform;
			this.minigunInstance.Muzzle = minigunPoint.transform;
			this.minigunInstance.Init();
			this.minigunInstance.SetWeaponOwner(driver);
			this.minigunInstance.InflictDamageEvent = new Weapon.DamageEvent(this.InflictDamageEvent);
			this.minigunInstance.PerformAttackEvent = new Weapon.AttackEvent(this.OverallWeaponAttackEvent);
			this.minigunInstance.SetScatterCalculateFromMuzzle();
			this.launcherInstance = PoolManager.Instance.GetFromPool<RangeWeaponProjectile>(this.LauncherPrefab);
			this.launcherInstance.transform.position = this.launcherPoints[0].transform.position;
			this.launcherInstance.transform.parent = this.launcherPoints[0].transform;
			this.launcherInstance.Muzzle = this.launcherPoints[0].transform;
			this.launcherInstance.Init();
			this.launcherInstance.SetWeaponOwner(driver);
			this.launcherInstance.PerformAttackEvent = new Weapon.AttackEvent(this.LauncherAttackEvent);
			RangeWeaponProjectile rangeWeaponProjectile = this.launcherInstance;
			rangeWeaponProjectile.PerformAttackEvent = (Weapon.AttackEvent)Delegate.Combine(rangeWeaponProjectile.PerformAttackEvent, new Weapon.AttackEvent(this.OverallWeaponAttackEvent));
			this.launcherInstance.SetScatterCalculateFromMuzzle();
		}

		public void DeInit()
		{
			this.minigunInstance.DeInit();
			this.launcherInstance.DeInit();
			PoolManager.Instance.ReturnToPool(this.minigunInstance);
			PoolManager.Instance.ReturnToPool(this.launcherInstance);
		}

		private void Awake()
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}

		private void FixedUpdate()
		{
			if (!this.helicopterController.InitializedGetter || this.helicopterController.IsGrounded)
			{
				return;
			}
			this.SetShootIndicator();
			this.Shooting();
		}

		private void InflictDamageEvent(Weapon weapon, HitEntity owner, HitEntity victim, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			victim.OnHit(DamageType.Bullet, owner, weapon.Damage, hitPos, hitVector, defenceReduction);
		}

		private void OverallWeaponAttackEvent(Weapon weapon)
		{
			weapon.PlayAttackSound(this.audioSource);
		}

		private void LauncherAttackEvent(Weapon weapon)
		{
			this.currentLauncherPointIndex++;
			if (this.currentLauncherPointIndex >= this.launcherPoints.Length)
			{
				this.currentLauncherPointIndex = 0;
			}
		}

		private void SetShootIndicator()
		{
			if (!this.shootIndicator)
			{
				this.shootIndicator = HelicopterControlPanel.Instance.ReloadIndicator;
			}
			this.shootIndicator.fillAmount = this.launcherInstance.GetRechargeStatus();
		}

		private void Shooting()
		{
			if (Controls.GetButton("Fire"))
			{
				this.ShootFromWeapon(this.minigunInstance, 45f, 70f);
			}
			if (Controls.GetButton("Fire2"))
			{
				this.launcherInstance.Muzzle = this.launcherPoints[this.currentLauncherPointIndex].transform;
				this.ShootFromWeapon(this.launcherInstance, 10f, 70f);
			}
		}

		private void ShootFromWeapon(RangedWeapon weapon, float xMaxAngle, float yMaxAngle)
		{
			Vector3 direction;
			CastResult castResult = TargetManager.Instance.ShootFromCamera(weapon.WeaponOwner, weapon.ScatterVector, out direction, weapon.AttackDistance, weapon, false);
			direction = this.currentHelicopter.transform.InverseTransformDirection(direction);
			direction.x = Mathf.Clamp(direction.x, -xMaxAngle, xMaxAngle);
			direction.y = Mathf.Clamp(direction.y, -yMaxAngle, yMaxAngle);
			direction.z = Mathf.Abs(direction.z);
			direction = this.currentHelicopter.transform.TransformDirection(direction);
			weapon.Attack(this.vehStatus.GetVehicleDriver(), direction);
		}

		private const string MinigunShootStateName = "Fire";

		private const string LauncherShootStateName = "Fire2";

		private const int MinigunXMaxAngle = 45;

		private const int MinigunYMaxAngle = 70;

		private const int LauncherXMaxAngle = 10;

		private const int LauncherYMaxAngle = 70;

		public RangedWeapon MinigunPrefab;

		public RangeWeaponProjectile LauncherPrefab;

		private GameObject[] launcherPoints;

		private int currentLauncherPointIndex;

		private RangedWeapon minigunInstance;

		private RangeWeaponProjectile launcherInstance;

		private DrivableHelicopter currentHelicopter;

		private HelicopterController helicopterController;

		private VehicleStatus vehStatus;

		private Image shootIndicator;

		private AudioSource audioSource;
	}
}
