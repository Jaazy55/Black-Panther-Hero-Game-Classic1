using System;
using System.Collections;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class DrivableCar : DrivableVehicle
	{
		public override bool HasEnterAnimation()
		{
			return this.HasEnterAnim;
		}

		public override bool HasExitAnimation()
		{
			return this.HasExitAnim;
		}

		public override VehicleType GetVehicleType()
		{
			return VehicleType.Car;
		}

		public override void ApplyStabilization(float force)
		{
			if (!this.OnGround)
			{
				return;
			}
			Vector3 position = base.MainRigidbody.transform.position;
			if (this.VehiclePoints.CenterOfMass != null)
			{
				position = this.VehiclePoints.CenterOfMass.position;
			}
			float num = 0f;
			if (this.wheels != null)
			{
				foreach (WheelCollider wheelCollider in this.wheels)
				{
					Vector3 a;
					Quaternion quaternion;
					wheelCollider.GetWorldPose(out a, out quaternion);
					float num2 = (a - wheelCollider.transform.position).magnitude / wheelCollider.suspensionDistance;
					num += num2 * Vector3.Dot(position - wheelCollider.transform.position, base.MainRigidbody.transform.right);
				}
			}
			base.MainRigidbody.AddRelativeTorque(Vector3.forward * num * force);
		}

		public override void CheckLocationOnGround()
		{
			if (this.wheels != null)
			{
				bool onGround = false;
				for (int i = 0; i < this.wheels.Length; i++)
				{
					WheelCollider wheelCollider = this.wheels[i];
					if (wheelCollider.isGrounded)
					{
						onGround = true;
						break;
					}
				}
				this.OnGround = onGround;
			}
		}

		protected override void Awake()
		{
            HasExitAnim = false;

            base.Awake();
			this.steeringWheels = base.GetComponent<SteeringWheels>();
			if (this.steeringWheels == null)
			{
				UnityEngine.Debug.LogWarning("Drivable car without steeringWheels");
			}
			this.wheels = base.GetComponentsInChildren<WheelCollider>();
			if (this.wheels == null)
			{
				UnityEngine.Debug.LogError("Drivable car without wheels");
			}
		}

		public override void OpenVehicleDoor(VehicleDoor door, bool isGettingIn)
		{
			base.OpenVehicleDoor(door, isGettingIn);
			if (this.controller != null || this.vehicleSpecific != null)
			{
				return;
			}
			if (this.VehicleSpecificPrefab != null)
			{
				this.vehicleSpecific = PoolManager.Instance.GetFromPool<VehicleSpecific>(this.VehicleSpecificPrefab);
				this.whileDoorOpenLock = true;
				PoolManager.Instance.AddBeforeReturnEvent(this.vehicleSpecific, delegate(GameObject poolingObject)
				{
					this.whileDoorOpenLock = false;
					this.vehicleSpecific = null;
					if (this.SimpleModel != null && this.controller == null)
					{
						this.SimpleModel.SetActive(true);
					}
				});
				if (this.SimpleModel != null)
				{
					this.SimpleModel.SetActive(false);
				}
				GameObject gameObject = this.vehicleSpecific.gameObject;
				gameObject.transform.parent = base.MainRigidbody.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				CarSpecific carSpecific = this.vehicleSpecific as CarSpecific;
				if (carSpecific != null && carSpecific.CarAnimator != null)
				{
					carSpecific.CarAnimator.SetBool("EnterInCar", isGettingIn);
					string trigger = string.Empty;
					if (door == VehicleDoor.LeftDoor)
					{
						trigger = "LeftOpen";
					}
					else if (door == VehicleDoor.RightDoor)
					{
						trigger = "RightOpen";
					}
					carSpecific.CarAnimator.SetTrigger(trigger);
				}
				float timeDelay = (!(carSpecific != null)) ? 1f : carSpecific.GetOutAnimationTime;
				base.StartCoroutine(this.ReturnToPoolVehicleSpecific(timeDelay));
			}
		}

		public override void Drive(Player driver)
		{
			if (this.vehicleSpecific != null)
			{
				PoolManager.Instance.ReturnToPool(this.vehicleSpecific);
			}
			base.Drive(driver);
		}

		public override void DeInit()
		{
			if (this.vehicleSpecific != null)
			{
				PoolManager.Instance.ReturnToPool(this.vehicleSpecific);
			}
			base.DeInit();
		}

		public override bool IsAbleToEnter()
		{
			return !this.whileDoorOpenLock && base.IsAbleToEnter() && !this.DeepInWater;
		}

		private IEnumerator ReturnToPoolVehicleSpecific(float timeDelay)
		{
			yield return new WaitForSeconds(timeDelay * 0.5f);
			if (this.vehicleSpecific != null)
			{
				this.whileDoorOpenLock = false;
			}
			yield return new WaitForSeconds(timeDelay * 0.5f);
			if (this.vehicleSpecific != null)
			{
				PoolManager.Instance.ReturnToPool(this.vehicleSpecific);
			}
			yield break;
		}

		private SteeringWheels steeringWheels;

		private VehicleSpecific vehicleSpecific;

		private bool whileDoorOpenLock;

		public WheelCollider[] wheels;

		public bool HasEnterAnim = true;

		public bool HasExitAnim = false;
	}
}
