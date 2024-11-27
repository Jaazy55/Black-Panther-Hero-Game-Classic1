using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vehicle
{
	public class TankGunController : MonoBehaviour
	{
		public void Init(DrivableTank drivableTank, HitEntity driver)
		{
			this.currTankController = (drivableTank.controller as TankController);
			this.Tower = drivableTank.Tower;
			this.MainGun = drivableTank.MainGun;
			this.MainGunOut = drivableTank.MainGunOut;
			this.tankWeapon = PoolManager.Instance.GetFromPool(this.LauncherPrefab).GetComponent<RangeWeaponProjectile>();
			this.tankWeapon.transform.parent = this.MainGunOut.transform;
			this.tankWeapon.transform.localPosition = Vector3.zero;
			this.tankWeapon.transform.localEulerAngles = Vector3.zero;
			this.tankWeapon.Init();
			this.tankWeapon.SetWeaponOwner(driver);
			this.rigidBody = this.Tower.GetComponent<Rigidbody>();
			this.rigidBody.maxAngularVelocity = this.MaximumAngularVelocity;
			this.tankWeapon.PerformAttackEvent = new Weapon.AttackEvent(this.WeaponAttackEvent);
			this.joint = this.Tower.GetComponent<HingeJoint>();
			this.JointConfiguration();
		}

		public void DeInit()
		{
			this.tankWeapon.DeInit();
			this.MainGun.transform.eulerAngles = new Vector3(-this.MaxElevationLimit, this.MainGun.transform.eulerAngles.y, this.MainGun.transform.eulerAngles.z);
			PoolManager.Instance.ReturnToPool(this.tankWeapon.gameObject);
		}

		private void Update()
		{
			if (!this.currTankController.EngineEnabled)
			{
				return;
			}
			this.SetShootIndicator();
			this.Shooting();
		}

		private void FixedUpdate()
		{
			if (!this.currTankController.EngineEnabled)
			{
				return;
			}
			CastResult castResult = TargetManager.Instance.ShootFromCamera(this.tankWeapon.WeaponOwner, this.tankWeapon.ScatterVector, this.tankWeapon);
			this.MoveBarrel(castResult.HitPosition);
		}

		private void MoveBarrel(Vector3 targetWorldPosition)
		{
			Vector3 vector = this.Tower.transform.InverseTransformPoint(targetWorldPosition);
			this.inputSteer = vector.x / vector.magnitude;
			float num = (float)((this.inputSteer <= 0f) ? (-(float)this.RotationTorque) : this.RotationTorque);
			this.rigidBody.AddRelativeTorque(0f, num * Mathf.Abs(this.inputSteer), 0f, ForceMode.Force);
			Quaternion b = Quaternion.LookRotation(targetWorldPosition - this.MainGun.transform.position);
			this.MainGun.transform.rotation = Quaternion.Slerp(this.MainGun.transform.rotation, b, Time.deltaTime * (float)this.MainGunSlerpSpeed);
			if (this.MainGun.transform.localEulerAngles.x > 0f && this.MainGun.transform.localEulerAngles.x < 180f)
			{
				this.MainGun.transform.localEulerAngles = new Vector3(Mathf.Clamp(this.MainGun.transform.localEulerAngles.x, -this.MinElevationLimit, this.MinElevationLimit), 0f, 0f);
			}
			else if (this.MainGun.transform.localEulerAngles.x > 180f && this.MainGun.transform.localEulerAngles.x < 360f)
			{
				this.MainGun.transform.localEulerAngles = new Vector3(Mathf.Clamp(this.MainGun.transform.localEulerAngles.x - 360f, -this.MaxElevationLimit, this.MaxElevationLimit), 0f, 0f);
			}
		}

		private void Shooting()
		{
			if (Controls.GetButton("Fire"))
			{
				this.tankWeapon.Attack(null, this.tankWeapon.transform.forward);
			}
		}

		private void WeaponAttackEvent(Weapon weapon)
		{
			PointSoundManager.Instance.PlayCustomClipAtPoint(this.tankWeapon.transform.position, this.tankWeapon.SoundAttack, null);
			WeaponManager.Instance.StartShootSFX(this.tankWeapon.transform, this.tankWeapon.ShotSfx);
			this.rigidBody.AddForce(-this.Tower.transform.forward * (float)this.FirekickForce, ForceMode.VelocityChange);
			this.tankWeapon.RechargeCall();
		}

		private void JointConfiguration()
		{
			if (this.UseLimitsForRotation)
			{
				this.jointRotationLimit.min = (float)(-(float)this.MaximumRotaitonLimit);
				this.jointRotationLimit.max = (float)this.MaximumRotaitonLimit;
				this.joint.limits = this.jointRotationLimit;
			}
			else
			{
				this.joint.useLimits = false;
			}
		}

		private void SetShootIndicator()
		{
			if (TankControlPanel.Instance == null)
			{
				return;
			}
			if (!this.shootIndicator)
			{
				this.shootIndicator = TankControlPanel.Instance.ReloadIndicator;
			}
			this.shootIndicator.fillAmount = this.tankWeapon.GetRechargeStatus();
		}

		private const string ShootStateName = "Fire";

		private const int ShootEffectDestroyTime = 3;

		private const int Pi = 180;

		public GameObject Tower;

		public GameObject MainGun;

		public GameObject MainGunOut;

		public GameObject LauncherPrefab;

		public int FirekickForce = 500;

		public int RotationTorque = 100;

		public int MainGunSlerpSpeed = 5;

		public float MaximumAngularVelocity = 1f;

		public int MaximumRotaitonLimit = 160;

		public float MinElevationLimit = 10f;

		public float MaxElevationLimit = 25f;

		public bool UseLimitsForRotation = true;

		private Rigidbody rigidBody;

		private RangeWeaponProjectile tankWeapon;

		private HingeJoint joint;

		private JointLimits jointRotationLimit;

		private float inputSteer;

		private Image shootIndicator;

		private TankController currTankController;
	}
}
