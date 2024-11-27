using System;
using Game.Character;
using Game.Character.Extras;
using Game.Character.Stats;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class CarController : VehicleController
	{
		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Car;
		}

		private void Awake()
		{
			if (CarController.chatacterLayerNumber == -1)
			{
				CarController.chatacterLayerNumber = LayerMask.NameToLayer("Character");
			}
			if (CarController.smallDynamicLayerNumber == -1)
			{
				CarController.smallDynamicLayerNumber = LayerMask.NameToLayer("SmallDynamic");
			}
			if (CarController.terrainLayerNumber == -1)
			{
				CarController.terrainLayerNumber = LayerMask.NameToLayer("Terrain");
			}
			if (CarController.complexStaticObjectLayerNumber == -1)
			{
				CarController.complexStaticObjectLayerNumber = LayerMask.NameToLayer("ComplexStaticObject");
			}
			if (CarController.staticObjectLayerNumber == -1)
			{
				CarController.staticObjectLayerNumber = LayerMask.NameToLayer("SimpleStaticObject");
			}
			if (CarController.bigDynamicLayerNumber == -1)
			{
				CarController.bigDynamicLayerNumber = LayerMask.NameToLayer("BigDynamic");
			}
			if (CarController.defaultLayerNumber == -1)
			{
				CarController.defaultLayerNumber = LayerMask.NameToLayer("Default");
			}
		}

		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			this.wheels = this.MainRigidbody.GetComponentsInChildren<WheelCollider>();
			SteeringWheels componentInChildren = this.MainRigidbody.GetComponentInChildren<SteeringWheels>();
			if (componentInChildren != null)
			{
				this.steerWheels = componentInChildren.Wheels;
			}
			foreach (WheelCollider wheelCollider in this.wheels)
			{
				wheelCollider.brakeTorque = 0f;
			}
			this.carSpecific = (this.VehicleSpecific as CarSpecific);
			this.vehicleStatus = drivableVehicle.GetVehicleStatus();
			if (this.carSpecific != null)
			{
				this.vehicleSound = drivableVehicle.SoundsPrefab;
				if (this.carSpecific.EngineSounds != null && this.carSpecific.EngineSounds.Length > 0)
				{
					this.vehicleSound.EngineSounds = this.carSpecific.EngineSounds;
				}
				if (this.carSpecific.GearShiftSound != null)
				{
					this.vehicleSound.GearShiftSound = this.carSpecific.GearShiftSound;
				}
				if (this.carSpecific.BrakeSound && this.BrakeAudioSource)
				{
					this.BrakeAudioSource.loop = true;
					this.BrakeAudioSource.clip = this.carSpecific.BrakeSound;
				}
				this.EngineAudioSource.pitch = 0.7f;
			}
			this.speedToNextGear = (this.CalcMaxSpeed(false) - this.CalcMaxSpeed(false) * 0.3f) / 5f;
			this.shiftGearTimeOut = this.CalcMaxSpeed(false) / (this.speedToNextGear * 6f);
			this.player = PlayerInteractionsManager.Instance.Player;
			this.inited = true;
		}

		public override void Animate(DrivableVehicle drivableVehicle)
		{
			base.Animate(drivableVehicle);
			this.carSpecific = (this.VehicleSpecific as CarSpecific);
			if (this.carSpecific != null && this.carSpecific.CarAnimator != null && drivableVehicle.AnimateGetInOut)
			{
				this.carSpecific.CarAnimator.SetBool("EnterInCar", true);
				this.carSpecific.CarAnimator.SetTrigger("LeftOpen");
			}
		}

		public override void DeInit(Action callbackAfterDeInit)
		{
			this.StopVehicle(false);
			if (this.EngineAudioSource)
			{
				this.EngineAudioSource.Stop();
			}
			this.IsInitialized = false;
			this.wheels = null;
			this.steerWheels = null;
			this.engineEnabled = true;
			if (this.carSpecific != null && this.carSpecific.CarAnimator != null && this.MainRigidbody.GetComponent<DrivableVehicle>().AnimateGetInOut)
			{
				this.carSpecific.CarAnimator.SetBool("EnterInCar", false);
				this.carSpecific.CarAnimator.SetTrigger("LeftOpen");
				this.deInitAction = callbackAfterDeInit;
				base.Invoke("DeferredDeInit", this.carSpecific.GetOutAnimationTime);
			}
			else
			{
				base.DeInit(callbackAfterDeInit);
			}
			this.inited = false;
		}

		private void DeferredDeInit()
		{
			base.DeInit(this.deInitAction);
		}

		private void Update()
		{
			if (!this.IsInitialized)
			{
				return;
			}
			float axis = Controls.GetAxis("Horizontal");
			if (this.steerWheels != null)
			{
				foreach (WheelCollider wheelCollider in this.steerWheels)
				{
					wheelCollider.steerAngle = axis * 30f;
				}
			}
			if (this.hitTimer > -1f)
			{
				this.hitTimer -= 2f * Time.deltaTime;
			}
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!this.IsInitialized)
			{
				return;
			}
			this.speed = this.CalcSpeedKmh();
			float num = Controls.GetAxis("Vertical");
			float axis = Controls.GetAxis("Horizontal");
			this.isBraking = ((float)((int)this.speed) * num < 0f);
			this.BrakeEffect();
			if (!this.engineEnabled)
			{
				num = 0f;
			}
			this.ShiftGears(num);
			this.ProceedEnginePitchSound(num);
			if (this.MainRigidbody != null)
			{
				bool isReverse = this.speed < 0f;
				if (Mathf.Abs(this.speed) < this.CalcMaxSpeed(isReverse) || (this.speed > 0f && num < 0f) || (this.speed < 0f && num > 0f))
				{
					if (this.OnGround())
					{
						this.MainRigidbody.AddRelativeForce(Vector3.forward * num * 800000f * Time.deltaTime * this.gearEffect * this.DrivableVehicle.Acceleration * this.player.stats.GetPlayerStat(StatsList.CarAcceleration));
					}
					if (this.wheels != null)
					{
						foreach (WheelCollider wheelCollider in this.wheels)
						{
							if (!this.isBraking)
							{
								wheelCollider.motorTorque = num * 30000f / (float)this.wheels.Length * Time.deltaTime;
								wheelCollider.brakeTorque = 0f;
							}
							else
							{
								wheelCollider.motorTorque = 0f;
								wheelCollider.brakeTorque = 800000f;
							}
						}
					}
				}
				float d = Mathf.Sign(this.speed);
				this.MainRigidbody.AddRelativeTorque(d * Vector3.up * axis * 1000f * Time.deltaTime);
				this.DrivableVehicle.ApplyStabilization(3000000f * Time.deltaTime);
				this.FixStuckState(this.speed, num);
			}
		}

		protected virtual void ProceedEnginePitchSound(float throttle)
		{
			if (this.MainRigidbody != null && this.EngineAudioSource != null)
			{
				this.averageSpeed = Mathf.Lerp(this.averageSpeed, this.speed, Time.deltaTime);
				float num;
				float num2;
				float num3;
				if (this.currGear <= 1)
				{
					num = Mathf.Abs(this.averageSpeed) / this.speedToNextGear + this.carSpecific.PitchOffset;
					num2 = 0.4f + this.carSpecific.PitchOffset;
					num3 = 1.5f + this.carSpecific.PitchOffset;
					num = num * num2 + num2;
				}
				else
				{
					num = this.averageSpeed / (this.speedToNextGear * (float)this.currGear) + this.carSpecific.PitchOffset;
					num2 = 0.45f + this.carSpecific.PitchOffset;
					num3 = 2f + this.carSpecific.PitchOffset;
				}
				bool flag = false;
				if (Mathf.Abs(throttle) < 0.1f)
				{
					this.reducePitch = true;
				}
				else
				{
					flag = this.reducePitch;
					this.ProceedEngineSound(this.currGear);
				}
				if (this.isBraking)
				{
					flag = false;
					this.reducePitch = true;
				}
				if (flag)
				{
					this.EngineAudioSource.pitch = Mathf.Clamp(Mathf.Lerp(this.EngineAudioSource.pitch, num, Time.deltaTime * 5f), num2, num3);
					if (Mathf.Abs(this.EngineAudioSource.pitch - num) <= 0.01f)
					{
						this.reducePitch = false;
					}
				}
				else if (this.reducePitch)
				{
					this.EngineAudioSource.pitch = Mathf.Lerp(this.EngineAudioSource.pitch, num2, Time.deltaTime * 5f);
				}
				else if (!this.OnGround())
				{
					this.EngineAudioSource.pitch = Mathf.Lerp(this.EngineAudioSource.pitch, num3 / 2f, Time.deltaTime * num3);
				}
				else
				{
					this.EngineAudioSource.pitch = Mathf.Clamp(num, num2, num3);
				}
			}
		}

		public bool OnGround()
		{
			bool result = true;
			for (int i = 0; i < this.wheels.Length; i++)
			{
				if (!this.wheels[i].isGrounded)
				{
					result = false;
					break;
				}
			}
			return result;
		}

		private void ProceedEngineSound(int id)
		{
			//if (!this.EngineAudioSource.clip.Equals(this.vehicleSound.EngineSounds[Mathf.Clamp(id, 0, this.vehicleSound.EngineSounds.Length - 1)]))
			{
				PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, this.vehicleSound.GearShiftSound, 1f);
				this.EngineAudioSource.clip = this.vehicleSound.EngineSounds[Mathf.Clamp(id, 0, this.vehicleSound.EngineSounds.Length - 1)];
			}
			if (!this.EngineAudioSource.isPlaying)
			{
				this.EngineAudioSource.Play();
			}
		}

		protected virtual void ShiftGears(float throttle)
		{
			this.shiftGearTimer -= Time.deltaTime;
			if (this.shiftGearTimer > 0f)
			{
				return;
			}
			if (Mathf.Abs(this.speed) < 0.1f)
			{
				return;
			}
			if (Mathf.Abs(throttle) < 0.2f)
			{
				return;
			}
			float f = 1f - ((float)this.currGear / 6f + (this.speed - (float)(this.currGear - 1) * this.speedToNextGear) / this.speedToNextGear) / 2f;
			if (this.speed <= 0f && this.currGear != 0)
			{
				this.currGear = 0;
				this.shiftGearTimer = this.shiftGearTimeOut;
				f = 1f;
			}
			if (this.speed > this.speedToNextGear && this.currGear < 6 && this.speed > (float)this.currGear * this.speedToNextGear)
			{
				this.currGear++;
				this.gearEffect = 0.5f;
				this.shiftGearTimer = this.shiftGearTimeOut;
			}
			else if (this.currGear > 1 && this.speed + this.speedToNextGear / 2f < (float)(this.currGear - 1) * this.speedToNextGear)
			{
				this.currGear--;
				this.shiftGearTimer = this.shiftGearTimeOut;
			}
			this.gearEffect = Mathf.Lerp(this.gearEffect, 1f, Mathf.Abs(f) * Time.deltaTime);
		}

		private float CalcMaxSpeed(bool isReverse)
		{
			float num = (!isReverse) ? this.DrivableVehicle.MaxSpeed : (this.DrivableVehicle.MaxSpeed / 2f);
			return num * this.player.stats.GetPlayerStat(StatsList.DrivingMaxSpeed);
		}

		private float CalcSpeedKmh()
		{
			return Vector3.Dot(this.MainRigidbody.velocity, this.MainRigidbody.transform.forward) * 3.6f;
		}

		public void OnHit(Vector3 point)
		{
			PointSoundManager.Instance.PlaySoundAtPoint(point, "Hit");
			SparksHitEffect.Instance.Emit(point);
		}

		public override void Particles(Collision collision)
		{
			Vector3 normal = collision.contacts[0].normal;
			Vector3 normalized = Vector3.Cross(Vector3.Cross(normal, collision.relativeVelocity), normal).normalized;
			float num = Mathf.Abs(Vector3.Dot(normalized, collision.relativeVelocity));
			float num2 = Vector3.Dot(normalized, collision.relativeVelocity.normalized);
			if (num > 3f && num2 > 0.7f && this.ScratchParticles != null)
			{
				Quaternion rotation = Quaternion.LookRotation(normalized + collision.relativeVelocity);
				GameObject fromPool = PoolManager.Instance.GetFromPool(this.ScratchParticles);
				fromPool.transform.position = collision.contacts[0].point;
				fromPool.transform.rotation = rotation;
				PoolManager.Instance.ReturnToPoolWithDelay(fromPool, 1f);
			}
		}

		protected virtual void BrakeEffect()
		{
			if (this.carSpecific == null || this.carSpecific.Taillights == null || this.carSpecific.Taillights.Length <= 0)
			{
				return;
			}
			if (this.BrakeAudioSource && this.isBraking != this.BrakeAudioSource.isPlaying)
			{
				if (Mathf.Abs(this.speed) > this.speedToNextGear && this.isBraking)
				{
					this.BrakeAudioSource.Play();
				}
				else
				{
					this.BrakeAudioSource.Stop();
				}
			}
			if (!this.OnGround())
			{
				this.BrakeAudioSource.Stop();
			}
			if (this.isBraking == this.carSpecific.Taillights[0].activeSelf)
			{
				return;
			}
			foreach (GameObject gameObject in this.carSpecific.Taillights)
			{
				gameObject.SetActive(this.isBraking);
			}
		}

		public override bool EnabledToExit()
		{
			int num = 0;
			foreach (WheelCollider wheelCollider in this.wheels)
			{
				if (wheelCollider.isGrounded)
				{
					num++;
				}
			}
			return num != 0;
		}

		public override void StopVehicle(bool inMoment = false)
		{
			if (inMoment)
			{
				this.MainRigidbody.velocity = Vector3.zero;
			}
			foreach (WheelCollider wheelCollider in this.wheels)
			{
				wheelCollider.brakeTorque = 100f;
			}
			this.vehicleStatus.MainCollider.material = this.standartFrictionMaterial;
		}

		protected override void Drowning()
		{
			if (!this.inited)
			{
				return;
			}
			base.Drowning();
			this.EngineAudioSource.Stop();
			if (this.wheels == null)
			{
				return;
			}
			foreach (WheelCollider wheelCollider in this.wheels)
			{
				wheelCollider.brakeTorque = 100f;
			}
		}

		private void FixStuckState(float speed, float throttel)
		{
		}

		public GameObject ScratchParticles;

		public float speed;

		public PhysicMaterial minFrictionMaterial;

		public PhysicMaterial standartFrictionMaterial;

		private const string SteerAxeName = "Horizontal";

		private const string ThrottleAxeName = "Vertical";

		private const int SteerMaxAngle = 30;

		private const int EngineTorque = 30000;

		private const int PullForce = 800000;

		private const int RotateForce = 1000;

		private const int StabilizationForce = 3000000;

		private const float IdleEnginePitch = 0.3f;

		private const int GearsCount = 6;

		private const float BrakeTorque = 100f;

		private const float normalizedImpactAngle = 0.7f;

		private const float minScratchVelocity = 3f;

		private const float lastGearOverallPercentage = 0.3f;

		private const float StartPitch = 0.7f;

		private const float relativeSpeedForStrongHit = 30f;

		private static int chatacterLayerNumber = -1;

		private static int smallDynamicLayerNumber = -1;

		private static int terrainLayerNumber = -1;

		private static int staticObjectLayerNumber = -1;

		private static int complexStaticObjectLayerNumber = -1;

		private static int bigDynamicLayerNumber = -1;

		private static int defaultLayerNumber = -1;

		private CarSpecific carSpecific;

		private VehicleStatus vehicleStatus;

		private WheelCollider[] wheels;

		private WheelCollider[] steerWheels;

		private float hitTimer = 1f;

		private Action deInitAction;

		private VehicleSound vehicleSound;

		private int currGear;

		protected float speedToNextGear;

		protected float gearEffect;

		private float shiftGearTimer;

		private float shiftGearTimeOut = 0.9f;

		private float averageSpeed;

		private bool reducePitch;

		private float timeForTrack = 1f;

		private float speedDiffSum;

		private float inputSum;

		private float lastSpeed;

		private float speedThrowDifference = 5f;

		private float inputThrowDifference = 3f;

		private bool stuck;

		protected bool isBraking;

		protected bool inited;
	}
}
