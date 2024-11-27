using System;
using System.Collections;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Traffic;
using UnityEngine;

namespace Game.Vehicle
{
	public class DummyDriver : MonoBehaviour
	{
		public bool HaveDriver
		{
			get
			{
				return this.initedDriverModel;
			}
		}

		public HumanoidStatusNPC InitedStatusNPC
		{
			get
			{
				return this.initedStatusNpc;
			}
		}

		public void InitDriver(DrivableVehicle drivableVehicle)
		{
			if (!this.DriverModel)
			{
				return;
			}
			this.driverOnVehicle = true;
			this.initedDriverModel = PoolManager.Instance.GetFromPool(this.DriverModel, drivableVehicle.transform.position, drivableVehicle.transform.rotation);
			this.initedDriverModel.transform.parent = base.transform;
			this.localDrivableVehicle = drivableVehicle;
			if (!this.driverStatusGO)
			{
				this.driverStatusGO = PoolManager.Instance.GetFromPool(this.DriverVehStatusPrefab);
				this.driverStatusGO.transform.parent = this.initedDriverModel.transform.Find("metarig").Find("hips").transform;
				this.driverStatusGO.transform.localPosition = this.localDrivableVehicle.VehicleSpecificPrefab.DriverStatusPosition;
				this.driverStatusGO.transform.localEulerAngles = this.localDrivableVehicle.VehicleSpecificPrefab.DriverStatusRotation;
				PoolManager.Instance.AddBeforeReturnEvent(this.driverStatusGO, delegate(GameObject poolingObject)
				{
					this.driverStatusGO = null;
				});
			}
			this.DriverStatus = this.driverStatusGO.GetComponent<DriverStatus>();
			this.DriverStatus.Init(base.gameObject, drivableVehicle.DriverIsVulnerable);
			this.localDrivableVehicle.SetDummyDriver(this);
			PoolManager.Instance.AddBeforeReturnEvent(this, delegate(GameObject poolingObject)
			{
				if (this.driverStatusGO != null)
				{
					PoolManager.Instance.ReturnToPool(this.driverStatusGO);
				}
			});
			this.DriverDead = false;
			this.driverIdeal = this.localDrivableVehicle.VehicleSpecificPrefab.Skeleton;
			this.CopyTransformRecurse(this.driverIdeal, this.initedDriverModel);
		}

		public void DeInitDriver()
		{
			if (!this.initedDriverModel)
			{
				return;
			}
			PoolManager.Instance.ReturnToPool(this.initedDriverModel);
			this.initedDriverModel = null;
			this.driverIdeal = null;
			this.initedStatusNpc = null;
			if (this.driverStatusGO != null)
			{
				PoolManager.Instance.ReturnToPool(this.driverStatusGO);
			}
		}

		private void CopyTransformRecurse(Transform idealModelTransform, GameObject newModel)
		{
			if (idealModelTransform != null && newModel != null)
			{
				newModel.transform.localPosition = idealModelTransform.localPosition;
				newModel.transform.localRotation = idealModelTransform.localRotation;
				newModel.transform.localScale = idealModelTransform.localScale;
				IEnumerator enumerator = newModel.transform.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						Transform transform = (Transform)obj;
						Transform transform2 = idealModelTransform.Find(transform.name);
						if (transform2)
						{
							this.CopyTransformRecurse(transform2, transform.gameObject);
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		public void DeInitTrafficDriver()
		{
			TrafficDriver componentInChildren = this.localDrivableVehicle.gameObject.GetComponentInChildren<TrafficDriver>();
			if (componentInChildren != null)
			{
				PoolManager.Instance.ReturnToPool(componentInChildren);
			}
		}

		public void DriverDie()
		{
			this.CopyTransformRecurse(this.localDrivableVehicle.VehicleSpecificPrefab.SkeletonDead, this.initedDriverModel);
			this.DriverDead = true;
			this.DeInitTrafficDriver();
			Autopilot componentInChildren = this.localDrivableVehicle.gameObject.GetComponentInChildren<Autopilot>();
			if (componentInChildren != null)
			{
				componentInChildren.DropPassangers();
			}
			this.localDrivableVehicle.StopVehicle();
		}

		public void InitOutOfVehicle(bool force, HitEntity disturber, bool isReplaceOnRagdoll)
		{
			if (!this.initedDriverModel || !this.driverOnVehicle)
			{
				return;
			}
			base.StartCoroutine(this.InitOutOfVehicleEnumerator(force, disturber, isReplaceOnRagdoll));
		}

		public void DropRagdoll(HitEntity disturber, Vector3 direction, bool canWakeUp = true, bool isOnWater = false, float waterHight = 0f)
		{
			if (!this.initedDriverModel || !this.driverOnVehicle)
			{
				return;
			}
			BaseNPC component = PoolManager.Instance.GetFromPool(this.DriverNPC).GetComponent<BaseNPC>();
			HumanoidStatusNPC component2 = component.GetComponent<HumanoidStatusNPC>();
			if (this.driverStatusGO != null)
			{
				component2.Health.Current = this.driverStatusGO.GetComponent<DriverStatus>().Health.Current;
			}
			this.CopyTransformRecurse(this.localDrivableVehicle.VehicleSpecificPrefab.Skeleton, component.RootModel);
			component.transform.position = this.initedDriverModel.transform.position;
			component.transform.rotation = this.initedDriverModel.transform.rotation;
			component.RootModel.transform.localPosition = Vector3.zero;
			component.QuietControllerType = BaseNPC.NPCControllerType.Pedestrian;
			if (!this.DriverDead)
			{
				TrafficManager.Instance.TakePedestrianSlot(component);
			}
			PoolManager.Instance.AddBeforeReturnEvent(component, delegate(GameObject poolingObject)
			{
				TrafficManager.Instance.FreePedestrianSlot();
			});
			if (disturber)
			{
				component.ChangeController(BaseNPC.NPCControllerType.Smart);
				SmartHumanoidController smartHumanoidController = component.CurrentController as SmartHumanoidController;
				component2.OnStatusAlarm(disturber);
				smartHumanoidController.AddPersonalTarget(disturber);
				smartHumanoidController.InitBackToDummyLogic();
				if (disturber.Faction == Faction.Player)
				{
					if (canWakeUp)
					{
						FactionsManager.Instance.PlayerAttackHuman(this.DriverStatus);
					}
					else
					{
						FactionsManager.Instance.CommitedACrime();
					}
				}
			}
			component2.LastWaterHeight = waterHight - 1.6f;
			component2.IsInWater = isOnWater;
			component2.ReplaceOnRagdoll(canWakeUp);
			component2.GetRagdollHips().GetComponent<Rigidbody>().AddForce(direction * 20000f);
			this.localDrivableVehicle.StopVehicle();
			this.DeInitTrafficDriver();
			this.DeInitDriver();
			PoolManager.Instance.ReturnToPool(this);
		}

		private IEnumerator InitOutOfVehicleEnumerator(bool force, HitEntity disturber, bool isReplaceOnRagdoll)
		{
			this.driverOnVehicle = false;
			if (this.localDrivableVehicle.CurrentDriver == null)
			{
				this.localDrivableVehicle.GetVehicleStatus().Faction = Faction.NoneFaction;
			}
			BaseNPC initNPC = PoolManager.Instance.GetFromPool(this.DriverNPC).GetComponent<BaseNPC>();
			initNPC.WaterSensor.Reset();
			if (!this.DriverDead)
			{
				TrafficManager.Instance.TakePedestrianSlot(initNPC);
			}
			BaseControllerNPC npcController;
			initNPC.ChangeController(BaseNPC.NPCControllerType.Smart, out npcController);
			PoolManager.Instance.AddBeforeReturnEvent(initNPC, delegate(GameObject poolingObject)
			{
				initNPC.WaterSensor.Reset();
				TrafficManager.Instance.FreePedestrianSlot();
			});
			SmartHumanoidController smartHumanoidController = npcController as SmartHumanoidController;
			smartHumanoidController.InitBackToDummyLogic();
			Collider[] npcColliders = initNPC.SpecificNpcLinks.ModelColliders;
			Rigidbody npcRigidbody = initNPC.NPCRigidbody;
			this.initedStatusNpc = (initNPC.StatusNpc as HumanoidStatusNPC);
			initNPC.transform.parent = base.transform.parent;
			foreach (Collider collider in npcColliders)
			{
				collider.isTrigger = true;
			}
			npcRigidbody.isKinematic = true;
			if (this.driverStatusGO != null)
			{
				this.initedStatusNpc.Health.Current = this.driverStatusGO.GetComponent<DriverStatus>().Health.Current;
			}
			smartHumanoidController.AnimationController.StartInCar(this.localDrivableVehicle.GetVehicleType(), force, this.DriverDead);
			yield return new WaitForEndOfFrame();
			smartHumanoidController.WeaponController.HideWeapon();
			npcController.gameObject.SetActive(false);
			Transform exitPoint;
			if (!force)
			{
				exitPoint = (this.DriverDead ? this.localDrivableVehicle.VehiclePoints.EnterFromPositions[4] : this.localDrivableVehicle.VehiclePoints.EnterFromPositions[0]);
			}
			else
			{
				exitPoint = this.localDrivableVehicle.VehiclePoints.EnterFromPositions[2];
			}
			initNPC.transform.position = exitPoint.position;
			initNPC.transform.rotation = exitPoint.rotation;
			PoolManager.Instance.ReturnToPool(this.initedDriverModel);
			this.initedDriverModel = null;
			float waitTime;
			if (!this.DriverDead)
			{
				waitTime = ((!force) ? this.getOutAnimationLength : this.localDrivableVehicle.VehicleSpecificPrefab.forceGetOutAnimationLength);
			}
			else
			{
				waitTime = 1.7f;
			}
			yield return new WaitForSeconds(waitTime);
			smartHumanoidController.WeaponController.ShowWeapon();
			npcController.gameObject.SetActive(true);
			foreach (Collider collider2 in npcColliders)
			{
				collider2.isTrigger = false;
			}
			npcRigidbody.isKinematic = false;
			initNPC.transform.parent = null;
			if (!this.DriverDead)
			{
				initNPC.QuietControllerType = BaseNPC.NPCControllerType.Pedestrian;
				smartHumanoidController.InitBackToDummyLogic();
				if (isReplaceOnRagdoll)
				{
					this.initedStatusNpc.ReplaceOnRagdoll(true);
					this.initedStatusNpc.GetRagdollHips().GetComponent<Rigidbody>().AddForce(-base.transform.forward * 20000f);
				}
				else
				{
					initNPC.transform.eulerAngles = new Vector3(0f, initNPC.transform.eulerAngles.y, 0f);
				}
			}
			else
			{
				this.initedStatusNpc.ReplaceOnRagdoll(false);
			}
			if (disturber)
			{
				if (!(this.driverStatusGO == null))
				{
					this.driverStatusGO.GetComponent<DriverStatus>();
				}
				this.initedStatusNpc.OnStatusAlarm(disturber);
				smartHumanoidController.AddPersonalTarget(disturber);
				if (disturber.Faction == Faction.Player && force)
				{
					FactionsManager.Instance.CommitedACrime();
				}
			}
			if (this.DummyExitEvent != null)
			{
				this.DummyExitEvent();
				this.DummyExitEvent = null;
			}
			this.localDrivableVehicle.StopVehicle();
			this.DeInitTrafficDriver();
			this.DeInitDriver();
			PoolManager.Instance.ReturnToPool(this);
			yield break;
		}

		private const float CorpseFallAnimationLength = 1.7f;

		private const float GetOutAlarmRange = 20f;

		private const float DropForce = 20000f;

		public GameObject DriverModel;

		public GameObject DriverNPC;

		public GameObject DriverVehStatusPrefab;

		public float forceGetOutAnimationLength = 3f;

		public float getOutAnimationLength = 2.8f;

		[HideInInspector]
		public bool DriverDead;

		public DriverStatus DriverStatus;

		private Transform driverIdeal;

		private GameObject initedDriverModel;

		private GameObject driverStatusGO;

		private DrivableVehicle localDrivableVehicle;

		private HumanoidStatusNPC initedStatusNpc;

		private bool driverOnVehicle;

		public DummyDriver.DummyEvent DummyExitEvent;

		public delegate void DummyEvent();
	}
}
