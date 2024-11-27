using System;
using System.Collections;
using Game.Character;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.Traffic
{
	public class TrafficDriver : MonoBehaviour
	{
		public TrafficDriver()
		{
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 0.5f);
		}

		public void DisableControll()
		{
			this.IsControlled = false;
			if (this.wheels != null)
			{
				foreach (WheelCollider wheelCollider in this.wheels)
				{
					wheelCollider.motorTorque = 0f;
					wheelCollider.brakeTorque = this.brakeTorque;
				}
			}
		}

		public void Init(Rigidbody rb, Transform initialPoint, RoadPoint fromPoint, RoadPoint toPoint, int line)
		{
			this.fromPoint = fromPoint;
			this.initPoint = fromPoint.Point;
			this.toPoint = toPoint;
			this.line = line;
			this.IsControlled = true;
			this.stayTimeout = 0f;
			this.initTime = Time.timeSinceLevelLoad;
			this.RootBody = rb;
			this.InitialPoint = initialPoint;
			base.transform.position = this.InitialPoint.position;
			base.transform.rotation = this.InitialPoint.rotation;
			this.trigger = base.GetComponent<BoxCollider>();
			this.trigger.center = Vector3.forward * this.trigger.size.z * 0.51f + Vector3.up * this.trigger.size.y * 0.51f;
			TrafficManager.Instance.CalcTargetPoint(fromPoint, toPoint, line, out this.startLine, out this.target);
			this.drivableVehicle = this.RootBody.GetComponent<DrivableVehicle>();
			this.steeringWheels = this.RootBody.GetComponent<SteeringWheels>();
			this.wheels = this.RootBody.GetComponentsInChildren<WheelCollider>();
			this.drivableVehicle.ConstraintsSetup(true);
			this.pullForce = this.drivableVehicle.VehicleSpecificPrefab.PullForce;
			this.engineTorque = this.drivableVehicle.VehicleSpecificPrefab.EngineTorque;
			this.rotateForce = this.drivableVehicle.VehicleSpecificPrefab.RotateForce;
			this.brakeTorque = this.drivableVehicle.VehicleSpecificPrefab.BrakeTorque;
			this.steerMaxAngle = this.drivableVehicle.VehicleSpecificPrefab.SteerMaxAngle;
			this.cruiseVel = this.drivableVehicle.MaxSpeed * (float)UnityEngine.Random.Range(20, 40) * 0.01f;
			this.cruiseVel = Mathf.Min(this.cruiseVel, 100f);
			this.roadBlocked = false;
			base.StartCoroutine(this.CalculateSpeedForwardVehicles(1f));
			foreach (WheelCollider wheelCollider in this.wheels)
			{
				wheelCollider.motorTorque = 0f;
				wheelCollider.brakeTorque = 0f;
			}
			if (this.EngineAudioSource != null && this.drivableVehicle != null && this.drivableVehicle.VehicleSpecificPrefab && ((CarSpecific)this.drivableVehicle.VehicleSpecificPrefab).EngineSounds != null && ((CarSpecific)this.drivableVehicle.VehicleSpecificPrefab).EngineSounds.Length > 0)
			{
				this.EngineAudioSource.clip = ((CarSpecific)this.drivableVehicle.VehicleSpecificPrefab).EngineSounds[0];
				this.EngineAudioSource.Play();
			}
		}

		public void DeInit()
		{
			if (this.EngineAudioSource != null)
			{
				this.EngineAudioSource.Stop();
				this.EngineAudioSource.clip = null;
			}
			this.IsControlled = false;
			this.RootBody = null;
			this.InitialPoint = null;
		}

		private void Awake()
		{
			if (TrafficDriver.PseudoObstacleLayerNumber == -1)
			{
				TrafficDriver.PseudoObstacleLayerNumber = LayerMask.NameToLayer("TrafficPseudoObstacles");
			}
		}

		private IEnumerator CalculateSpeedForwardVehicles(float calculateTime)
		{
			this.calculateStartSpeed = true;
			bool calculated = false;
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			while (calculateTime > 0f)
			{
				if (this.lastVehicle != null && this.RootBody.transform.forward == this.lastVehicle.transform.forward)
				{
					this.RootBody.velocity = this.lastVehicle.MainRigidbody.velocity;
					calculated = true;
					break;
				}
				if (this.pseudoObsacleObject != null)
				{
					this.RootBody.velocity = Vector3.zero;
					calculated = true;
					break;
				}
				calculateTime -= Time.deltaTime;
				yield return waitForEndOfFrame;
			}
			if (!calculated)
			{
				this.RootBody.velocity = this.cruiseVel * (this.target - this.startLine).normalized * 0.2777778f;
			}
			this.calculateStartSpeed = false;
			yield break;
		}

		private float CalcCurrentCruiseVel()
		{
			if (this.roadBlocked)
			{
				return 0f;
			}
			float num = this.cruiseVel;
			if (Vector3.Distance(this.RootBody.transform.position, this.toPoint.Point) < 30f)
			{
				num = Mathf.Min(num, this.toPoint.SpeedLimit);
			}
			return num * 0.2777778f;
		}

		private void SlowUpdate()
		{
			if (this.trigger != null)
			{
				this.trigger.size = new Vector3(this.trigger.size.x, this.trigger.size.y, Mathf.Max(Mathf.Abs(this.speed) * 1.2f, 2f));
				this.trigger.center = new Vector3(this.trigger.center.x, this.trigger.center.y, this.trigger.size.z / 2f);
			}
			float num = this.initTime + 2f;
			if (!SectorManager.Instance.IsInActiveSector(this.RootBody.transform.position) && (num < Time.timeSinceLevelLoad || (num >= Time.timeSinceLevelLoad && !SectorManager.Instance.IsInActiveSector(this.initPoint))) && !CameraManager.Instance.IsInCameraFrustrum(this.RootBody.transform.position))
			{
				this.DestroyVehicle();
			}
		}

		public void DestroyVehicle()
		{
			TrafficManager.Instance.TrafficVehicleOutOfRange(this.drivableVehicle, this);
		}

		private void Klaxon()
		{
			if (this.roadBlocked)
			{
				this.klaxonTimeout -= Time.deltaTime;
			}
			else
			{
				this.klaxonTimeout = 3f;
			}
			if (this.klaxonTimeout < 0f)
			{
				PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.Klaxon);
				this.klaxonTimeout = UnityEngine.Random.Range(3f, 15f);
			}
		}

		private void ProceedEnginePitchSound()
		{
			if (this.EngineAudioSource)
			{
				this.EngineAudioSource.pitch = Mathf.Lerp(this.EngineAudioSource.pitch, Mathf.Clamp(this.accelerationImitation + 0.35f, 0.35f, 1.1f), Time.deltaTime);
			}
		}

		private void Update()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
			if (!this.IsControlled)
			{
				return;
			}
			if (!this.drivableVehicle.OnGround)
			{
				return;
			}
			this.Klaxon();
			if (this.stayTimeout < 0f)
			{
				this.stayTimeout = 0f;
				this.roadBlocked = false;
			}
			else if (this.stayTimeout > 0f)
			{
				this.stayTimeout -= Time.deltaTime;
			}
			if (this.RootBody != null)
			{
				if ((this.target - this.RootBody.transform.position).magnitude < 3f)
				{
					TrafficManager.Instance.GetNextRoute(ref this.fromPoint, ref this.toPoint, ref this.line);
					TrafficManager.Instance.CalcTargetPoint(this.fromPoint, this.toPoint, this.line, out this.startLine, out this.target);
				}
				Vector3 rhs = this.target - this.startLine;
				Vector3 vector = Vector3.Cross(rhs.normalized, Vector3.up);
				Vector3 lhs = -this.RootBody.transform.position + this.target;
				Vector3 normalized = lhs.normalized;
				float num = Vector3.Dot(this.RootBody.transform.forward, normalized);
				float num2 = Vector3.Cross(lhs, rhs).magnitude / rhs.magnitude;
				if (num2 > TrafficManager.Instance.RoadLineSize * 0.1f)
				{
					float num3 = Vector3.Dot(normalized, vector);
					normalized = (normalized + num3 * 3f * vector).normalized;
					num *= 0.5f;
				}
				this.speed = this.RootBody.transform.InverseTransformDirection(this.RootBody.velocity).z;
				float num4 = this.CalcCurrentCruiseVel() * Mathf.Max(Mathf.Abs(num), 0.2f);
				float f = num4 - this.speed;
				float num5 = (Mathf.Abs(f) <= 0.1f) ? 0f : Mathf.Sign(f);
				bool flag = Mathf.Sign(num5) * Mathf.Sign(this.speed) < 0f;
				this.accelerationImitation = ((this.CalcCurrentCruiseVel() != 0f) ? Mathf.Lerp(this.accelerationImitation, 1f, Time.deltaTime * 0.4f) : this.CalcCurrentCruiseVel());
				this.onBrakeForceMultiplier = ((!flag) ? 0f : Mathf.Lerp(this.onBrakeForceMultiplier, 3f, Time.deltaTime * 3f));
				this.ProceedEnginePitchSound();
				Vector3 vector2 = Vector3.forward * num5 * (float)this.pullForce * Time.deltaTime;
				vector2 *= ((!flag) ? this.accelerationImitation : this.onBrakeForceMultiplier);
				this.RootBody.AddRelativeForce(vector2);
				if (this.wheels != null)
				{
					foreach (WheelCollider wheelCollider in this.wheels)
					{
						wheelCollider.motorTorque = num5 * (float)this.engineTorque / (float)this.wheels.Length * Time.deltaTime * this.accelerationImitation;
					}
				}
				this.steer = this.RootBody.transform.InverseTransformDirection(normalized).normalized.x;
				float d = Mathf.Sign(this.speed);
				this.RootBody.AddRelativeTorque(d * Vector3.up * this.steer * (float)this.rotateForce * Time.deltaTime);
				if (this.steeringWheels != null)
				{
					float num6 = 1f - Mathf.Abs(this.speed) / (1.2f * this.drivableVehicle.MaxSpeed * 0.2777778f);
					this.drivableVehicle.SteerStabilization(this.steer * (float)this.steerMaxAngle * num6);
					foreach (WheelCollider wheelCollider2 in this.steeringWheels.Wheels)
					{
						wheelCollider2.steerAngle = this.steer * (float)this.steerMaxAngle * num6;
					}
				}
				this.drivableVehicle.ApplyStabilization(1000000f * Time.deltaTime);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (!this.IsControlled)
			{
				return;
			}
			this.stayTimeout = 0.3f;
			this.roadBlocked = true;
			if (this.calculateStartSpeed)
			{
				this.lastVehicle = other.GetComponentInParent<DrivableVehicle>();
				this.pseudoObsacleObject = ((other.gameObject.layer != TrafficDriver.PseudoObstacleLayerNumber) ? null : other.gameObject);
			}
		}

		private const int StabilizationForce = 1000000;

		private const float ReturnToLineSpeed = 3f;

		private const float LineFollowAccuracy = 0.1f;

		private const float SpeedAccuracy = 0.1f;

		private const float MinSpeedValue = 0.2f;

		private const float StayTimeout = 0.3f;

		private const float CollideTriggerSpeedCoef = 1.2f;

		private const float MinTriggerSize = 2f;

		private const float KlaxonTimeoutMax = 15f;

		private const float KlaxonTimeoutMin = 3f;

		private const float SpeedLimitDistance = 30f;

		private const float SpeedLimit = 100f;

		private const float OnBrakeForceMultiplier = 3f;

		private const float OnBrakeForceRate = 3f;

		private const float AccelerationImitationRate = 0.4f;

		private const float MinPitch = 0.35f;

		private const float MaxPitch = 1.1f;

		private const string PseudoObstaclesLayerName = "TrafficPseudoObstacles";

		private const float CalculateStartSpeedTime = 1f;

		private static int PseudoObstacleLayerNumber = -1;

		public Rigidbody RootBody;

		public Transform InitialPoint;

		public AudioSource EngineAudioSource;

		public bool IsControlled = true;

		private int pullForce = 300000;

		private int engineTorque = 10000;

		private int rotateForce = 10000;

		private float brakeTorque = 50f;

		private int steerMaxAngle = 60;

		private RoadPoint fromPoint;

		private RoadPoint toPoint;

		private Vector3 target;

		private Vector3 startLine;

		private int line = 1;

		private float cruiseVel = 20f;

		private DrivableVehicle drivableVehicle;

		private WheelCollider[] wheels;

		private SteeringWheels steeringWheels;

		private float speed;

		private float stayTimeout;

		private SlowUpdateProc slowUpdateProc;

		private BoxCollider trigger;

		private float klaxonTimeout;

		private float initTime;

		private Vector3 initPoint;

		private bool roadBlocked;

		private float accelerationImitation = 1f;

		private float steer;

		private DrivableVehicle lastVehicle;

		private GameObject pseudoObsacleObject;

		private bool calculateStartSpeed;

		private float onBrakeForceMultiplier = 3f;
	}
}
