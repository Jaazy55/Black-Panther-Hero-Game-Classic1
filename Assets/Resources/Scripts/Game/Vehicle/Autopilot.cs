using System;
using System.Collections.Generic;
using Code.Game.Race;
using Code.Game.Race.UI;
using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Managers;
using Game.Traffic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Vehicle
{
	public class Autopilot : MonoBehaviour
	{
		public void OnSensorStay(Collider otherCollider, CollisionTrigger.Sensor sensor)
		{
			if (otherCollider.attachedRigidbody != null)
			{
				Vector3 rhs = otherCollider.attachedRigidbody.velocity - this.rootBody.velocity;
				if (Vector3.Dot(this.rootBody.transform.forward, rhs) > -(Vector3.Distance(this.rootBody.transform.position, otherCollider.attachedRigidbody.position) - 5f))
				{
					return;
				}
			}
			if (sensor == CollisionTrigger.Sensor.Front)
			{
				this.frontS = true;
			}
			else if (sensor == CollisionTrigger.Sensor.Left)
			{
				this.leftS = true;
			}
			else if (sensor == CollisionTrigger.Sensor.LeftBlocking)
			{
				this.leftBlockS = true;
			}
			else if (sensor == CollisionTrigger.Sensor.Right)
			{
				this.rightS = true;
			}
			else if (sensor == CollisionTrigger.Sensor.RightBlocking)
			{
				this.rightBlockS = true;
			}
		}

		public void DeInit()
		{
			this.isWorking = false;
			this.NavMeshAgent.gameObject.SetActive(false);
			this.speed = 0f;
			this.stackTimer = 0f;
			this.exitBlockTime = 0f;
			this.stackSteerSign = 0;
			this.KeepNavigatorPlace();
			if (this.engineAudioSource != null)
			{
				this.engineAudioSource.Stop();
				this.engineAudioSource.clip = null;
			}
			if (this.wheels != null)
			{
				foreach (WheelCollider wheelCollider in this.wheels)
				{
					wheelCollider.brakeTorque = 0f;
					wheelCollider.motorTorque = 0f;
					wheelCollider.steerAngle = 0f;
				}
			}
			this.drivableVehicle = null;
			this.wheels = null;
			this.steeringWheels = null;
			this.rootBody = null;
			this.target = null;
			this.waypoints = null;
			this.instanceDriver = null;
			this.DriverWasKilled = false;
			this.DriverExit = false;
		}

		public void InitWaypointTour(Transform[] waypoints, Rigidbody vehicleBody)
		{
			this.Init(Autopilot.Tactic.WaypointTour, vehicleBody);
			this.waypoints = waypoints;
			if (waypoints != null && waypoints.Length > 1)
			{
				this.waypointIndex = 0;
				for (int i = 1; i < waypoints.Length; i++)
				{
					Transform transform = waypoints[i];
					if (Vector3.Distance(waypoints[this.waypointIndex].position, vehicleBody.position) > Vector3.Distance(transform.position, vehicleBody.position))
					{
						this.waypointIndex = i;
					}
				}
				this.NavMeshAgent.SetDestination(waypoints[this.waypointIndex].position);
			}
			this.cruiseVel = this.drivableVehicle.MaxSpeed * (float)UnityEngine.Random.Range(20, 70) * 0.01f;
			this.cruiseVel = Mathf.Min(this.cruiseVel, 100f);
		}

		public void InitRace(Transform[] waypoints, Racer racer, int rounds)
		{
			Rigidbody mainRigidbody = racer.GetDrivableVehicle().MainRigidbody;
			this.Init(Autopilot.Tactic.Race, mainRigidbody);
			this.waypoints = waypoints;
			this.laps = rounds;
			this.lapIndex = 0;
			if (waypoints != null && waypoints.Length > 1)
			{
				this.waypointIndex = 0;
				this.NavMeshAgent.SetDestination(waypoints[this.waypointIndex].position);
			}
			this.cruiseVel = this.drivableVehicle.MaxSpeed;
			this.racer = racer;
			RacePositionController.Instance.AddItem(this.racer, false);
		}

		public void InitChase(Rigidbody vehicleBody)
		{
			this.Init(Autopilot.Tactic.Chase, vehicleBody);
			this.cruiseVel = this.drivableVehicle.MaxSpeed;
			vehicleBody.velocity = vehicleBody.transform.forward * 0.2777778f * Mathf.Min(this.cruiseVel, 100f);
		}

		private void Init(Autopilot.Tactic tactic, Rigidbody vehicleBody)
		{
			this.tactic = tactic;
			this.rootBody = vehicleBody;
			this.drivableVehicle = this.rootBody.GetComponent<DrivableVehicle>();
			this.steeringWheels = this.rootBody.GetComponent<SteeringWheels>();
			this.wheels = this.rootBody.GetComponentsInChildren<WheelCollider>();
			if (this.engineAudioSource != null && this.drivableVehicle != null && this.drivableVehicle.VehicleSpecificPrefab is CarSpecific)
			{
				AudioClip[] engineSounds = ((CarSpecific)this.drivableVehicle.VehicleSpecificPrefab).EngineSounds;
				if (engineSounds != null && engineSounds.Length > 0)
				{
					this.engineAudioSource.clip = engineSounds[0];
					this.engineAudioSource.Play();
				}
			}
			foreach (WheelCollider wheelCollider in this.wheels)
			{
				wheelCollider.brakeTorque = 0f;
			}
			this.InitSensors();
			this.NavMeshAgent.gameObject.SetActive(true);
			this.KeepNavigatorPlace();
			this.isWorking = true;
		}

		private void InitSensors()
		{
			float maxWidth = this.drivableVehicle.VehicleSpecificPrefab.MaxWidth;
			float maxHeight = this.drivableVehicle.VehicleSpecificPrefab.MaxHeight;
			this.SetSizeX(this.sensors[CollisionTrigger.Sensor.Front], maxWidth * 0.5f);
			this.SetSizeX(this.sensors[CollisionTrigger.Sensor.Left], maxWidth * 0.25f);
			this.SetSizeX(this.sensors[CollisionTrigger.Sensor.Right], maxWidth * 0.25f);
			float num = (this.sensors[CollisionTrigger.Sensor.Front].size.x + this.sensors[CollisionTrigger.Sensor.Right].size.x) * 0.5f;
			this.SetLocalX(this.sensors[CollisionTrigger.Sensor.Right], num);
			this.SetLocalX(this.sensors[CollisionTrigger.Sensor.Left], -num);
			float num2 = (maxWidth + this.sensors[CollisionTrigger.Sensor.LeftBlocking].size.x) * 0.5f;
			this.SetLocalX(this.sensors[CollisionTrigger.Sensor.RightBlocking], num2);
			this.SetLocalX(this.sensors[CollisionTrigger.Sensor.LeftBlocking], -num2);
			foreach (BoxCollider boxCollider in this.sensors.Values)
			{
				Vector3 size = boxCollider.size;
				size.y = maxHeight;
				boxCollider.size = size;
				Vector3 localPosition = boxCollider.transform.localPosition;
				localPosition.y = maxHeight * 0.5f;
				boxCollider.transform.localPosition = localPosition;
			}
			this.UpdateSensors();
		}

		private void SetSizeX(BoxCollider box, float x)
		{
			box.size = new Vector3(x, box.size.y, box.size.z);
		}

		private void SetLocalX(Component component, float x)
		{
			component.transform.localPosition = new Vector3(x, component.transform.localPosition.y, component.transform.localPosition.z);
		}

		private void Awake()
		{
			this.waterMask = 1 << NavMesh.GetAreaFromName("Water");
			if (this.NavMeshAgent == null && GameManager.ShowDebugs)
			{
				UnityEngine.Debug.LogError("NavMeshAgent not found");
			}
			this.engineAudioSource = base.GetComponent<AudioSource>();
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 0.5f);
			foreach (CollisionTrigger collisionTrigger in this.Triggers.GetComponentsInChildren<CollisionTrigger>())
			{
				this.sensors.Add(collisionTrigger.SensorType, collisionTrigger.GetComponent<BoxCollider>());
			}
		}

		private void SlowUpdate()
		{
			this.UpdateSensors();
		}

		private void UpdateSensors()
		{
			float num = Mathf.Min(Mathf.Abs(this.speed), 50f) * 0.6f;
			foreach (KeyValuePair<CollisionTrigger.Sensor, BoxCollider> keyValuePair in this.sensors)
			{
				BoxCollider value = keyValuePair.Value;
				Vector3 size = value.size;
				size.z = num * Autopilot.SensorsExpanders[keyValuePair.Key] + Autopilot.SensorsShift[keyValuePair.Key];
				value.size = size;
				value.center = new Vector3(value.center.x, value.center.y, value.size.z * 0.5f);
			}
		}

		private void Drowning()
		{
			this.isWorking = false;
			this.rootBody.velocity = Vector3.zero;
			if (this.wheels != null)
			{
				foreach (WheelCollider wheelCollider in this.wheels)
				{
					wheelCollider.brakeTorque = 10f;
				}
			}
			this.NavMeshAgent.Stop();
		}

		public virtual void DropPassangers()
		{
			this.DriverExit = true;
			this.isWorking = false;
			this.rootBody.velocity = Vector3.zero;
			DummyDriver componentInChildren = this.rootBody.GetComponentInChildren<DummyDriver>();
			if (componentInChildren != null && !this.drivableVehicle.CompareTag("Racer"))
			{
				HitEntity player = PlayerInteractionsManager.Instance.Player;
				this.DriverWasKilled = componentInChildren.DriverDead;
				if (this.DriverWasKilled)
				{
					TrafficManager.Instance.FreeCopVehicleSlot();
				}
				componentInChildren.InitOutOfVehicle(false, player, false);
				this.drivableVehicle.OpenVehicleDoor(VehicleDoor.LeftDoor, false);
				this.instanceDriver = componentInChildren.InitedStatusNPC;
				this.instanceDriver.DiedEvent += this.DropedCopKillEvent;
				if (!this.DriverWasKilled)
				{
					PoolManager.Instance.AddBeforeReturnEvent(this.instanceDriver, delegate(GameObject poolingObject)
					{
						TrafficManager.Instance.FreeCopVehicleSlot();
					});
				}
				PoolManager.Instance.AddBeforeReturnEvent(this.drivableVehicle, delegate(GameObject poolingObject)
				{
					if (this.instanceDriver.transform.IsChildOf(this.drivableVehicle.transform))
					{
						PoolManager.Instance.ReturnToPool(this.instanceDriver);
					}
				});
			}
			if (this.wheels != null)
			{
				foreach (WheelCollider wheelCollider in this.wheels)
				{
					wheelCollider.brakeTorque = 10f;
				}
			}
			this.NavMeshAgent.Stop();
		}

		public virtual void DropedCopKillEvent()
		{
			this.DriverWasKilled = true;
		}

		public virtual void ChangeDropedCopKillEvent()
		{
			this.instanceDriver.DiedEvent -= this.DropedCopKillEvent;
		}

		private void FixedUpdate()
		{
			if (!this.isWorking)
			{
				return;
			}
			if (this.drivableVehicle.DeepInWater)
			{
				this.Drowning();
				return;
			}
			this.slowUpdateProc.ProceedOnFixedUpdate();
			this.speed = this.rootBody.transform.InverseTransformDirection(this.rootBody.velocity).z;
			if (this.rootBody.velocity.magnitude * 3.6f < 3f && !this.isStacked)
			{
				this.stackTimer += Time.deltaTime;
				if (this.stackTimer > 3f)
				{
					this.isStacked = true;
				}
			}
			switch (this.tactic)
			{
			case Autopilot.Tactic.Chase:
				this.ChaseFixedUpdate();
				break;
			case Autopilot.Tactic.WaypointTour:
				this.WaypointTourFixedUpdate();
				break;
			case Autopilot.Tactic.Race:
				this.RaceFixedUpdate();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (this.drivableVehicle != null)
			{
				this.drivableVehicle.ApplyStabilization(1000000f * Time.deltaTime);
			}
			this.DropSensorsStatus();
		}

		private void ChaseFixedUpdate()
		{
			this.target = ((!PlayerInteractionsManager.Instance.IsDrivingAVehicle()) ? PlayerInteractionsManager.Instance.Player.rigidbody : PlayerInteractionsManager.Instance.LastDrivableVehicle.MainRigidbody);
			Vector3 position = this.rootBody.transform.position;
			position.y = 0f;
			Vector3 position2 = this.target.position;
			position2.y = 0f;
			float num = Vector3.Distance(position, position2);
			float num2;
			float throttle;
			if (num < this.MaxDistanceToDrop && this.target.velocity.magnitude < 1.388889f)
			{
				num2 = this.rootBody.transform.InverseTransformDirection(this.target.transform.forward).x;
				this.ProceedSensors(out throttle, ref num2, 0f);
				if (this.speed < 5f && this.exitBlockTime <= 0f)
				{
					this.exitBlockTime = 2f;
					Vector3 vector;
					NavMeshHit navMeshHit;
					if (!this.drivableVehicle.IsDoorBlockedOffset(this.DropPassengersBlockingLayer, this.rootBody.transform, out vector, true) && !this.drivableVehicle.DeepInWater && !this.NavMeshAgent.SamplePathPosition(-1, 50f, out navMeshHit) && (navMeshHit.mask & this.waterMask) == 0)
					{
						this.isWorking = false;
						this.DropPassangers();
					}
				}
			}
			else
			{
				Vector3 vector2 = this.target.position + this.target.transform.forward * 5f;
				Vector3 normalized;
				if (Vector3.Distance(position, position2) > 30f)
				{
					this.SetDestination(vector2, 1f);
					normalized = this.NavMeshAgent.desiredVelocity.normalized;
				}
				else
				{
					normalized = (vector2 - this.rootBody.transform.position).normalized;
				}
				num2 = this.rootBody.transform.InverseTransformDirection(normalized).normalized.x;
				float num3 = this.cruiseVel;
				if (Vector3.Dot(this.rootBody.transform.forward, normalized) < 0f)
				{
					num2 = Mathf.Sign(num2);
					num3 *= 0.1f;
				}
				if (Vector3.Distance(position, vector2) < 2f)
				{
					num3 = 0f;
				}
				this.ProceedSensors(out throttle, ref num2, num3);
				this.ProceedStack(ref throttle, ref num2);
			}
			this.ThrottleFixedUpdate(throttle);
			this.SteeringFixedUpdate(num2);
			if (this.exitBlockTime > 0f)
			{
				this.exitBlockTime -= Time.deltaTime;
			}
		}

		private void Update()
		{
			if (!this.isWorking)
			{
				return;
			}
			this.KeepNavigatorPlace();
		}

		private void SetDestination(Vector3 point, float timeOut)
		{
			if (Time.time - this.lastTimeSetDestination < timeOut)
			{
				return;
			}
			this.NavMeshAgent.SetDestination(point);
			this.lastTimeSetDestination = Time.time;
		}

		private void DropSensorsStatus()
		{
			this.frontS = false;
			this.leftS = false;
			this.leftBlockS = false;
			this.rightS = false;
			this.rightBlockS = false;
		}

		private void KeepNavigatorPlace()
		{
			this.NavMeshAgent.transform.localPosition = Vector3.zero;
		}

		private void WaypointTourFixedUpdate()
		{
			if (this.waypoints == null || this.waypoints.Length < 1)
			{
				return;
			}
			Transform transform = this.waypoints[this.waypointIndex];
			if (Vector3.Distance(this.rootBody.transform.position, transform.position) < 30f)
			{
				this.waypointIndex = (this.waypointIndex + 1) % this.waypoints.Length;
				this.SetDestination(this.waypoints[this.waypointIndex].position, 1f);
			}
			float num = this.rootBody.transform.InverseTransformDirection(this.NavMeshAgent.desiredVelocity).normalized.x;
			if (Vector3.Dot(this.rootBody.transform.forward, this.NavMeshAgent.desiredVelocity.normalized) < 0f)
			{
				num = Mathf.Sign(num);
			}
			float steer = num;
			float throttle;
			this.ProceedSensors(out throttle, ref steer, this.cruiseVel);
			this.ProceedStack(ref throttle, ref steer);
			this.ThrottleFixedUpdate(throttle);
			this.SteeringFixedUpdate(steer);
		}

		private void Finish()
		{
			if (this.drivableVehicle.CompareTag("Racer"))
			{
				UiRaceManager.Instance.AddItemToResultTable(this.racer, false);
			}
			this.isWorking = false;
			TrafficManager.Instance.AddTrafficDriverForVehicle(this.drivableVehicle);
			this.drivableVehicle.tag = "Untagged";
		}

		private void RaceFixedUpdate()
		{
			if (this.drivableVehicle.CurrentDriver.IsPlayer)
			{
				this.isWorking = false;
			}
			Transform transform = this.waypoints[this.waypointIndex];
			this.distanceToPoint = Vector3.Distance(this.rootBody.transform.position, transform.position);
			this.racer.SetLap(this.lapIndex);
			this.racer.SetWaypointIndex(this.waypointIndex);
			this.racer.SetDistanceToPoint(this.distanceToPoint);
			if (this.distanceToPoint < 30f)
			{
				this.waypointIndex = (this.waypointIndex + 1) % this.waypoints.Length;
				if (this.waypointIndex == 0)
				{
					this.lapIndex++;
					if (this.laps == this.lapIndex)
					{
						this.Finish();
					}
				}
				this.SetDestination(this.waypoints[this.waypointIndex].position, 1f);
			}
			float num = this.rootBody.transform.InverseTransformDirection(this.NavMeshAgent.desiredVelocity).normalized.x;
			if (Vector3.Dot(this.rootBody.transform.forward, this.NavMeshAgent.desiredVelocity.normalized) < 0f)
			{
				num = Mathf.Sign(num);
			}
			float steer = num;
			float throttle;
			this.ProceedSensors(out throttle, ref steer, this.cruiseVel);
			this.ProceedStack(ref throttle, ref steer);
			this.ThrottleFixedUpdate(throttle);
			this.SteeringFixedUpdate(steer);
		}

		private void ProceedStack(ref float resultThrottle, ref float resultSteer)
		{
			if (this.isStacked)
			{
				if (this.stackTimer > 0f)
				{
					this.stackTimer -= Time.deltaTime;
				}
				else
				{
					this.isStacked = false;
					this.stackSteerSign = 0;
				}
				float num = this.stackTimer / 2f;
				float num2 = this.stackTimer - (float)((int)num) * 1f;
				resultThrottle = -1f;
				resultSteer = Mathf.Sign(resultSteer);
				if (this.stackSteerSign == 0)
				{
					this.stackSteerSign = (int)(-(int)resultSteer);
				}
				if (this.stackSteerSign == 0)
				{
					this.stackSteerSign = -1;
				}
				resultSteer = (float)this.stackSteerSign;
				if (num2 <= 1f)
				{
					resultThrottle *= -1f;
					resultSteer = (float)(-(float)this.stackSteerSign);
				}
			}
		}

		private void ProceedSensors(out float resultThrottle, ref float resultSteer, float shouldBeSpeed)
		{
			resultThrottle = 0f;
			if (this.isStacked)
			{
				return;
			}
			if (this.frontS)
			{
				shouldBeSpeed = this.speed * 3.6f * 0.95f;
			}
			if (this.rightBlockS)
			{
				resultSteer = Mathf.Clamp(resultSteer, -1f, 0f);
			}
			if (this.leftBlockS)
			{
				resultSteer = Mathf.Clamp(resultSteer, 0f, 1f);
			}
			if (this.leftS && !this.rightBlockS)
			{
				resultSteer += 0.5f;
			}
			else if (this.rightS && !this.leftBlockS)
			{
				resultSteer -= 0.5f;
			}
			float num = shouldBeSpeed - this.speed * 3.6f;
			resultThrottle = Mathf.Clamp(num / 5f, -1f, 1f);
			resultSteer = Mathf.Clamp(resultSteer, -1f, 1f);
		}

		private void SteeringFixedUpdate(float steer)
		{
			float d = Mathf.Sign(this.speed);
			if (this.rootBody != null)
			{
				this.rootBody.AddRelativeTorque(d * Vector3.up * steer * 10000f * Time.deltaTime);
			}
			if (this.steeringWheels != null)
			{
				foreach (WheelCollider wheelCollider in this.steeringWheels.Wheels)
				{
					float num = 1f - Mathf.Abs(this.speed) / (1.2f * this.drivableVehicle.MaxSpeed * 0.2777778f);
					wheelCollider.steerAngle = steer * 60f * num;
				}
			}
		}

		private void ThrottleFixedUpdate(float throttle)
		{
			bool flag = Mathf.Sign(throttle) * Mathf.Sign(this.speed) < 0f;
			Vector3 vector = Vector3.forward * throttle * 300000f * Time.deltaTime;
			if (flag)
			{
				vector *= 3f;
			}
			if (this.rootBody != null)
			{
				this.rootBody.AddRelativeForce(vector);
			}
			if (this.wheels != null)
			{
				foreach (WheelCollider wheelCollider in this.wheels)
				{
					wheelCollider.motorTorque = throttle * 10000f / (float)this.wheels.Length * Time.deltaTime;
				}
			}
		}

		private const string WaterAreaName = "Water";

		private const int SteerMaxAngle = 60;

		private const int RotateForce = 10000;

		private const int PullForce = 300000;

		private const int EngineTorque = 10000;

		protected const float BrakeTorque = 10f;

		private const float OnBrakeForceMultiplier = 3f;

		private const int StabilizationForce = 1000000;

		private const float CheckDistance = 30f;

		private const float SpeedLimit = 100f;

		private const float SpeedAccuracy = 5f;

		private const float CheckChaseDistance = 2f;

		private const float NavigatorToUseDistance = 30f;

		private const float TargetForwardShift = 5f;

		private float lastTimeSetDestination;

		private const float SensorSizeVelocityFactor = 0.6f;

		private const float MaxSensorVelocity = 50f;

		private const float MinSensorSize = 0f;

		private const float StackVelocity = 3f;

		private const float StackTimeToAction = 3f;

		private const float OneWayTime = 1f;

		private const float MaxTargetVelocityToDrop = 5f;

		private const float ExitBlockedCheckFreq = 2f;

		private static readonly IDictionary<CollisionTrigger.Sensor, float> SensorsExpanders = new Dictionary<CollisionTrigger.Sensor, float>
		{
			{
				CollisionTrigger.Sensor.Front,
				2f
			},
			{
				CollisionTrigger.Sensor.Left,
				1f
			},
			{
				CollisionTrigger.Sensor.LeftBlocking,
				1f
			},
			{
				CollisionTrigger.Sensor.Right,
				1f
			},
			{
				CollisionTrigger.Sensor.RightBlocking,
				1f
			}
		};

		private static readonly IDictionary<CollisionTrigger.Sensor, float> SensorsShift = new Dictionary<CollisionTrigger.Sensor, float>
		{
			{
				CollisionTrigger.Sensor.Front,
				0f
			},
			{
				CollisionTrigger.Sensor.Left,
				0f
			},
			{
				CollisionTrigger.Sensor.LeftBlocking,
				3f
			},
			{
				CollisionTrigger.Sensor.Right,
				0f
			},
			{
				CollisionTrigger.Sensor.RightBlocking,
				3f
			}
		};

		public GameObject Triggers;

		public NavMeshAgent NavMeshAgent;

		public LayerMask DropPassengersBlockingLayer;

		public bool DriverExit;

		public bool DriverWasKilled;

		public float MaxDistanceToDrop = 20f;

		private Racer racer;

		private Transform[] waypoints;

		private int waypointIndex;

		private float distanceToPoint;

		private int laps;

		private int lapIndex;

		private Autopilot.Tactic tactic;

		private Rigidbody target;

		private int waterMask;

		private DrivableVehicle drivableVehicle;

		protected WheelCollider[] wheels;

		private SteeringWheels steeringWheels;

		protected Rigidbody rootBody;

		private AudioSource engineAudioSource;

		private HitEntity instanceDriver;

		private float speed;

		private SlowUpdateProc slowUpdateProc;

		private float cruiseVel = 20f;

		private bool frontS;

		private bool leftS;

		private bool leftBlockS;

		private bool rightS;

		private bool rightBlockS;

		private float stackTimer;

		private bool isStacked;

		protected bool isWorking;

		private float exitBlockTime;

		private int stackSteerSign;

		private readonly IDictionary<CollisionTrigger.Sensor, BoxCollider> sensors = new Dictionary<CollisionTrigger.Sensor, BoxCollider>();

		public enum Tactic
		{
			Chase,
			WaypointTour,
			Race
		}
	}
}
