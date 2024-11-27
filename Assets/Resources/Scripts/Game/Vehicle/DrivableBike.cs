using System;
using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Vehicle
{
	public class DrivableBike : DrivableVehicle
	{
		[HideInInspector]
		public Animator animator
		{
			get
			{
				return base.GetComponent<Animator>();
			}
		}

		public override VehicleType GetVehicleType()
		{
			return VehicleType.Bicycle;
		}

		public override void Init()
		{
		}

		public override bool IsControlsPlayerAnimations()
		{
			return true;
		}

		public override bool HasExitAnimation()
		{
			return false;
		}

		public override bool HasEnterAnimation()
		{
			return false;
		}

		public override void Drive(Player driver)
		{
			base.Drive(driver);
			if (this.BikeTrigger)
			{
				this.BikeTrigger.enabled = true;
				this.BikeTrigger.Init();
			}
		}

		public override void GetOut()
		{
			base.GetOut();
			if (this.BikeTrigger)
			{
				this.BikeTrigger.enabled = false;
			}
		}

		public override void OnCollisionEnter(Collision col)
		{
			if (PlayerInteractionsManager.Instance.inVehicle && this.controller)
			{
				((BikeController)this.controller).DropFromSpeed(Vector3.Dot(col.relativeVelocity, col.contacts[0].normal));
			}
		}

		public Transform metarig;

		public Transform DriverStatPoint;

		public BikeTrigger BikeTrigger;
	}
}
