using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.Mech
{
	public class DrivableMech : DrivableVehicle
	{
		public override VehicleType GetVehicleType()
		{
			return VehicleType.Mech;
		}

		public MechGunController GunController
		{
			get
			{
				return this.gunControll;
			}
		}

		public override bool HasExitAnimation()
		{
			return false;
		}

		public override bool HasEnterAnimation()
		{
			return false;
		}

		public override void GetOut()
		{
			base.GetOut();
			this.FreezeRotation(false);
			if (this.gunControll != null)
			{
				PoolManager.Instance.ReturnToPool(this.gunControll);
				this.gunControll = null;
			}
		}

		public override void Drive(Player driver)
		{
			base.Drive(driver);
			this.FreezeRotation(true);
			if (this.GunControllerPrefab != null && this.gunControll == null)
			{
				this.gunControll = PoolManager.Instance.GetFromPool<MechGunController>(this.GunControllerPrefab);
				MechGunController gunControllerLocal = this.gunControll;
				PoolManager.Instance.AddBeforeReturnEvent(this.gunControll, delegate(GameObject poolingObject)
				{
					gunControllerLocal.DeInit();
				});
				this.gunControll.transform.parent = base.transform;
				this.gunControll.transform.localPosition = Vector3.zero;
				this.gunControll.transform.localEulerAngles = Vector3.zero;
				this.gunControll.Init(this, driver);
			}
		}

		public override void DeInit()
		{
			base.DeInit();
			base.MainRigidbody.ResetInertiaTensor();
		}

		public override void Init()
		{
			base.Init();
			base.MainRigidbody.ResetInertiaTensor();
			if (this.AnimationController == null)
			{
				this.AnimationController = base.GetComponent<MechAnimationController>();
			}
		}

		private void FreezeRotation(bool freeze = true)
		{
			if (freeze)
			{
				base.MainRigidbody.constraints = (RigidbodyConstraints)80;
			}
			else
			{
				base.MainRigidbody.freezeRotation = false;
			}
		}

		[Separator("Mech Links")]
		public MechGunController GunControllerPrefab;

		[HideInInspector]
		public MechAnimationController AnimationController;

		public GameObject mainTarget;

		public CapsuleCollider mainCollider;

		[Tooltip("Part which looking in camera.")]
		public GameObject rotatedPart;

		public Transform fixCameraTarget;

		[Separator("Gun Links")]
		public GameObject LasergunPoint1;

		public GameObject LasergunPoint2;

		public GameObject LauncherPoint1;

		public GameObject testObject;

		private MechGunController gunControll;
	}
}
