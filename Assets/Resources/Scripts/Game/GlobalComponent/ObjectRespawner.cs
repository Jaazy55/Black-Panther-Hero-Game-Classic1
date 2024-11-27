using System;
using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.Enemy;
using Game.PickUps;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class ObjectRespawner : MonoBehaviour
	{
		public void SetCollectionRespawner()
		{
			this.ObjectType = RespawnedObjectType.CollectionItem;
		}

		public void SetIsTaken(bool val)
		{
			this.isTaken = val;
		}

		private void Awake()
		{
			if (this.awaked)
			{
				return;
			}
			UnityEngine.Object.DestroyImmediate(base.transform.GetChild(0).gameObject);
			this.lostControllTime = Time.time - this.RespawnTime;
			this.awaked = true;
			if (this.ObjectPrefab.GetComponent<CollectionPickup>() != null)
			{
				this.SetCollectionRespawner();
				if (this.DebugVal)
				{
					UnityEngine.Debug.LogFormat(this, "isTaken = " + this.isTaken, new object[0]);
				}
			}
		}

		protected void OnEnable()
		{
			if (!this.awaked)
			{
				this.Awake();
			}
			if (this.controlledObject == null && Time.time >= this.lostControllTime + this.RespawnTime)
			{
				if (this.ObjectType == RespawnedObjectType.CollectionItem && this.isTaken)
				{
					return;
				}
				this.controlledObject = PoolManager.Instance.GetFromPool(this.ObjectPrefab, base.transform.position, base.transform.rotation);
				HitEntity controlledHitEntity = null;
				PickUp controlledPickup = null;
				if (this.ObjectType == RespawnedObjectType.Entity)
				{
					controlledHitEntity = this.controlledObject.GetComponentInChildren<HitEntity>();
					BaseNPC component = this.controlledObject.GetComponent<BaseNPC>();
					if (component != null)
					{
						component.QuietControllerType = BaseNPC.NPCControllerType.Simple;
						BaseControllerNPC baseControllerNPC;
						component.ChangeController((!this.SpawnGuardNpc) ? BaseNPC.NPCControllerType.Simple : BaseNPC.NPCControllerType.Smart, out baseControllerNPC);
						SmartHumanoidController smartHumanoidController = baseControllerNPC as SmartHumanoidController;
						if (smartHumanoidController != null)
						{
							if (!this.SpawnGuardNpc)
							{
								smartHumanoidController.InitBackToDummyLogic();
							}
							smartHumanoidController.IsFCKingManiac = (this.SpawnManiacNpc || this.SpawnGuardNpc);
						}
					}
				}
				else
				{
					controlledPickup = this.controlledObject.GetComponent<PickUp>();
				}
				PoolManager.Instance.AddBeforeReturnEvent(this.controlledObject, delegate(GameObject poolingObject)
				{
					bool flag = (this.ObjectType != RespawnedObjectType.Entity) ? PickUpManager.Instance.PickupWasTaked(controlledPickup) : EntityManager.Instance.EntityWasKilled(controlledHitEntity);
					if (flag)
					{
						this.lostControllTime = Time.time;
					}
					this.controlledObject = null;
				});
			}
			if (this.controlledObject != null)
			{
				this.controlledObject.transform.parent = base.transform.parent;
				this.controlledObject.SetActive(true);
			}
			if (this.ObjectType == RespawnedObjectType.CollectionItem)
			{
				CollectionPickUpsManager.Instance.OnPickupCreate(this, this.controlledObject);
			}
			if (this.DebugVal)
			{
				UnityEngine.Debug.LogFormat(this.controlledObject, "controlledObject = " + this.controlledObject, new object[0]);
				UnityEngine.Debug.LogFormat(this, "respawner = " + this, new object[0]);
			}
		}

		public GameObject ObjectPrefab;

		public RespawnedObjectType ObjectType;

		public float RespawnTime = 60f;

		[Tooltip("Заспавнится в виде смарта")]
		public bool SpawnGuardNpc;

		[Tooltip("Будет пиздеть всех \"не друзей\", даже нейтралов")]
		public bool SpawnManiacNpc;

		[Space(10f)]
		public bool isTaken;

		public bool DebugVal;

		protected GameObject controlledObject;

		protected float lostControllTime;

		private bool awaked;
	}
}
