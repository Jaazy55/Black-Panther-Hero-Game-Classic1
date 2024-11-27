using System;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Vehicle
{
	public class DrivableCarForTransformer : DrivableCar
	{
		public override bool IsAbleToEnter()
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

		public override bool IsControlsPlayerAnimations()
		{
			return true;
		}

		public override void Init()
		{
			if (this.VehicleControllerPrefab == null)
			{
				throw new Exception(string.Format("{0} DrivableVehicle is missing VehicleControllerPrefab", base.gameObject.name));
			}
			if (this.vehStatus == null)
			{
				this.vehStatus = base.GetComponentInChildren<VehicleStatus>();
			}
			this.vehStatus.Initialization(false);
			base.ApplyCenterOfMass(this.VehiclePoints.CenterOfMass);
			base.ChangeBodyColor(this.BodyRenderers);
			if (this.WaterSensor)
			{
				this.WaterSensor.Init();
			}
		}

		protected override void FixedUpdate()
		{
			if (this.CurrentDriver && this.CurrentDriver.IsPlayer && -14f > base.transform.position.y + this.VehicleSpecificPrefab.MaxHeight / 2f)
			{
				this.CurrentDriver.OnHit(DamageType.Water, null, 10f * Time.deltaTime, Vector3.zero, Vector3.zero, 0f);
			}
			base.FixedUpdate();
		}

		private const float DamagePerDrow = 10f;

		private const float waterDepth = -14f;
	}
}
