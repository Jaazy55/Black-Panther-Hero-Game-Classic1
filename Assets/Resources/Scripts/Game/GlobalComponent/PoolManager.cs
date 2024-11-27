using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Managers;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class PoolManager : MonoBehaviour
	{
		public static PoolManager Instance
		{
			get
			{
				if (PoolManager.instance == null)
				{
					throw new Exception("PoolManager is not initialized");
				}
				return PoolManager.instance;
			}
		}

		public int GetItemInUse<T>(T[] items) where T : Component
		{
			int num = 0;
			int count = this.activeItems.Count;
			int num2 = 0;
			while (num2 < count && num < items.Length)
			{
				T component = this.activeItems[num2].GetComponent<T>();
				if (component != null)
				{
					items[num] = component;
					num++;
				}
				num2++;
			}
			return num;
		}

		public T GetFromPool<T>(T prefabComponent) where T : Component
		{
			return this.GetFromPool(prefabComponent.gameObject, Vector3.zero, Quaternion.identity).GetComponent<T>();
		}

		public T GetFromPool<T>(T prefabComponent, Vector3 position, Quaternion rotation) where T : Component
		{
			return this.GetFromPool(prefabComponent.gameObject, position, rotation).GetComponent<T>();
		}

		public GameObject GetFromPool(GameObject prefab)
		{
			return this.GetFromPool(prefab, Vector3.zero, Quaternion.identity);
		}

		public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			this.InitPoolingPrefab(prefab, 2);
			return this.GetFromPoolNotInitable(prefab.name, position, rotation);
		}

		public void InitPoolingPrefab(Component prefab, int initialCount = 2)
		{
			this.InitPoolingPrefab(prefab.gameObject, initialCount);
		}

		public void InitPoolingPrefab(GameObject prefab, int initialCount = 2)
		{
			if (!this.storage.ContainsKey(prefab.name))
			{
				if (prefab.name.Contains("(Clone)"))
				{
					throw new Exception("Prefab based objects are only allowed");
				}
				PoolManager.PoolConfig poolConfig = new PoolManager.PoolConfig();
				poolConfig.Prefab = prefab;
				poolConfig.InitialCount = initialCount;
				Component[] components = prefab.GetComponents<Component>();
				foreach (Component component in components)
				{
					if (component is IInitable)
					{
						poolConfig.InitableComponents.Add(component.GetType());
					}
				}
				this.InitCustomConfig(poolConfig);
			}
		}

		public GameObject GetFromPoolNotInitable(string poolName, Vector3 position, Quaternion rotation)
		{
			if (this.storage.ContainsKey(poolName))
			{
				List<GameObject> list = this.storage[poolName];
				PoolManager.PoolConfig poolConfig = this.poolConfigsByName[poolName];
				if (list.Count == 0)
				{
					PoolManager.PoolFillRequest request = new PoolManager.PoolFillRequest
					{
						Config = poolConfig,
						VacantObjects = list,
						ToFillCount = 1
					};
					this.CompleteRequest(request);
				}
				if (list.Count > 0)
				{
					GameObject gameObject = list[list.Count - 1];
					RectTransform rectTransform = gameObject.transform as RectTransform;
					if (rectTransform != null)
					{
						rectTransform.SetParent(null, false);
					}
					else
					{
						gameObject.transform.parent = null;
					}
					gameObject.transform.position = position;
					gameObject.transform.rotation = rotation;
					this.itemsInUse.Add(gameObject, poolName);
					this.activeItems.Add(gameObject);
					if (this.IsDebug)
					{
						this.debugItemsInUse.Add(gameObject, Time.timeSinceLevelLoad);
					}
					list.Remove(gameObject);
					if (list.Count == 0)
					{
						PoolManager.PoolFillRequest item = new PoolManager.PoolFillRequest
						{
							Config = poolConfig,
							VacantObjects = list,
							ToFillCount = 3
						};
						this.fillRequests.Add(item);
					}
					poolConfig.ItemsInUse++;
					gameObject.SetActive(true);
					foreach (Type type in poolConfig.InitableComponents)
					{
						Component[] components = gameObject.GetComponents(type);
						if (components != null)
						{
							foreach (Component component in components)
							{
								IInitable initable = component as IInitable;
								if (initable != null)
								{
									initable.Init();
								}
							}
						}
					}
					this.ItemInUseCount++;
					return gameObject;
				}
			}
			throw new Exception("No such pool named " + poolName);
		}

		public T GetFromPool<T>(GameObject prefab) where T : Component
		{
			return this.GetFromPool<T>(prefab.GetComponent<T>());
		}

		public T GetFromPoolNotInitable<T>(string poolName, Vector3 position, Quaternion rotation) where T : Component
		{
			return this.GetFromPoolNotInitable(poolName, position, rotation).GetComponent<T>();
		}

		public bool AddBeforeReturnEvent(Component pooledObject, PoolEventProcessor.PoolEvent poolEvent)
		{
			return this.AddBeforeReturnEvent(pooledObject.gameObject, poolEvent);
		}

		public bool AddBeforeReturnEvent(GameObject gameObject, PoolEventProcessor.PoolEvent poolEvent)
		{
			if (gameObject == null)
			{
				return false;
			}
			if (this.itemsInUse.ContainsKey(gameObject))
			{
				this.eventProcessor.AddBeforeReturnToPoolEvent(gameObject, poolEvent);
				return true;
			}
			return false;
		}

		public bool ReturnToPool(GameObject o)
		{
			if (o != null && this.itemsInUse.ContainsKey(o))
			{
				this.eventProcessor.InvokeBeforeReturnToPoolEvents(o);
				o.SetActive(false);
				string key = this.itemsInUse[o];
				PoolManager.PoolConfig poolConfig = this.poolConfigsByName[key];
				poolConfig.ItemsInUse--;
				foreach (Type type in poolConfig.InitableComponents)
				{
					Component[] components = o.GetComponents(type);
					if (components != null)
					{
						foreach (Component component in components)
						{
							IInitable initable = component as IInitable;
							if (initable != null)
							{
								initable.DeInit();
							}
						}
					}
				}
				this.itemsInUse.Remove(o);
				this.activeItems.Remove(o);
				if (this.IsDebug)
				{
					this.debugItemsInUse.Remove(o);
				}
				this.storage[key].Add(o);
				RectTransform rectTransform = o.transform as RectTransform;
				if (rectTransform != null)
				{
					rectTransform.SetParent(poolConfig.Wrapper.transform, false);
				}
				else
				{
					o.transform.parent = poolConfig.Wrapper.transform;
				}
				o.transform.position = Vector3.zero;
				this.ItemInUseCount--;
				return true;
			}
			return o != null && o.transform.IsChildOf(base.transform);
		}

		public PoolManager.PoolConfig GetConfig(GameObject prefab)
		{
			return this.GetConfig(prefab.name);
		}

		public PoolManager.PoolConfig GetConfig(string poolName)
		{
			PoolManager.PoolConfig poolConfig;
			this.poolConfigsByName.TryGetValue(poolName, out poolConfig);
			return null;
		}

		public bool ReturnToPool(Component o)
		{
			return this.ReturnToPool(o.gameObject);
		}

		public bool ReturnToPoolWithDelay(Component poolingObject, float timeDelay)
		{
			return this.ReturnToPoolWithDelay(poolingObject.gameObject, timeDelay);
		}

		public bool ReturnToPoolWithDelay(GameObject o, float timeDelay)
		{
			if (this.itemsInUse.ContainsKey(o) && base.gameObject.activeInHierarchy)
			{
				base.StartCoroutine(this.ReturnToPoolEnumerator(o, timeDelay));
				return true;
			}
			return false;
		}

		public GameObject PrefabOf(GameObject pooledObject)
		{
			if (pooledObject != null && this.itemsInUse.ContainsKey(pooledObject))
			{
				string key = this.itemsInUse[pooledObject];
				PoolManager.PoolConfig poolConfig = this.poolConfigsByName[key];
				return poolConfig.Prefab;
			}
			return null;
		}

		private IEnumerator ReturnToPoolEnumerator(GameObject o, float timeDelay)
		{
			yield return new WaitForSeconds(timeDelay);
			this.ReturnToPool(o);
			yield break;
		}

		private void Awake()
		{
			if (PoolManager.instance == null)
			{
				PoolManager.instance = this;
			}
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.DebugSlowUpdate), 10f);
			this.autoFillUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.AutoFillSlowUpdate), 0.6f);
		}

		private void DebugSlowUpdate()
		{
			if (this.IsDebug)
			{
				this.DebugArray = this.debugItemsInUse.Keys.ToArray<GameObject>();
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<GameObject, float> keyValuePair in this.debugItemsInUse)
				{
					stringBuilder.Append(keyValuePair.Value).Append(" ").Append(keyValuePair.Key.ToString()).Append("\n");
				}
				if (GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log(stringBuilder.ToString());
				}
			}
		}

		private void AutoFillSlowUpdate()
		{
			if (this.fillRequests.Count == 0)
			{
				return;
			}
			PoolManager.PoolFillRequest poolFillRequest = this.fillRequests[0];
			int num = this.CompleteRequest(poolFillRequest);
			poolFillRequest.ToFillCount -= num;
			if (poolFillRequest.ToFillCount <= 0)
			{
				this.fillRequests.Remove(poolFillRequest);
			}
		}

		private int CompleteRequest(PoolManager.PoolFillRequest request)
		{
			List<GameObject> vacantObjects = request.VacantObjects;
			PoolManager.PoolConfig config = request.Config;
			if (config.ItemsInUse < config.MaximumCount)
			{
				int num = Mathf.Min(config.MaximumCount, vacantObjects.Count + config.ItemsInUse + 1);
				int num2 = 0;
				for (int i = 0; i < num - (config.ItemsInUse + vacantObjects.Count); i++)
				{
					GameObject item = this.InitObject(config);
					vacantObjects.Add(item);
					num2++;
				}
				return num2;
			}
			UnityEngine.Debug.LogWarning(string.Format("PoolManager: pool, named {0} exceed maximum object count {1}", config.Prefab.name, config.MaximumCount));
			return 0;
		}

		private void InitCustomConfig(PoolManager.PoolConfig config)
		{
			string name = config.Prefab.name;
			GameObject gameObject = new GameObject(name + "_Pool");
			config.Wrapper = gameObject;
			RectTransform rectTransform = gameObject.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(base.transform, false);
			}
			else
			{
				gameObject.transform.parent = base.transform;
			}
			gameObject.transform.position = Vector3.zero;
			List<GameObject> list = new List<GameObject>(config.InitialCount);
			this.storage.Add(name, list);
			for (int i = 0; i < config.InitialCount; i++)
			{
				GameObject item = this.InitObject(config);
				list.Add(item);
			}
			this.poolConfigsByName.Add(name, config);
		}

		private GameObject InitObject(PoolManager.PoolConfig config)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(config.Prefab);
			RectTransform rectTransform = gameObject.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(config.Wrapper.transform, false);
			}
			else
			{
				gameObject.transform.parent = config.Wrapper.transform;
			}
			gameObject.transform.position = Vector3.zero;
			gameObject.SetActive(false);
			this.TotalItemsCount++;
			return gameObject;
		}

		private void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
			this.autoFillUpdateProc.ProceedOnFixedUpdate();
		}

		private static PoolManager instance;

		public bool IsDebug;

		[Separator("Debug components")]
		public GameObject[] DebugArray;

		public int TotalItemsCount;

		public int ItemInUseCount;

		private IDictionary<string, List<GameObject>> storage = new Dictionary<string, List<GameObject>>();

		private IDictionary<GameObject, string> itemsInUse = new Dictionary<GameObject, string>();

		private List<GameObject> activeItems = new List<GameObject>();

		private IDictionary<GameObject, float> debugItemsInUse = new Dictionary<GameObject, float>();

		private IDictionary<string, PoolManager.PoolConfig> poolConfigsByName = new Dictionary<string, PoolManager.PoolConfig>();

		private readonly List<PoolManager.PoolFillRequest> fillRequests = new List<PoolManager.PoolFillRequest>();

		private PoolEventProcessor eventProcessor = new PoolEventProcessor();

		private SlowUpdateProc slowUpdateProc;

		private SlowUpdateProc autoFillUpdateProc;

		[Serializable]
		public class PoolConfig
		{
			public GameObject Wrapper
			{
				get
				{
					return this.wrapper;
				}
				set
				{
					this.wrapper = value;
				}
			}

			public int ItemsInUse
			{
				get
				{
					return this.itemsInUse;
				}
				set
				{
					this.itemsInUse = value;
				}
			}

			public GameObject Prefab;

			public int InitialCount = 2;

			public int MaximumCount = 100;

			public List<Type> InitableComponents = new List<Type>();

			private GameObject wrapper;

			private int itemsInUse;
		}

		private class PoolFillRequest
		{
			public List<GameObject> VacantObjects;

			public PoolManager.PoolConfig Config;

			public int ToFillCount;
		}
	}
}
