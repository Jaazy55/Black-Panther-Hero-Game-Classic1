using System;
using System.Collections;
using Game.Character;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using Game.Managers;
using Game.Traffic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Vehicle
{
	[RequireComponent(typeof(Rigidbody))]
	public class DrivableVehicle : MonoBehaviour, IInitable
	{
		public static int terrainLayerNumber { get; private set; }

		public static int chatacterLayerNumber { get; private set; }

		public static int staticObjectLayerNumber { get; private set; }

		public static int smallDynamicLayerNumber { get; private set; }

		public static int complexStaticObjectLayerNumber { get; private set; }

		public static int bigDynamicLayerNumber { get; private set; }

		public static int defaultLayerNumber { get; private set; }

		public bool DriverIsVulnerable
		{
			get
			{
				return !this.vehStatus.IsArmored;
			}
		}

		public float Speed()
		{
			return Vector3.Dot(this.MainRigidbody.velocity, this.MainRigidbody.transform.forward) * 3.6f;
		}

		public virtual VehicleType GetVehicleType()
		{
			return VehicleType.Car;
		}

		public virtual bool HasExitAnimation()
		{
			return false;
		}

		public virtual bool HasEnterAnimation()
		{
			return true;
		}

		public virtual bool IsControlsPlayerAnimations()
		{
			return false;
		}

		public virtual void ApplyStabilization(float force)
		{
		}

		public virtual void OnDriverStatusDamageEvent(float damage, HitEntity owner)
		{
		}

		public virtual void CheckLocationOnGround()
		{
			this.OnGround = true;
		}

		public virtual void SetDummyDriver(DummyDriver driver)
		{
			if (!driver)
			{
				return;
			}
			this.DummyDriver = driver;
			this.CurrentDriver = this.DummyDriver.DriverStatus;
			PoolManager.Instance.AddBeforeReturnEvent(driver, delegate(GameObject poolingObject)
			{
				this.DummyDriver = null;
				if (PlayerInteractionsManager.Instance.LastDrivableVehicle != this)
				{
					this.CurrentDriver = null;
					this.GetVehicleStatus().Faction = Faction.NoneFaction;
				}
			});
		}

		public Rigidbody MainRigidbody
		{
			get
			{
				Rigidbody result;
				if ((result = this.mainRigidbody) == null)
				{
					result = (this.mainRigidbody = base.GetComponent<Rigidbody>());
				}
				return result;
			}
		}

		public VehicleStatus GetVehicleStatus()
		{
			if (this.vehStatus == null)
			{
				this.vehStatus = base.GetComponentInChildren<VehicleStatus>();
			}
			return this.vehStatus;
		}

		protected virtual void Awake()
		{
			if (DrivableVehicle.chatacterLayerNumber == 0)
			{
				DrivableVehicle.chatacterLayerNumber = LayerMask.NameToLayer("Character");
			}
			if (DrivableVehicle.smallDynamicLayerNumber == 0)
			{
				DrivableVehicle.smallDynamicLayerNumber = LayerMask.NameToLayer("SmallDynamic");
			}
			if (DrivableVehicle.terrainLayerNumber == 0)
			{
				DrivableVehicle.terrainLayerNumber = LayerMask.NameToLayer("Terrain");
			}
			if (DrivableVehicle.complexStaticObjectLayerNumber == 0)
			{
				DrivableVehicle.complexStaticObjectLayerNumber = LayerMask.NameToLayer("ComplexStaticObject");
			}
			if (DrivableVehicle.staticObjectLayerNumber == 0)
			{
				DrivableVehicle.staticObjectLayerNumber = LayerMask.NameToLayer("SimpleStaticObject");
			}
			if (DrivableVehicle.bigDynamicLayerNumber == 0)
			{
				DrivableVehicle.bigDynamicLayerNumber = LayerMask.NameToLayer("BigDynamic");
			}
			if (DrivableVehicle.defaultLayerNumber == 0)
			{
				DrivableVehicle.defaultLayerNumber = LayerMask.NameToLayer("Default");
			}
			if (DrivableVehicle.WaterLayerNumber == 0)
			{
				DrivableVehicle.WaterLayerNumber = LayerMask.NameToLayer("Water");
			}
			this.AnimateGetInOut = true;
			this.vehicleSource = base.GetComponentInChildren<VehicleSource>();
			this.boxSource = this.vehicleSource.SourceCollider;
			this.vehicleSource.RootVehicle = this;
			this.verySlowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.VerySlowUpdate), 10f);
			if (base.GetComponent<CarTransformer>() != null)
			{
				this.isTransformer = true;
			}
		}

		public virtual void Init()
		{
			if (this.VehicleControllerPrefab == null)
			{
				throw new Exception(string.Format("{0} DrivableVehicle is missing VehicleControllerPrefab", base.gameObject.name));
			}
			if (this.vehStatus == null)
			{
				this.vehStatus = base.GetComponentInChildren<VehicleStatus>();
			}
			this.vehStatus.Initialization(true);
			this.ApplyCenterOfMass(this.VehiclePoints.CenterOfMass);
			this.ChangeBodyColor(this.BodyRenderers);
			if (!this.WaterSensor)
			{
				this.WaterSensor = base.GetComponentInChildren<WaterSensor>();
			}
			if (this.WaterSensor)
			{
				this.WaterSensor.Init();
			}
		}

		public Material GetBodyMaterial()
		{
			if (this.bodyMaterialIndex == -1)
			{
				int num = UnityEngine.Random.Range(0, this.BodyMaterials.Length);
				this.bodyMaterialIndex = num;
			}
			return this.BodyMaterials[this.bodyMaterialIndex];
		}

		public virtual void DeInit()
		{
			this.AnimateGetInOut = true;
			if (this.controller != null)
			{
				PoolManager.Instance.ReturnToPool(this.controller);
				this.controller = null;
			}
			this.RemoveObstacle();
			if (this.WaterSensor)
			{
				this.WaterSensor.Reset();
			}
		}

		public virtual bool IsAbleToEnter()
		{
			return this.MainRigidbody.velocity.magnitude < 20f && !this.isTransformer && !base.gameObject.CompareTag("Racer");
		}

		public VehicleSource GetVehicleSource()
		{
			return this.vehicleSource;
		}

		public virtual bool PointOnTheLeft(Vector3 pointPosition)
		{
			float num = Vector3.Distance(this.MainRigidbody.transform.position + this.MainRigidbody.transform.right, pointPosition);
			float num2 = Vector3.Distance(this.MainRigidbody.transform.position - this.MainRigidbody.transform.right, pointPosition);
			return num < num2;
		}

		public void ChangeSubstrate(bool isSubstrate)
		{
			if (!this.Wheels || !this.VehicleSubstrateColider)
			{
				return;
			}
			this.Wheels.SetActive(!isSubstrate);
			this.VehicleSubstrateColider.SetActive(isSubstrate);
		}

		public virtual void ConstraintsSetup(bool isin)
		{
		}

		public virtual void Drive(Player driver)
		{
			if (this.VehicleControllerPrefab != null && this.controller == null)
			{
				this.ChangeSubstrate(false);
				if (this.vehStatus == null)
				{
					this.vehStatus = base.GetComponentInChildren<VehicleStatus>();
					this.vehStatus.Initialization(true);
				}
				this.vehStatus.SetCameraIgnoreCollision();
				this.vehStatus.Faction = driver.Faction;
				this.MainRigidbody.velocity = Vector3.zero;
				this.controller = PoolManager.Instance.GetFromPool<VehicleController>(this.VehicleControllerPrefab);
				PoolManager.Instance.AddBeforeReturnEvent(this.controller, delegate(GameObject poolingObject)
				{
					if (this.SimpleModel != null)
					{
						this.SimpleModel.SetActive(true);
					}
				});
				GameObject gameObject = this.controller.gameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				this.controller.Init(this);
				if (this.SimpleModel != null)
				{
					this.SimpleModel.SetActive(false);
				}
				TrafficDriver componentInChildren = base.GetComponentInChildren<TrafficDriver>();
				if (componentInChildren != null)
				{
					PoolManager.Instance.ReturnToPool(componentInChildren);
				}
				GameEventManager.Instance.Event.GetIntoVehicleEvent(this);
				this.AnimateGetInOut = true;
			}
		}

		public virtual void SteerStabilization(float steer)
		{
		}

		public virtual void GetOut()
		{
			this.CurrentDriver = null;
			if (this.controller != null)
			{
				this.oldController = this.controller;
				this.controller.DeInit(delegate()
				{
					this.ChangeSubstrate(true);
					PoolManager.Instance.ReturnToPool(this.oldController);
					GameEventManager.Instance.Event.GetOutVehicleEvent(this);
				});
				this.controller = null;
			}
			this.vehStatus.RemoveCameraIgnoreCollision();
			this.vehStatus.Faction = Faction.NoneFaction;
		}

		public void ChangeBodyColor(Renderer[] renderers)
		{
			if (!this.Tuning)
			{
				return;
			}
			Material bodyMaterial = this.GetBodyMaterial();
			foreach (Renderer renderer in renderers)
			{
				if (renderer.materials.Length == 1)
				{
					renderer.material = bodyMaterial;
				}
				else if (renderer.materials.Length >= 2)
				{
					Material[] materials = renderer.materials;
					int num = materials.Length;
					int num2 = -1;
					for (int j = 0; j < num; j++)
					{
						string a = materials[j].name.Substring(0, 4);
						if (a == "Body")
						{
							num2 = j;
						}
					}
					materials[num2] = bodyMaterial;
					renderer.materials = materials;
				}
			}
		}

		public virtual bool AddObstacle()
		{
			if (this.obstacle != null)
			{
				return false;
			}
			this.obstacle = base.transform.gameObject.AddComponent<NavMeshObstacle>();
			this.obstacle.size = this.boxSource.size;
			this.obstacle.center = this.boxSource.center;
			this.obstacle.carving = true;
			this.obstacle.carveOnlyStationary = false;
			return true;
		}

		public virtual void RemoveObstacle()
		{
			if (this.obstacle)
			{
				UnityEngine.Object.Destroy(this.obstacle);
			}
		}

		public virtual void StopVehicle()
		{
			WheelCollider[] componentsInChildren = base.GetComponentsInChildren<WheelCollider>();
			foreach (WheelCollider wheelCollider in componentsInChildren)
			{
				wheelCollider.brakeTorque = 10f;
			}
		}

		private void OnEnable()
		{
			this.StopVehicle();
			base.StartCoroutine(this.CheckingIsOnWater());
		}

		private IEnumerator CheckingIsOnWater()
		{
			WaitForSeconds checkingIsOnWater = new WaitForSeconds(0.5f);
			for (;;)
			{
				this.DeepInWater = (this.waterDepth > (base.transform.position + base.transform.up * this.MaxAcceptableWaterHigh).y);
				yield return checkingIsOnWater;
			}
			yield break;
		}

		public virtual void OnCollisionEnter(Collision c)
		{
			this.Effects(c);
			float num = Vector3.Dot(c.relativeVelocity, c.contacts[0].normal);
			if (Mathf.Abs(num) >= this.SpeedForImpact)
			{
				if (c.collider.gameObject.layer == DrivableVehicle.bigDynamicLayerNumber)
				{
					VehicleStatus vehicleStatus = c.collider.transform.GetComponentInChildren<VehicleStatus>();
					if (!vehicleStatus)
					{
						vehicleStatus = c.collider.transform.GetComponentInParent<VehicleStatus>();
					}
					if (vehicleStatus)
					{
						vehicleStatus.OnHit(DamageType.Collision, this.CurrentDriver, num * this.DamagePerSpeed, c.contacts[0].point, (c.contacts[0].point - base.transform.position).normalized, 0f);
					}
				}
				if (this.DamagingFromCollision && (c.collider.gameObject.layer == DrivableVehicle.complexStaticObjectLayerNumber || c.collider.gameObject.layer == DrivableVehicle.staticObjectLayerNumber || c.collider.gameObject.layer == DrivableVehicle.terrainLayerNumber))
				{
					this.vehStatus.OnHit(DamageType.Collision, this.CurrentDriver, num * this.DamagePerSpeed, c.contacts[0].point, c.contacts[0].normal.normalized, 0f);
				}
				this.OnCollisionSpecific(c);
			}
		}

		protected virtual void OnCollisionSpecific(Collision col)
		{
		}

		public virtual void OpenVehicleDoor(VehicleDoor door, bool isGettingIn)
		{
		}

		public virtual Vector3 GetExitPosition(bool toLeft)
		{
			return (!toLeft) ? this.VehiclePoints.EnterFromPositions[0].position : this.VehiclePoints.EnterFromPositions[1].position;
		}

		public virtual bool IsDoorBlockedOffset(LayerMask blockedLayerMask, Transform driver, out Vector3 offset, bool horizontalCheckOnly = true)
		{
			Vector3 vector = base.transform.position + base.transform.up * this.VehicleSpecificPrefab.MaxHeight * 0.5f;
			CapsuleCollider capsuleCollider = PlayerManager.Instance.Player.collider as CapsuleCollider;
			Vector3 vector2 = capsuleCollider.transform.position + capsuleCollider.center + capsuleCollider.transform.up * capsuleCollider.height / 2f;
			UnityEngine.Debug.DrawRay(vector, -base.transform.right * this.ExitLeftMinDistance, Color.cyan, 5f);
			UnityEngine.Debug.DrawRay(vector2, -capsuleCollider.transform.up * capsuleCollider.height, Color.yellow, 5f);
			RaycastHit raycastHit;
			bool flag = !Physics.Raycast(vector, -base.transform.right, out raycastHit, this.ExitLeftMinDistance, blockedLayerMask);
			bool flag2 = horizontalCheckOnly || !Physics.Raycast(vector2, -capsuleCollider.transform.up, out raycastHit, capsuleCollider.height, blockedLayerMask);
			if (flag && flag2)
			{
				offset = -base.transform.right * this.ExitLeftMinDistance;
				return false;
			}
			bool flag3 = Physics.Raycast(vector, base.transform.right, out raycastHit, this.ExitRightMinDistance, blockedLayerMask);
			offset = ((!flag3) ? (base.transform.right * this.ExitRightMinDistance) : (Vector3.up * this.ExitUpMinDistance));
			return true;
		}

		protected virtual void FixedUpdate()
		{
			this.verySlowUpdateProc.ProceedOnFixedUpdate();
			if (this.hitTimer > -1f)
			{
				this.hitTimer -= 2f * Time.deltaTime;
			}
			this.waterDepth = this.WaterSensor.CurrWaterSurfaceHeight;
			if (this.WaterSensor.InWater && this.DeepInWater && this.CurrentDriver != null)
			{
				this.CurrentDriver.Drowning(this.waterDepth, 1f);
				GameEventManager.Instance.Event.VehicleDrawingEvent(this);
			}
			this.DrowningDummyDriver();
			this.CheckLocationOnGround();
		}

		public void ResetDriver()
		{
			this.CurrentDriver = null;
			this.vehStatus.Faction = Faction.NoneFaction;
		}

		protected void DrowningDummyDriver()
		{
			if (!this.DeepInWater)
			{
				return;
			}
			if (!this.DummyDriver)
			{
				return;
			}
			this.DummyDriver.DropRagdoll(null, Vector3.zero, false, true, this.waterDepth);
			this.ResetDriver();
		}

		private void VerySlowUpdate()
		{
			if (!SectorManager.Instance.IsInActiveSector(base.transform.position) && !base.gameObject.CompareTag("Racer") && !base.gameObject.CompareTag("PlayerRacer"))
			{
				this.DestroyVehicle();
			}
		}

		public void DestroyVehicle()
		{
			TrafficDriver componentInChildren = base.GetComponentInChildren<TrafficDriver>();
			if (componentInChildren != null)
			{
				componentInChildren.DestroyVehicle();
			}
			else if (!PoolManager.Instance.ReturnToPool(base.gameObject))
			{
				if (GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log("Shouldn't happen: Vehicle Destroy on DrivableVehicle");
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void ApplyCenterOfMass(Transform newCenterOfMass)
		{
			if (this.MainRigidbody != null && newCenterOfMass != null)
			{
				this.MainRigidbody.centerOfMass = new Vector3(newCenterOfMass.localPosition.x * base.transform.localScale.x, newCenterOfMass.localPosition.y * base.transform.localScale.y, newCenterOfMass.localPosition.z * base.transform.localScale.z);
			}
			this.CalculateInertiaTensor();
		}

		private void CalculateInertiaTensor()
		{
			Quaternion rotation = base.transform.rotation;
			base.transform.rotation = Quaternion.identity;
			Collider component = this.vehStatus.GetComponent<Collider>();
			float x = component.bounds.size.x;
			float y = component.bounds.size.y;
			float z = component.bounds.size.z;
			float mass = this.MainRigidbody.mass;
			float x2 = 0.083f * mass * (z * z + y * y);
			float y2 = 0.083f * mass * (x * x + z * z);
			float z2 = 0.083f * mass * (x * x + y * y);
			this.MainRigidbody.inertiaTensorRotation = Quaternion.identity;
			this.MainRigidbody.inertiaTensor = new Vector3(x2, y2, z2);
			this.MainRigidbody.angularVelocity = Vector3.zero;
			base.transform.rotation = rotation;
		}

		public void Effects(Collision c)
		{
			if (c.contacts.Length < 1)
			{
				return;
			}
			if (c.gameObject.layer == DrivableVehicle.terrainLayerNumber)
			{
				return;
			}
			float num = Mathf.Abs(Vector3.Dot(c.relativeVelocity, c.contacts[0].normal));
			if (c.gameObject.layer == DrivableVehicle.chatacterLayerNumber && num > 7f)
			{
				if (c.gameObject.tag == "Player" && PlayerManager.Instance.Player.IsTransformer)
				{
					PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, TypeOfSound.CarHitCar);
				}
				else
				{
					PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, TypeOfSound.CarHitHuman);
				}
			}
			else if (c.gameObject.layer == DrivableVehicle.staticObjectLayerNumber || c.gameObject.layer == DrivableVehicle.complexStaticObjectLayerNumber)
			{
				PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, (num >= 30f) ? TypeOfSound.StrongCarHit : TypeOfSound.CarHit);
				if (this.controller)
				{
					this.controller.Particles(c);
				}
			}
			else if ((c.gameObject.layer == DrivableVehicle.bigDynamicLayerNumber && num > 7f) || c.gameObject.layer == DrivableVehicle.defaultLayerNumber)
			{
				PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, (num >= 30f) ? TypeOfSound.StrongCarHit : TypeOfSound.CarHitCar);
				if (this.controller)
				{
					this.controller.Particles(c);
				}
			}
			else if (c.gameObject.layer == DrivableVehicle.smallDynamicLayerNumber && num > 7f)
			{
				Rigidbody component = c.gameObject.GetComponent<Rigidbody>();
				if (component)
				{
					float mass = component.mass;
					if (mass < 5f && this.hitTimer <= 0f)
					{
						this.hitTimer = 1f;
						PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, TypeOfSound.CarHitHuman);
					}
				}
				else if (this.hitTimer <= 0f)
				{
					this.hitTimer = 1f;
					PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, (num >= 30f) ? TypeOfSound.StrongCarHit : TypeOfSound.CarHit);
					if (this.controller)
					{
						this.controller.Particles(c);
					}
				}
			}
		}

		private const float MaxSpeedToEnter = 20f;

		private const float MagicConstant = 0.083f;

		private const int BrakeTorqueOnStart = 10;

		private const float relativeSpeedForStrongHit = 30f;

		private const float relativeSpeedForHitSmallDynamic = 7f;

		private const float relativeSpeedForHitBigDynamic = 2f;

		private static int WaterLayerNumber = -1;

		public bool DebugLog;

		public GameObject VehicleSubstrateColider;

		public GameObject Wheels;

		public GameObject VehicleControllerPrefab;

		public VehicleSpecific VehicleSpecificPrefab;

		public VehicleSound SoundsPrefab;

		public GameObject SimpleModel;

		[Header("Tuning")]
		public Renderer[] BodyRenderers;

		public Material[] BodyMaterials;

		public bool Tuning;

		public bool OnGround;

		[Header("Vehicle Acceleration Multiplier")]
		[Range(1f, 10f)]
		public float Acceleration = 1f;

		[Header("MaxSpeed KMH")]
		public float MaxSpeed = 100f;

		public float DamagePerSpeed = 10f;

		public float SpeedForImpact = 7f;

		public bool DamagingFromCollision = true;

		[Space(10f)]
		public DrivableVehicle.VehiclePointsContainer VehiclePoints = new DrivableVehicle.VehiclePointsContainer();

		public WaterSensor WaterSensor;

		public float MaxAcceptableWaterHigh = 0.8f;

		public float ToSitPositionLerpTic = 0.15f;

		[Space(10f)]
		public DriverStatus CurrentDriver;

		[HideInInspector]
		public bool AnimateGetInOut;

		[HideInInspector]
		public bool DeepInWater;

		[HideInInspector]
		public DummyDriver DummyDriver;

		private int bodyMaterialIndex = -1;

		private NavMeshObstacle obstacle;

		protected BoxCollider boxSource;

		private SlowUpdateProc verySlowUpdateProc;

		private float waterDepth = -1000f;

		private float hitTimer = 1f;

		private bool isTransformer;

		private VehicleSource vehicleSource;

		[HideInInspector]
		public VehicleController controller;

		public float ExitRightMinDistance;

		public float ExitLeftMinDistance;

		public float ExitUpMinDistance = 3f;

		protected VehicleController oldController;

		protected VehicleStatus vehStatus;

		private Rigidbody mainRigidbody;

		[Serializable]
		public class VehiclePointsContainer
		{
			public Transform CenterOfMass;

			public Transform DriverPosition;

			public Transform TrafficDriverPosition;

			public Transform[] EnterFromPositions;

			public Transform JumpOutPosition;
		}
	}
}
