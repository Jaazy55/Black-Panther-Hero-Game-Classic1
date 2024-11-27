using System;
using System.Collections;
using System.Collections.Generic;
using Game.GlobalComponent.Qwest;
using Game.PickUps;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class CollectionPickUpsManager : MonoBehaviour
	{
		public static CollectionPickUpsManager Instance
		{
			get
			{
				if (CollectionPickUpsManager._instance == null)
				{
					CollectionPickUpsManager._instance = UnityEngine.Object.FindObjectOfType<CollectionPickUpsManager>();
				}
				return CollectionPickUpsManager._instance;
			}
		}

		public void OnPickupCreate(ObjectRespawner respawner, GameObject pickupObject)
		{
			if (!pickupObject)
			{
				return;
			}
			if (this.pickUpRespawnerDict.ContainsKey(pickupObject))
			{
				this.pickUpRespawnerDict.Remove(pickupObject);
			}
			this.pickUpRespawnerDict.Add(pickupObject, respawner);
			if (this.respawnerPickUpDict.ContainsKey(respawner))
			{
				this.respawnerPickUpDict.Remove(respawner);
			}
			this.respawnerPickUpDict.Add(respawner, pickupObject);
		}

		public void SaveCollections()
		{
			if (this.collectinBools == null)
			{
				return;
			}
			BaseProfile.StoreArray<bool>(this.collectinBools, "Collections");
		}

		public void ElementWasTaken(GameObject elem)
		{
			ObjectRespawner objectRespawner = this.pickUpRespawnerDict[elem];
			for (int i = 0; i < this.collectionRespauners.Length; i++)
			{
				if (this.collectionRespauners[i] == objectRespawner)
				{
					this.collectinBools[i] = true;
					break;
				}
			}
			this.RefreshAmmount();
			CollectionPickup component = objectRespawner.ObjectPrefab.GetComponent<CollectionPickup>();
			if (component == null)
			{
				return;
			}
			CollectionPickUpsManager.CollectionTypes collectionType = component.CollectionType;
			string specificString = string.Concat(new object[]
			{
				"You have collected ",
				this.countAmmount[collectionType],
				" / ",
				this.totalAmmount[collectionType],
				" of ",
				collectionType.ToString()
			});
			InGameLogManager.Instance.RegisterNewMessage(MessageType.Collect, specificString);
			if (this.OnElementTakenEvent != null)
			{
				this.OnElementTakenEvent(collectionType);
			}
			foreach (CollectionPickUpsManager.Reward reward in this.RewardList)
			{
				if (collectionType == reward.collectionType && this.countAmmount[reward.collectionType] == reward.count)
				{
					reward.Action();
				}
			}
		}

		private void LoadCollections()
		{
			try
			{
				bool[] array = BaseProfile.ResolveArray<bool>("Collections");
				if (array.Length == this.collectionRespauners.Length)
				{
					this.collectinBools = array;
				}
				else
				{
					BaseProfile.StoreArray<bool>(new bool[this.collectionRespauners.Length], "Collections");
					this.LoadCollections();
				}
			}
			catch (Exception)
			{
				BaseProfile.StoreArray<bool>(this.collectinBools, "Collections");
				this.LoadCollections();
			}
		}

		private void ResetCollections()
		{
			bool[] values = new bool[this.collectionRespauners.Length];
			BaseProfile.StoreArray<bool>(values, "Collections");
		}

		private void Awake()
		{
			if (this.initialized)
			{
				return;
			}
			this.initialized = true;
			this.collectionRespauners = this.collectionsContainer.GetComponentsInChildren<ObjectRespawner>();
			if (this.restart)
			{
				this.ResetCollections();
			}
			this.collectinBools = new bool[this.collectionRespauners.Length];
			this.LoadCollections();
			for (int i = 0; i < this.collectionRespauners.Length; i++)
			{
				this.collectionRespauners[i].SetCollectionRespawner();
				this.collectionRespauners[i].SetIsTaken(this.collectinBools[i]);
				if (!this.collectinBools[i] && this.respawnerPickUpDict.ContainsKey(this.collectionRespauners[i]))
				{
					PoolManager.Instance.ReturnToPool(this.respawnerPickUpDict[this.collectionRespauners[i]]);
					this.respawnerPickUpDict[this.collectionRespauners[i]].SetActive(false);
				}
			}
			this.RefreshAmmount();
			this.AddRevard();
			if (this.OnManagerInitAction != null)
			{
				this.OnManagerInitAction();
			}
		}

		private void RefreshAmmount()
		{
			IEnumerator enumerator = Enum.GetValues(typeof(CollectionPickUpsManager.CollectionTypes)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					CollectionPickUpsManager.CollectionTypes collectionTypes = (CollectionPickUpsManager.CollectionTypes)obj;
					int num = 0;
					int num2 = 0;
					for (int i = 0; i < this.collectionRespauners.Length; i++)
					{
						if (this.collectinBools[i])
						{
							this.collectionRespauners[i].SetIsTaken(true);
						}
						CollectionPickup component = this.collectionRespauners[i].ObjectPrefab.GetComponent<CollectionPickup>();
						if (!(component == null))
						{
							if (component.CollectionType == collectionTypes)
							{
								num++;
								if (this.collectinBools[i])
								{
									num2++;
								}
							}
						}
					}
					this.totalAmmount[collectionTypes] = num;
					this.countAmmount[collectionTypes] = num2;
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

		private void AddRevard()
		{
			IEnumerator enumerator = Enum.GetValues(typeof(CollectionPickUpsManager.CollectionTypes)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					CollectionPickUpsManager.CollectionTypes collectionTypes = (CollectionPickUpsManager.CollectionTypes)obj;
					CollectionPickUpsManager.CollectionTypes ct = collectionTypes;
					CollectionPickUpsManager.Reward item = new CollectionPickUpsManager.Reward(collectionTypes, this.totalAmmount[collectionTypes], delegate()
					{
						GameEventManager.Instance.Event.PickUpCollectionEvent(ct.ToString());
					});
					this.RewardList.Add(item);
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

		private static CollectionPickUpsManager _instance;

		public bool restart;

		public Transform collectionsContainer;

		public ObjectRespawner[] collectionRespauners;

		private bool[] collectinBools;

		public Dictionary<GameObject, ObjectRespawner> pickUpRespawnerDict = new Dictionary<GameObject, ObjectRespawner>();

		public Dictionary<ObjectRespawner, GameObject> respawnerPickUpDict = new Dictionary<ObjectRespawner, GameObject>();

		public Dictionary<CollectionPickUpsManager.CollectionTypes, int> totalAmmount = new Dictionary<CollectionPickUpsManager.CollectionTypes, int>();

		public Dictionary<CollectionPickUpsManager.CollectionTypes, int> countAmmount = new Dictionary<CollectionPickUpsManager.CollectionTypes, int>();

		public Action OnManagerInitAction;

		public CollectionPickUpsManager.OnElementTaken OnElementTakenEvent;

		private bool initialized;

		private List<CollectionPickUpsManager.Reward> RewardList = new List<CollectionPickUpsManager.Reward>();

		public class Reward
		{
			public Reward(CollectionPickUpsManager.CollectionTypes type, int countForStart, CollectionPickUpsManager.Reward.RewardAction actionForRewardDelegate)
			{
				this.collectionType = type;
				this.count = countForStart;
				this.Action = actionForRewardDelegate;
			}

			public CollectionPickUpsManager.CollectionTypes collectionType;

			public int count;

			public CollectionPickUpsManager.Reward.RewardAction Action;

			public delegate void RewardAction();
		}

		public enum CollectionTypes
		{
			Biceps,
			Rope,
			Hook
		}

		public delegate void OnElementTaken(CollectionPickUpsManager.CollectionTypes type);
	}
}
