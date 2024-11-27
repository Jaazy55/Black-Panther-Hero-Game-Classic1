using System;
using System.Collections;
using Game.Character;
using Game.Character.Stats;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class BikeController : VehicleController
	{
		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			this.MainRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
			this.MainRigidbody.mass = 500f;
			this.MainRigidbody.maxAngularVelocity = 2f;
			this.mainDrivableVehicle = drivableVehicle;
			WheelCollider[] componentsInChildren = this.MainRigidbody.GetComponentsInChildren<WheelCollider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].Equals(drivableVehicle.GetComponent<SteeringWheels>().Wheels[0]))
				{
					this.FrontWheelCollider = componentsInChildren[i];
				}
				else
				{
					this.RearWheelCollider = componentsInChildren[i];
				}
			}
			base.GetComponent<BikeAnimatorController>().Init(drivableVehicle.GetComponent<DrivableBike>().animator);
			this.maxSpeed = drivableVehicle.MaxSpeed * this.player.stats.GetPlayerStat(StatsList.DrivingMaxSpeed);
			this.IsInitialized = true;
		}

		public void DropFromSpeed(float relativeSpeed)
		{
			if (Mathf.Abs(relativeSpeed) >= 14.8f || (!this.RearWheelCollider.isGrounded && !this.FrontWheelCollider.isGrounded))
			{
				this.DropFrom();
			}
		}

		public override void DropFrom()
		{
			base.StartCoroutine(this.DropFromDelay());
		}

		public IEnumerator DropFromDelay()
		{
			this.crashed = true;
			yield return new WaitForSeconds(0.05f);
			if (this.IsInitialized)
			{
				bool flag = base.transform.up.y < 0.4f;
				Transform transform = PlayerInteractionsManager.Instance.Player.transform;
				transform.localPosition = Vector3.up * 2f;
				if (flag)
				{
					transform.position += Vector3.up * 2f;
				}
				transform.parent = null;
				PlayerInteractionsManager.Instance.SwitchSkeletons(false, false);
				this.player.ResetRotation();
				GameObject gameObject;
				this.player.ReplaceOnRagdoll(true, out gameObject, false);
				this.MainRigidbody.constraints = RigidbodyConstraints.None;
				gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 0f);
				PlayerInteractionsManager.Instance.GetOutFromVehicle(true);
			}
			yield break;
		}

		public override void DeInit(Action callbackAfterDeInit)
		{
			this.MainRigidbody.constraints = RigidbodyConstraints.None;
			this.MainRigidbody.centerOfMass = this.primordialCenterOfMass;
			this.MainRigidbody.ResetInertiaTensor();
			this.MotorInput = 0f;
			this.RearWheelCollider.motorTorque = 0f;
			this.FrontWheelCollider.brakeTorque = this.Brake;
			this.RearWheelCollider.brakeTorque = this.Brake;
			this.FrontWheelCollider = null;
			this.RearWheelCollider = null;
			base.GetComponent<BikeAnimatorController>().DeInit();
			base.DeInit(callbackAfterDeInit);
		}

		public void Braking()
		{
			if (Mathf.Abs(this.MotorInput) <= 0.05f)
			{
				this.brakingNow = false;
				this.FrontWheelCollider.brakeTorque = this.Brake / 25f;
				this.RearWheelCollider.brakeTorque = this.Brake / 25f;
			}
			else if (this.MotorInput < 0f && !this.reversing)
			{
				this.brakingNow = true;
				this.FrontWheelCollider.brakeTorque = this.Brake * (Mathf.Abs(this.MotorInput) / 5f);
				this.RearWheelCollider.brakeTorque = this.Brake * Mathf.Abs(this.MotorInput);
			}
			else
			{
				this.brakingNow = false;
				this.FrontWheelCollider.brakeTorque = 0f;
				this.RearWheelCollider.brakeTorque = 0f;
			}
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Bicycle;
		}

		private void Update()
		{
			this.Inputs();
			this.ProceedStamina();
			this.Lean();
			this.Drive();
			this.Braking();
		}

		public void ProceedStamina()
		{
			if (this.MotorInput <= 0f)
			{
				this.player.stats.stamina.DoFixedUpdate();
			}
			else
			{
				this.player.stats.stamina.SetAmount(-1f * Time.deltaTime);
			}
			if (this.player.stats.stamina.Current <= 10f && this.MotorInput > 0f)
			{
				this.MotorInput = 0f;
			}
		}

		private void Inputs()
		{
			this.leanInput = Controls.GetAxis("Lean");
			if (Controls.GetButtonDown("Jump"))
			{
				this.Jump();
			}
			if (!this.crashed)
			{
				this.MotorInput = Controls.GetAxis("Vertical");
				this.steerInput = Controls.GetAxis("Horizontal");
			}
			else
			{
				this.MotorInput = 0f;
				this.steerInput = 0f;
			}
			if (this.MotorInput < 0f && base.transform.InverseTransformDirection(this.MainRigidbody.velocity).z < 0f)
			{
				this.reversing = true;
			}
			else
			{
				this.reversing = false;
			}
		}

		private void Jump()
		{
			WheelHit wheelHit;
			if (this.RearWheelCollider.GetGroundHit(out wheelHit))
			{
				this.player.stats.stamina.SetAmount(-3f);
				this.MainRigidbody.AddForce(Vector3.up * 160000f);
			}
		}

		private void Drive()
		{
			this.Speed = this.CalcSpeedKmh();
			this.FrontWheelCollider.steerAngle = this.SteerAngle * this.steerInput;
			if (this.Speed > this.maxSpeed)
			{
				this.RearWheelCollider.motorTorque = 0f;
			}
			else if (!this.reversing)
			{
				this.RearWheelCollider.motorTorque = this.EngineTorque * Mathf.Clamp(this.MotorInput, 0f, 1f);
			}
			if (this.reversing)
			{
				if (this.Speed < 10f)
				{
					this.RearWheelCollider.motorTorque = this.EngineTorque * this.MotorInput / 5f;
				}
				else
				{
					this.RearWheelCollider.motorTorque = 0f;
				}
			}
		}

		private void LateUpdate()
		{
			if (this.FrontWheelCollider.isGrounded || this.RearWheelCollider.isGrounded)
			{
				this.MainRigidbody.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f);
			}
		}

		private void Lean()
		{
			if (Mathf.Abs(this.leanInput) > 0.3f && this.MotorInput > 0f)
			{
				this.MainRigidbody.AddRelativeTorque(new Vector3(5000f * this.leanInput, 0f, 0f));
			}
		}

		protected void ShiftGears()
		{
		}

		private float CalcSpeedKmh()
		{
			return Vector3.Dot(this.MainRigidbody.velocity, this.MainRigidbody.transform.forward) * 3.6f;
		}

		private const string SteerAxeName = "Horizontal";

		private const string ThrottleAxeName = "Vertical";

		private const string LeanAxisName = "Lean";

		private const string JumpAxisName = "Jump";

		private const float BikeMass = 500f;

		private const float DropSpeed = 14.8f;

		private const float ForcePopRagdoll = 400f;

		private const float DropFromTimeOut = 0.05f;

		private const float DropFromOffsetRate = 2f;

		private const float StaminaPerSecond = 1f;

		private const float StaminaPerJump = 3f;

		private const float jumpForce = 160000f;

		private const float SteerSpeedRate = 0.1f;

		public WheelCollider FrontWheelCollider;

		public WheelCollider RearWheelCollider;

		public float EngineTorque = 1500f;

		public float MaxEngineRPM = 6000f;

		public float MinEngineRPM = 1000f;

		public float SteerAngle = 40f;

		[HideInInspector]
		public float Speed;

		public float maxSpeed = 180f;

		public float Brake = 2500f;

		[HideInInspector]
		public float MotorInput;

		[HideInInspector]
		public bool brakingNow;

		[HideInInspector]
		public float steerInput;

		[HideInInspector]
		public bool crashed;

		[HideInInspector]
		public bool reversing;

		[HideInInspector]
		public DrivableVehicle mainDrivableVehicle;

		private float leanInput;
	}
}
