using System;
using System.Collections;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.GlobalComponent;
using Game.UI;
using UnityEngine;

namespace Game.Vehicle
{
	public class MotorcycleController : CarController
	{
		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			this.motorcycleSpecific = (MotorcycleSpecific)this.VehicleSpecific;
			this.leanAngle = this.motorcycleSpecific.MaxLeanAngle;
			this.SwithDriverModel(true);
			this.helm = this.motorcycleSpecific.Helm;
			this.helmAngleMultiplier = this.motorcycleSpecific.HelmAngle / this.SteerAngle;
			this.helmStartRotation = this.helm.transform.localEulerAngles;
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
			this.FrontWheelController = this.FrontWheelCollider.GetComponent<SimpleWheelController>();
			this.RearWheelController = this.RearWheelCollider.GetComponent<SimpleWheelController>();
			this.FrontWheelController.WheelPoint = ((MotorcycleSpecific)this.VehicleSpecific).FrontWheelPoint.transform;
			this.FrontWheelController.ResetWheelCollider();
			this.RearWheelController.ResetWheelCollider();
			if (this.motorcycleSpecific.BrakeSound && this.BrakeAudioSource)
			{
				this.BrakeAudioSource.loop = true;
				this.BrakeAudioSource.clip = this.motorcycleSpecific.BrakeSound;
			}
			this.maxSpeed = drivableVehicle.MaxSpeed * this.player.stats.GetPlayerStat(StatsList.DrivingMaxSpeed);
			this.IsInitialized = true;
		}

		private void Start()
		{
			if (this.FrontWheelController == null)
			{
				this.FrontWheelController = this.FrontWheelCollider.GetComponent<SimpleWheelController>();
			}
			if (this.RearWheelController == null)
			{
				this.RearWheelController = this.RearWheelCollider.GetComponent<SimpleWheelController>();
			}
		}

		protected override void FixedUpdate()
		{
			this.Lean();
		}

		public void DropFromSpeed(float relativeSpeed)
		{
			if (Mathf.Abs(relativeSpeed) >= 15f || (!this.RearWheelCollider.isGrounded && !this.FrontWheelCollider.isGrounded))
			{
				this.DropFrom();
			}
		}

		public override void DropFrom()
		{
			if (PlayerInteractionsManager.Instance.IsPossibleGetOut() && base.gameObject.activeInHierarchy)
			{
				base.StartCoroutine(this.DropFromDelay());
			}
		}

		public IEnumerator DropFromDelay()
		{
			if (this.IsInitialized)
			{
				Transform transform = PlayerInteractionsManager.Instance.Player.transform;
				this.MainRigidbody.constraints = RigidbodyConstraints.None;
				transform.position += Vector3.up;
				transform.parent = null;
				PlayerInteractionsManager.Instance.ResetCharacterHipsParent();
				this.player.ReplaceOnRagdoll(true, this.MainRigidbody.velocity, false);
				this.MainRigidbody.constraints = RigidbodyConstraints.None;
				PlayerInteractionsManager.Instance.GetOutFromVehicle(true);
			}
			yield return new WaitForSeconds(0.05f);
			yield break;
		}

		public override void DeInit(Action callbackAfterDeInit)
		{
			this.SwithDriverModel(false);
			this.helm.transform.localEulerAngles = this.helmStartRotation;
			this.FrontWheelCollider.GetComponent<SimpleWheelController>().WheelPoint = null;
			this.MainRigidbody.centerOfMass = this.primordialCenterOfMass;
			this.MainRigidbody.ResetInertiaTensor();
			this.MotorInput = 0f;
			this.RearWheelCollider.motorTorque = 0f;
			this.FrontWheelCollider.brakeTorque = this.Brake;
			this.RearWheelCollider.brakeTorque = this.Brake;
			this.FrontWheelCollider = null;
			this.RearWheelCollider = null;
			base.DeInit(callbackAfterDeInit);
		}

		private void Braking()
		{
			if (Mathf.Abs(this.MotorInput) <= 0.05f)
			{
				this.isBraking = false;
				this.FrontWheelCollider.brakeTorque = this.Brake / 25f;
				this.RearWheelCollider.brakeTorque = this.Brake / 25f;
			}
			else if (this.MotorInput < 0f && !this.reversing)
			{
				this.isBraking = true;
				this.FrontWheelCollider.brakeTorque = this.Brake * (Mathf.Abs(this.MotorInput) / 5f);
				this.RearWheelCollider.brakeTorque = this.Brake * Mathf.Abs(this.MotorInput);
				if (this.FrontWheelCollider.isGrounded || this.RearWheelCollider.isGrounded)
				{
					this.MainRigidbody.AddForce(this.MotorInput * this.MainRigidbody.transform.forward * this.Brake);
				}
			}
			else
			{
				this.isBraking = false;
				this.FrontWheelCollider.brakeTorque = 0f;
				this.RearWheelCollider.brakeTorque = 0f;
			}
		}

		private IEnumerator SwithDriverModelDelay(bool isGetin, float time = 0f)
		{
			yield return new WaitForSeconds(time);
			this.SwithDriverModel(isGetin);
			yield break;
		}

		private void SwithDriverModel(bool isGetin)
		{
			if (isGetin)
			{
				foreach (MyCustomIK.Limb limb in this.motorcycleSpecific.HandsIKController.Limbs)
				{
					if (limb.LimbName == "RightArm")
					{
						limb.upperArm = PlayerManager.Instance.CurrentStuffHelper.RightUpperArm;
						limb.forearm = PlayerManager.Instance.CurrentStuffHelper.RightForeArm;
						limb.hand = PlayerManager.Instance.CurrentStuffHelper.RightHand;
					}
					if (limb.LimbName == "LeftArm")
					{
						limb.upperArm = PlayerManager.Instance.CurrentStuffHelper.LeftUpperArm;
						limb.forearm = PlayerManager.Instance.CurrentStuffHelper.LeftForeArm;
						limb.hand = PlayerManager.Instance.CurrentStuffHelper.LeftHand;
					}
				}
			}
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Motorbike;
		}

		private void Update()
		{
			this.Inputs();
			this.ProceedStamina();
			this.Drive();
			this.ShiftGears(this.MotorInput);
			this.ProceedEnginePitchSound(this.MotorInput);
			this.Braking();
			this.BrakeEffect();
		}

		public void ProceedStamina()
		{
			this.player.stats.stamina.DoFixedUpdate();
		}

		private void Inputs()
		{
			if (this.mainDrivableVehicle.DeepInWater && this.mainDrivableVehicle.WaterSensor.InWater)
			{
				this.MotorInput = 0f;
				this.EngineAudioSource.Stop();
				DangerIndicator.Instance.Activate("You are drowning.");
				RadioManager.Instance.DisableRadio();
				return;
			}
			this.leanInput = Controls.GetAxis("Lean");
			if (!this.crashed)
			{
				this.MotorInput = Controls.GetAxis("Vertical");
				float num = Time.deltaTime * 4f * (1f - Mathf.Abs(this.speed) / this.maxSpeed * 0.25f);
				if (Controls.GetAxis("Horizontal") == 0f)
				{
					num *= 5f;
				}
				this.steerInput = Mathf.Lerp(this.steerInput, Controls.GetAxis("Horizontal"), num);
			}
			else
			{
				this.MotorInput = 0f;
				this.steerInput = 0f;
			}
			if (this.MotorInput < 0f && base.transform.InverseTransformDirection(this.MainRigidbody.velocity).z < 0.1f)
			{
				this.reversing = true;
			}
			else
			{
				this.reversing = false;
			}
		}

		private void Drive()
		{
			this.FrontWheelController.ResetWheelCollider();
			this.RearWheelController.ResetWheelCollider();
			this.speed = this.CalcSpeedKmh();
			float num = this.SteerAngle * this.steerInput * (1f - Mathf.Abs(this.speed) / this.maxSpeed * 0.25f);
			this.FrontWheelCollider.steerAngle = num;
			this.helm.transform.localEulerAngles = this.helmStartRotation + new Vector3(0f, 0f, num * this.helmAngleMultiplier);
			if (this.speed > this.maxSpeed)
			{
				this.RearWheelCollider.motorTorque = 0f;
			}
			else if (!this.reversing)
			{
				float num2 = this.DrivableVehicle.Acceleration * this.player.stats.GetPlayerStat(StatsList.CarAcceleration);
				num2 = ((Controls.GetAxis("Vertical") <= 0f || (!this.FrontWheelCollider.isGrounded && !this.RearWheelCollider.isGrounded)) ? 0f : (num2 * Controls.GetAxis("Vertical")));
				this.RearWheelCollider.motorTorque = this.EngineTorque * Mathf.Clamp(this.MotorInput, 0f, 1f) * Mathf.Clamp(this.gearEffect, 0.1f, 2f);
				this.MainRigidbody.AddForce(this.MainRigidbody.transform.forward * 1000f * num2);
			}
			if (this.reversing)
			{
				if (this.speed < 10f)
				{
					this.RearWheelCollider.motorTorque = this.EngineTorque * this.MotorInput / 4f;
				}
				else
				{
					this.RearWheelCollider.motorTorque = 0f;
				}
			}
		}

		private void LateUpdate()
		{
			if ((this.FrontWheelCollider.isGrounded || this.RearWheelCollider.isGrounded) && this.MainRigidbody.transform.up.y > 0f)
			{
				float num = this.MainRigidbody.transform.eulerAngles.z;
				float b;
				if (this.speed > this.speedToNextGear)
				{
					b = -this.leanAngle * this.steerInput;
				}
				else
				{
					b = -this.leanAngle * this.steerInput * this.speed / this.maxSpeed;
				}
				num = Mathf.LerpAngle(num, b, Time.deltaTime * 4f);
				this.MainRigidbody.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, num);
			}
		}

		private void Lean()
		{
			if (Mathf.Abs(this.leanInput) > 0.5f)
			{
				float num;
				if (this.FrontWheelCollider.isGrounded || this.RearWheelCollider.isGrounded)
				{
					num = 2000f;
				}
				else
				{
					num = 5000f;
				}
				this.MainRigidbody.AddRelativeTorque(new Vector3(num * this.leanInput, 0f, 0f), ForceMode.Force);
			}
		}

		private float CalcSpeedKmh()
		{
			return Vector3.Dot(this.MainRigidbody.velocity, this.MainRigidbody.transform.forward) * 3.6f;
		}

		private const string SteerAxeName = "Horizontal";

		private const string ThrottleAxeName = "Vertical";

		private const string LeanAxisName = "Lean";

		private const float BikeMass = 500f;

		private const float DropSpeed = 15f;

		private const float ForcePopRagdoll = 4000f;

		private const float DropFromTimeOut = 0.05f;

		private const float DropFromOffsetRate = 2f;

		private const float StaminaPerSecond = 1f;

		private const float LeanRate = 4f;

		private const float SteerSpeedRate = 0.25f;

		public WheelCollider FrontWheelCollider;

		public WheelCollider RearWheelCollider;

		public float EngineTorque = 1500f;

		public float MaxEngineRPM = 6000f;

		public float MinEngineRPM = 1000f;

		public float SteerAngle = 40f;

		public float maxSpeed = 180f;

		public float Brake = 2500f;

		[HideInInspector]
		public float MotorInput;

		[HideInInspector]
		public float steerInput;

		[HideInInspector]
		public bool crashed;

		[HideInInspector]
		public bool reversing;

		[HideInInspector]
		public DrivableVehicle mainDrivableVehicle;

		private float leanInput;

		private GameObject helm;

		private Vector3 helmStartRotation;

		private MotorcycleSpecific motorcycleSpecific;

		private float helmAngleMultiplier = 1f;

		private float leanAngle;

		private SimpleWheelController FrontWheelController;

		private SimpleWheelController RearWheelController;
	}
}
