using System;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class DrivableTank : DrivableVehicle
	{
		public override VehicleType GetVehicleType()
		{
			return VehicleType.Tank;
		}

		public override void DeInit()
		{
			base.DeInit();
			if (this.gunControll != null)
			{
				PoolManager.Instance.ReturnToPool(this.gunControll);
				this.gunControll = null;
			}
		}

		public override void Drive(Player driver)
		{
			base.Drive(driver);
			if (this.GunController != null && this.gunControll == null)
			{
				TankGunController gunControllerLocal = this.gunControll = PoolManager.Instance.GetFromPool<TankGunController>(this.GunController);
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

		public override void GetOut()
		{
			base.GetOut();
			if (this.gunControll != null)
			{
				PoolManager.Instance.ReturnToPool(this.gunControll);
				this.gunControll = null;
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

		public override bool IsControlsPlayerAnimations()
		{
			return false;
		}

		protected override void OnCollisionSpecific(Collision col)
		{
			base.OnCollisionSpecific(col);
			BaseStatusNPC component = col.gameObject.GetComponent<BaseStatusNPC>();
			if (component != null)
			{
				component.OnHit(DamageType.Collision, this.CurrentDriver, 100f, col.contacts[0].point, col.contacts[0].normal, 0f);
			}
		}

		[Separator("Tank Links")]
		public TankGunController GunController;

		public Transform[] WheelTransformLeft;

		public Transform[] WheelTransformsRight;

		public WheelCollider[] WheelCollidersLeft;

		public WheelCollider[] WheelCollidersRight;

		public Transform[] UselessGearTransformsLeft;

		public Transform[] UselessGearTransformsRight;

		public Transform[] TrackBoneTransformsLeft;

		public Transform[] TrackBoneTransformsRight;

		public Renderer TrackObjectLeft;

		public Renderer TrackObjectRight;

		public GameObject ExhaustGasPoint;

		public GameObject Tower;

		public GameObject MainGun;

		public GameObject MainGunOut;

		private TankGunController gunControll;
	}
}
