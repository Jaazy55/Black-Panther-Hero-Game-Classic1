using System;
using System.Collections.Generic;
using System.Linq;
using Game.Character.Stats;
using Game.GlobalComponent;
using Game.Managers;
using UnityEngine;

namespace Game.Vehicle
{
	public class TankController : VehicleController
	{
		public bool EngineEnabled
		{
			get
			{
				return this.engineEnabled;
			}
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Tank;
		}

		public override void Init(DrivableVehicle drivableVehicle)
		{
			this.engineEnabled = true;
			base.Init(drivableVehicle);
			this.currentVolume = SoundManager.instance.GetSoundValue();
			SoundManager.ValueChanged b = delegate(float value)
			{
				this.currentVolume = SoundManager.instance.GetSoundValue();
				this.currentVolume *= value;
			};
			SoundManager instance = SoundManager.instance;
			instance.GameSoundChanged = (SoundManager.ValueChanged)Delegate.Combine(instance.GameSoundChanged, b);
			this.SoundInit(this.EngineRunningAudioSource, this.EngineRunningAudioClip);
			DrivableTank drivableTank = drivableVehicle as DrivableTank;
			if (drivableTank != null)
			{
				this.WheelTransformLeft = drivableTank.WheelTransformLeft;
				this.WheelTransformsRight = drivableTank.WheelTransformsRight;
				this.WheelCollidersLeft = drivableTank.WheelCollidersLeft;
				this.WheelCollidersRight = drivableTank.WheelCollidersRight;
				this.UselessGearTransformsLeft = drivableTank.UselessGearTransformsLeft;
				this.UselessGearTransformsRight = drivableTank.UselessGearTransformsRight;
				this.TrackBoneTransformsLeft = drivableTank.TrackBoneTransformsLeft;
				this.TrackBoneTransformsRight = drivableTank.TrackBoneTransformsRight;
				this.TrackObjectLeft = drivableTank.TrackObjectLeft;
				this.TrackObjectRight = drivableTank.TrackObjectRight;
				this.ExhaustGasPoint = drivableTank.ExhaustGasPoint;
			}
			this.MaxSpeed = drivableVehicle.MaxSpeed * this.player.stats.GetPlayerStat(StatsList.DrivingMaxSpeed);
			this.SmokeInit();
			this.MainRigidbody.maxAngularVelocity = 5f;
			this.rotationValueLeft = new float[this.WheelCollidersLeft.Length];
			this.rotationValueRight = new float[this.WheelCollidersRight.Length];
		}

		public override void DeInit(Action callbackAfterDeInit)
		{
			this.engineEnabled = false;
			base.DeInit(callbackAfterDeInit);
			this.EngineRunningAudioSource.Stop();
			this.SmokeDeinit();
			foreach (WheelCollider wheelCollider in this.allWheelColliders)
			{
				wheelCollider.brakeTorque = this.BrakeTorque * 10f;
			}
		}

		private void Update()
		{
			if (!this.IsInitialized)
			{
				return;
			}
			this.WheelsAlign();
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!this.IsInitialized || !this.EngineEnabled)
			{
				return;
			}
			this.AnimateGears(this.UselessGearTransformsRight, this.WheelCollidersRight, this.rotationValueRight);
			this.AnimateGears(this.UselessGearTransformsLeft, this.WheelCollidersLeft, this.rotationValueLeft);
			this.Engine();
			this.Braking();
			this.Inputs();
			this.AudioSetup();
			this.SmokeRate();
		}

		private void SoundInit(AudioSource source, AudioClip clip)
		{
			source.clip = clip;
			source.loop = true;
			source.volume = this.currentVolume;
			source.Play();
		}

		private void SmokeInit()
		{
			//if (this.ExhaustGasPoint != null)
			//{
			//	this.initedHeavyExhaustGas = PoolManager.Instance.GetFromPool<ParticleEmitter>(this.HeavyExhaustGas).GetComponent<ParticleEmitter>();
			//	this.initedHeavyExhaustGas.transform.parent = this.ExhaustGasPoint.transform;
			//	this.initedHeavyExhaustGas.transform.localPosition = Vector3.zero;
			//	this.initedHeavyExhaustGas.transform.localEulerAngles = Vector3.zero;
			//	this.initedNormalExhaustGas = PoolManager.Instance.GetFromPool<ParticleEmitter>(this.NormalExhaustGas).GetComponent<ParticleEmitter>();
			//	this.initedNormalExhaustGas.transform.parent = this.ExhaustGasPoint.transform;
			//	this.initedNormalExhaustGas.transform.localPosition = Vector3.zero;
			//	this.initedNormalExhaustGas.transform.localEulerAngles = Vector3.zero;
			//}
			this.allWheelColliders = this.WheelCollidersLeft.ToList<WheelCollider>();
			foreach (WheelCollider item in this.WheelCollidersRight)
			{
				this.allWheelColliders.Add(item);
			}
			if (!this.WheelSlipPrefab)
			{
				return;
			}
			foreach (WheelCollider wheelCollider in this.allWheelColliders)
			{
				GameObject fromPool = PoolManager.Instance.GetFromPool(this.WheelSlipPrefab);
				fromPool.transform.position = wheelCollider.transform.position;
				fromPool.transform.rotation = this.MainRigidbody.transform.rotation;
				fromPool.transform.parent = wheelCollider.transform;
				//this.wheelParticles.Add(fromPool.GetComponent<ParticleEmitter>());
			}
		}

		private void SmokeDeinit()
		{
			//if (this.initedHeavyExhaustGas)
			//{
			//	PoolManager.Instance.ReturnToPool(this.initedHeavyExhaustGas);
			//}
			//if (this.initedNormalExhaustGas)
			//{
			//	PoolManager.Instance.ReturnToPool(this.initedNormalExhaustGas);
			//}
			//foreach (ParticleEmitter particleEmitter in this.wheelParticles)
			//{
			//	PoolManager.Instance.ReturnToPool(particleEmitter.gameObject);
			//}
		}

		private void WheelsAlign()
		{
			this.CalculateWheelsTransform(this.WheelCollidersRight, this.WheelTransformsRight, this.TrackBoneTransformsRight, this.rotationValueRight);
			this.CalculateWheelsTransform(this.WheelCollidersLeft, this.WheelTransformLeft, this.TrackBoneTransformsLeft, this.rotationValueLeft);
			this.CalculateTextureOffset(this.TrackObjectLeft, this.WheelCollidersLeft, this.rotationValueLeft);
			this.CalculateTextureOffset(this.TrackObjectRight, this.WheelCollidersRight, this.rotationValueRight);
		}

		private void CalculateWheelsTransform(WheelCollider[] collidersArray, Transform[] transformsArray, Transform[] trackBoneTransforms, float[] rotationsArray)
		{
			int num = 0;
			while (num < collidersArray.Length && num < transformsArray.Length)
			{
				WheelCollider wheelCollider = collidersArray[num];
				Transform transform = transformsArray[num];
				Transform transform2 = trackBoneTransforms[num];
				Vector3 vector = wheelCollider.transform.TransformPoint(wheelCollider.center);
				RaycastHit raycastHit;
				if (Physics.Raycast(vector, -wheelCollider.transform.up, out raycastHit, (wheelCollider.suspensionDistance + wheelCollider.radius) * this.MainRigidbody.transform.localScale.y))
				{
					transform.position = raycastHit.point + wheelCollider.transform.up * wheelCollider.radius * this.MainRigidbody.transform.localScale.y;
					transform2.position = raycastHit.point + wheelCollider.transform.up * this.TrackOffset * this.MainRigidbody.transform.localScale.y;
				}
				else
				{
					transform.position = vector - wheelCollider.transform.up * wheelCollider.suspensionDistance * this.MainRigidbody.transform.localScale.y;
					transform2.position = vector - wheelCollider.transform.up * (wheelCollider.suspensionDistance + wheelCollider.radius - this.TrackOffset) * this.MainRigidbody.transform.localScale.y;
				}
				transform.rotation = wheelCollider.transform.rotation * Quaternion.Euler(rotationsArray[Mathf.CeilToInt((float)(collidersArray.Length / 2))], 0f, 0f);
				rotationsArray[num] += wheelCollider.rpm * 6f * Time.deltaTime;
				num++;
			}
		}

		private void CalculateTextureOffset(Renderer trackRenderer, WheelCollider[] collidersArray, float[] rotationsArray)
		{
			trackRenderer.material.SetTextureOffset("_MainTex", new Vector2(rotationsArray[Mathf.CeilToInt((float)(collidersArray.Length / 2))] / 1000f * this.TrackScrollSpeedMultipler, 0f));
			trackRenderer.material.SetTextureOffset("_BumpMap", new Vector2(rotationsArray[Mathf.CeilToInt((float)(collidersArray.Length / 2))] / 1000f * this.TrackScrollSpeedMultipler, 0f));
		}

		private void AnimateGears(Transform[] uselessGearArray, WheelCollider[] collidersArray, float[] rotationsArray)
		{
			for (int i = 0; i < uselessGearArray.Length; i++)
			{
				uselessGearArray[i].rotation = collidersArray[i].transform.rotation * Quaternion.Euler(rotationsArray[Mathf.CeilToInt((float)(collidersArray.Length / 2))], collidersArray[i].steerAngle, 0f);
			}
		}

		private void Engine()
		{
			if (this.speed > this.MaxSpeed)
			{
				for (int i = 0; i < this.allWheelColliders.Count; i++)
				{
					this.allWheelColliders[i].motorTorque = 0f;
				}
			}
			else
			{
				this.ApplyingMotorTorque(this.WheelCollidersLeft, 1);
				this.ApplyingMotorTorque(this.WheelCollidersRight, -1);
			}
			if ((this.WheelCollidersLeft[2].isGrounded || this.WheelCollidersRight[2].isGrounded) && Mathf.Abs(this.MainRigidbody.angularVelocity.y) < 1f)
			{
				Vector3 a = (!this.reversing) ? Vector3.up : (-Vector3.up);
				this.MainRigidbody.AddRelativeTorque(a * this.steerInput * this.SteerTorque, ForceMode.Acceleration);
			}
		}

		private void ApplyingMotorTorque(WheelCollider[] wheelsArray, int steerInputCounter)
		{
			foreach (WheelCollider wheelCollider in wheelsArray)
			{
				if (!this.reversing)
				{
					if (wheelCollider.isGrounded && Mathf.Abs(wheelCollider.rpm) < 1000f)
					{
						wheelCollider.motorTorque = this.EngineTorque * Mathf.Clamp(Mathf.Clamp(this.motorInput, 0f, 1f) + Mathf.Clamp(this.steerInput * (float)steerInputCounter, -1f, 1f), -1f, 1f) * this.EngineTorqueCurve.Evaluate(this.speed);
					}
					else
					{
						wheelCollider.motorTorque = 0f;
					}
				}
				else if (this.speed < 30f)
				{
					wheelCollider.motorTorque = this.EngineTorque * this.motorInput;
				}
				else
				{
					wheelCollider.motorTorque = 0f;
				}
			}
		}

		private void Braking()
		{
			for (int i = 0; i < this.allWheelColliders.Count; i++)
			{
				WheelCollider wheelCollider = this.allWheelColliders[i];
				if (this.motorInput == 0f)
				{
					if (this.speed < 25f && Mathf.Abs(this.steerInput) < 0.1f)
					{
						wheelCollider.brakeTorque = this.BrakeTorque / 5f;
					}
					else
					{
						wheelCollider.brakeTorque = 0f;
					}
				}
				else if (this.motorInput < -0.1f && this.allWheelColliders[0].rpm > 50f)
				{
					wheelCollider.brakeTorque = this.BrakeTorque * Mathf.Abs(this.motorInput);
				}
				else
				{
					wheelCollider.brakeTorque = 0f;
				}
			}
		}

		private void Inputs()
		{
			this.motorInput = Controls.GetAxis("Vertical");
			this.steerInput = Controls.GetAxis("Horizontal");
			this.reversing = (this.motorInput < 0f && this.allWheelColliders[0].rpm < 50f);
			if (this.WheelCollidersLeft[0].rpm < -10f && this.WheelCollidersRight[0].rpm < -10f && !this.reversing)
			{
				this.steerInput = -this.steerInput;
			}
			this.speed = this.MainRigidbody.velocity.magnitude * 3f;
			this.acceleration = 0f;
			float z = this.MainRigidbody.transform.InverseTransformDirection(this.MainRigidbody.velocity).z;
			this.acceleration = (z - this.lastVelocity) / Time.deltaTime;
			this.lastVelocity = z;
			this.MainRigidbody.drag = Mathf.Clamp(this.acceleration / 10f, 0f, 1f);
			this.engineRPM = Mathf.Clamp(Mathf.Abs(this.WheelCollidersLeft[0].rpm + this.WheelCollidersRight[0].rpm) * 5f + this.MinEngineRPM, this.MinEngineRPM, this.MaxEngineRPM);
		}

		private void AudioSetup()
		{
			this.EngineRunningAudioSource.pitch = Mathf.Lerp(this.EngineRunningAudioSource.pitch, Mathf.Lerp(0.4f, 1f, (this.engineRPM - this.MinEngineRPM / 1.5f) / (this.MaxEngineRPM + this.MinEngineRPM) + Mathf.Clamp(Mathf.Clamp(this.motorInput, 0f, 1f) + Mathf.Clamp(Mathf.Abs(this.steerInput), 0f, 0.5f), 0.35f, 0.85f)), Time.deltaTime * 2f);
			this.EngineRunningAudioSource.volume = Mathf.Lerp(this.EngineRunningAudioSource.volume, Mathf.Clamp(Mathf.Clamp(Mathf.Abs(this.motorInput), 0f, 1f) + Mathf.Clamp(Mathf.Abs(this.steerInput), 0f, 1f), 0.35f, 0.85f) * this.currentVolume, Time.deltaTime * 2f);
		}

		private void SmokeRate()
		{
			//if (this.WheelSlipPrefab && this.wheelParticles.Count > 0)
			//{
			//	for (int i = 0; i < this.allWheelColliders.Count; i++)
			//	{
			//		this.wheelParticles[i].emit = (this.speed > 25f && this.allWheelColliders[i].isGrounded);
			//	}
			//}
			//if (this.initedNormalExhaustGas)
			//{
			//	this.initedNormalExhaustGas.emit = (this.speed < 15f);
			//}
			//if (this.initedHeavyExhaustGas)
			//{
			//	this.initedHeavyExhaustGas.emit = (Mathf.Abs(this.motorInput) > 0.1f || Mathf.Abs(this.steerInput) > 0.1f);
			//}
		}

		protected override void Drowning()
		{
			base.Drowning();
			foreach (WheelCollider wheelCollider in this.allWheelColliders)
			{
				wheelCollider.brakeTorque = this.BrakeTorque;
			}
		}

		public override void DisableEngine()
		{
			base.DisableEngine();
			this.SmokeDeinit();
			this.EngineRunningAudioSource.Stop();
			this.engineEnabled = false;
		}

		private const string SteerAxeName = "Horizontal";

		private const string ThrottleAxeName = "Vertical";

		private const int MaxAngularVelocity = 5;

		private const int RpmMultipler = 6;

		private const int OffsetReducer = 1000;

		private const int MaxWheelRpm = 1000;

		private const int SmoothBrakeTorqueReducer = 5;

		private const int SmoothBrakeSpeed = 25;

		private const int MinSpeedToHardBrake = 50;

		private const int SpeedToReversing = 50;

		private const int AccelerationReducer = 10;

		private const int ToShowSpeedMultipler = 3;

		private const int EngineRpmMultipler = 5;

		private const int SmokeInitMinSpeed = 25;

		private const int NormalExhaustMaxSpeed = 15;

		private const int DeInitBrakeMultipler = 10;

		private const int MoveDownRpm = -10;

		[Separator("Tank Links")]
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

		[Separator("Settings")]
		public float TrackOffset;

		public float TrackScrollSpeedMultipler = 1f;

		public AnimationCurve EngineTorqueCurve;

		public float EngineTorque = 250f;

		public float BrakeTorque = 250f;

		public float MinEngineRPM = 1000f;

		public float MaxEngineRPM = 5000f;

		public float MaxSpeed = 80f;

		public float SteerTorque = 3f;

		[Separator("Audios")]
		public AudioSource EngineRunningAudioSource;

		public AudioClip EngineRunningAudioClip;

		[Separator("Effects")]
		public GameObject WheelSlipPrefab;

		//public ParticleEmitter NormalExhaustGas;

		//public ParticleEmitter HeavyExhaustGas;

		private bool reversing;

		private List<WheelCollider> allWheelColliders = new List<WheelCollider>();

		private float[] rotationValueLeft;

		private float[] rotationValueRight;

		private float speed;

		private float defSteerAngle;

		private float acceleration;

		private float lastVelocity;

		private float engineRPM;

		private float motorInput;

		private float steerInput;

		private float currentVolume;

		//private readonly List<ParticleEmitter> wheelParticles = new List<ParticleEmitter>();

		//private ParticleEmitter initedNormalExhaustGas;

		//private ParticleEmitter initedHeavyExhaustGas;
	}
}
