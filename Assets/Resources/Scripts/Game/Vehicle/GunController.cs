using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Weapons;
using Game.Enemy;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vehicle
{
	public class GunController : MonoBehaviour
	{

		public void Init(DrivableHelicopter drivableHelicopter, HitEntity driver)
		{
			this.launcherPoints = drivableHelicopter.RocketsPoints;
			this.currentHelicopter = drivableHelicopter;
			this.helicopterController = (drivableHelicopter.controller as HelicopterController);
			this.vehStatus = drivableHelicopter.GetComponentInChildren<VehicleStatus>();
			//this.minigunInstance = PoolManager.Instance.GetFromPool<RangedWeapon>(this.MinigunPrefab);
			this.minigunInstance = GetComponent<RangedWeapon>();
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
			this.minigunInstance = GetComponent<RangedWeapon>();
		}

		private void FixedUpdate()
		{
			//	if (!this.helicopterController.InitializedGetter || this.helicopterController.IsGrounded)
			//	{
			//		return;
			//}
			//this.SetShootIndicator();
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
		public bool Enemy;
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
			if (Enemy == true)
			{
				this.ShootFromEnemyWeapon(this.minigunInstance, 45f, 70f);

			}
		}

		private void ShootFromWeapon(RangedWeapon weapon, float xMaxAngle, float yMaxAngle)
		{
			Vector3 direction;

			CastResult castResult = TargetManager.Instance.ShootFromCamera(weapon.WeaponOwner, weapon.ScatterVector, out direction, weapon.AttackDistance, weapon, false);
			//direction = this.currentHelicopter.transform.InverseTransformDirection(direction);
			direction = transform.InverseTransformDirection(direction);
			direction.x = Mathf.Clamp(direction.x, -xMaxAngle, xMaxAngle);
			direction.y = Mathf.Clamp(direction.y, -yMaxAngle, yMaxAngle);
			direction.z = Mathf.Abs(direction.z);
			//direction = this.currentHelicopter.transform.TransformDirection(direction);
			direction = transform.TransformDirection(direction);
			//weapon.Attack(this.vehStatus.GetVehicleDriver(), direction);

			weapon.Attack(transform.root.GetComponent<Player>(), direction);

		}
		private void ShootFromEnemy(RangedWeapon weapon, float xMaxAngle, float yMaxAngle)
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
		private void ShootFromEnemyWeapon(RangedWeapon weapon, float xMaxAngle, float yMaxAngle)
		{
			Vector3 direction2 = Vector3.forward;

			//direction2 = Quaternion.Euler(0, -45, 0) * direction2;

			//CastResult castResult = TargetManager.Instance.ShootAt(weapon.WeaponOwner, weapon.ScatterVector, weapon.AttackDistance + 15); 
			//ShootFromCamera(HitEntity owner, Vector3 scatterVector, out Vector3 shotDirVector, float maxShotDistance = 0f, RangedWeapon customWeapon = null, bool humanoidShoot = false)
			//CastResult castResult = TargetManager.Instance.ShootFromCamera(weapon.WeaponOwner, weapon.ScatterVector, out direction2, weapon.AttackDistance, weapon, true);
			//CastResult castResult2 = TargetManager.Instance.ShootAt(weapon.WeaponOwner, weapon.ScatterVector, weapon.AttackDistance + 15);
			//CastResult castResult2 = TargetManager.Instance.ShootAt(weapon.WeaponOwner, weapon.ScatterVector, weapon.AttackDistance + 15);
			//ShootFromAt(HitEntity owner, Vector3 fromPos, Vector3 direction, float maxShootDistance = 0f)
			CastResult castResult2 = TargetManager.Instance.ShootFromAt(weapon.WeaponOwner, weapon.WeaponOwner.transform.position, direction2, weapon.AttackDistance + 15);

			//direction2 = Quaternion.Euler(0, 10, -15) * direction2;
			direction2 = Quaternion.AngleAxis(-15, Vector3.right) * direction2;
			direction2 = Quaternion.AngleAxis(10, Vector3.up) * direction2;
			//direction = this.currentHelicopter.transform.InverseTransformDirection(direction);
			//direction2 = transform.InverseTransformDirection(direction2);
			direction2.x = Mathf.Clamp(direction2.x, -xMaxAngle, xMaxAngle);
			direction2.y = Mathf.Clamp(direction2.y, -yMaxAngle, yMaxAngle);
			direction2.z = Mathf.Abs(direction2.z);
			//direction = this.currentHelicopter.transform.TransformDirection(direction);
			direction2 = transform.TransformDirection(direction2);
			//weapon.Attack(this.vehStatus.GetVehicleDriver(), direction);
			//if (transform.root.GetComponent<HumanoidStatusNPC>());
			//// Check for a Wall.
			//LayerMask mask = LayerMask.GetMask("Wall");
			//TargetManager.Instance.ShootingLayerMask = mask;
			//Gizmos.color = Color.yellow;
			//Gizmos.DrawRay(transform.root.position, direction2);
			//direction = this.currentHelicopter.transform.TransformDirection(direction);GameObject.FindGameObjectWithTag("Player").transform.position
			try
			{
				weapon.Attack(transform.root.GetComponent<HumanoidStatusNPC>(), direction2); //, direction2
			}
			catch (Exception e)
			{
				Debug.LogError("Horrible things happened! : ");
			}
			//else
			//	Debug.LogError("No HmanoidStatusNPC found on Enemy");
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
