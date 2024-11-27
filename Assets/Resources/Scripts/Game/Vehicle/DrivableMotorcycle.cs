using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class DrivableMotorcycle : DrivableVehicle
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
			return VehicleType.Motorbike;
		}

		public override void Init()
		{
			if (this.vehStatus == null)
			{
				this.vehStatus = base.GetComponentInChildren<VehicleStatus>();
			}
			this.vehStatus.Initialization(true);
			base.ChangeBodyColor(this.BodyRenderers);
			PoolManager.Instance.AddBeforeReturnEvent(base.gameObject, delegate(GameObject poolingObject)
			{
				base.MainRigidbody.ResetInertiaTensor();
				this.ConstraintsSetup(false);
			});
			RigidbodyConstraints constraints = base.MainRigidbody.constraints;
			base.MainRigidbody.constraints = RigidbodyConstraints.None;
			base.ApplyCenterOfMass(this.VehiclePoints.CenterOfMass);
			base.MainRigidbody.constraints = constraints;
		}

		public override bool IsControlsPlayerAnimations()
		{
			return false;
		}

		public override bool HasExitAnimation()
		{
			return true;
		}

		public override bool HasEnterAnimation()
		{
			return true;
		}

		public override void StopVehicle()
		{
			WheelCollider[] componentsInChildren = base.GetComponentsInChildren<WheelCollider>();
			foreach (WheelCollider wheelCollider in componentsInChildren)
			{
				wheelCollider.motorTorque = 0f;
				wheelCollider.brakeTorque = 400f;
			}
			base.MainRigidbody.velocity = Vector3.zero;
		}

		public override void Drive(Player driver)
		{
			base.Drive(driver);
			base.ApplyCenterOfMass(this.VehiclePoints.CenterOfMass);
			this.ConstraintsSetup(true);
			if (this.BikeTrigger)
			{
				this.BikeTrigger.enabled = true;
				this.BikeTrigger.Init();
			}
		}

		public override void ConstraintsSetup(bool isIn)
		{
			base.MainRigidbody.constraints = ((!isIn) ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeRotationZ);
		}

		public override void SteerStabilization(float steer)
		{
			float num = base.MainRigidbody.transform.eulerAngles.z;
			float b = -steer * 0.5f;
			num = Mathf.LerpAngle(num, b, Time.deltaTime * 15f);
			base.MainRigidbody.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, num);
		}

		public override void ApplyStabilization(float force)
		{
		}

		public override Vector3 GetExitPosition(bool toLeft)
		{
			return toLeft ? this.VehiclePoints.EnterFromPositions[0].position : this.VehiclePoints.EnterFromPositions[1].position;
		}

		public override bool IsDoorBlockedOffset(LayerMask blockedLayerMask, Transform driver, out Vector3 offset, bool horizontalCheckOnly = true)
		{
			offset = Vector3.zero;
			Vector3 vector = base.transform.position + base.transform.up * this.VehicleSpecificPrefab.MaxHeight * 0.5f;
			bool flag = false;
			bool flag2 = false;
			RaycastHit raycastHit;
			if (Physics.Raycast(vector, -base.transform.right, out raycastHit, this.ExitLeftMinDistance, blockedLayerMask))
			{
				offset = this.VehiclePoints.EnterFromPositions[0].position - driver.position;
				flag = true;
			}
			if (Physics.Raycast(vector, base.transform.right, out raycastHit, this.ExitRightMinDistance, blockedLayerMask))
			{
				offset = this.VehiclePoints.EnterFromPositions[1].position - driver.position;
				flag2 = true;
			}
			UnityEngine.Debug.DrawRay(vector, -base.transform.right * this.ExitLeftMinDistance, Color.cyan, 5f);
			UnityEngine.Debug.DrawRay(vector, base.transform.right * this.ExitLeftMinDistance, Color.cyan, 5f);
			UnityEngine.Debug.DrawRay(driver.position + Vector3.up * 0.1f, Vector3.up * 2f, Color.yellow, 5f);
			if (flag2 && flag)
			{
				offset = Vector3.up * this.ExitUpMinDistance;
			}
			return flag2 && flag;
		}

		public override void SetDummyDriver(DummyDriver driver)
		{
			base.SetDummyDriver(driver);
			if (this.DummyDriver)
			{
				this.DummyDriver.DriverStatus.DamageEvent += this.OnDriverStatusDamageEvent;
				PoolManager.Instance.AddBeforeReturnEvent(driver, delegate(GameObject poolingObject)
				{
					this.DummyDriver.DriverStatus.DamageEvent -= this.OnDriverStatusDamageEvent;
					if (this.controller)
					{
						return;
					}
					this.ConstraintsSetup(false);
					base.MainRigidbody.ResetInertiaTensor();
				});
			}
		}

		public override void OnDriverStatusDamageEvent(float damage, HitEntity owner)
		{
			if (this.DummyDriver && owner)
			{
				this.DummyDriver.DropRagdoll(owner, -(owner.transform.position - this.DummyDriver.transform.position).normalized * damage * 0.003f, !this.DummyDriver.DriverDead, false, 0f);
				this.ConstraintsSetup(false);
				base.MainRigidbody.ResetInertiaTensor();
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

		public override bool IsAbleToEnter()
		{
			return !this.DeepInWater || !this.WaterSensor.InWater;
		}

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			if (this.controller)
			{
				((MotorcycleController)this.controller).DropFromSpeed(Vector3.Dot(col.relativeVelocity, col.contacts[0].normal));
			}
			float num = Vector3.Dot(col.relativeVelocity, col.contacts[0].normal);
			HitEntity disturber = null;
			VehicleStatus component = col.collider.GetComponent<VehicleStatus>();
			if (component != null)
			{
				disturber = component.GetVehicleDriver();
			}
			if (num > 5f && this.DummyDriver)
			{
				this.DummyDriver.DropRagdoll(disturber, col.contacts[0].normal, !this.DummyDriver.DriverDead, false, 0f);
				this.ConstraintsSetup(false);
				base.MainRigidbody.ResetInertiaTensor();
			}
		}

		private const float SpeedForDrop = 5f;

		private const float DamageForDropMultipler = 0.003f;

		public Transform DriverStatPoint;

		public BikeTrigger BikeTrigger;
	}
}
