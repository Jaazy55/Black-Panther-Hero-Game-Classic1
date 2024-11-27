using System;
using System.Collections.Generic;
using Game.Traffic;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class StashManager : MonoBehaviour
	{
		public static StashManager Instance
		{
			get
			{
				StashManager result;
				if ((result = StashManager.instance) == null)
				{
					result = (StashManager.instance = UnityEngine.Object.FindObjectOfType<StashManager>());
				}
				return result;
			}
		}

		public void UpdateObjects()
		{
			PseudoDynamicObject[] componentsInChildren = this.RootCity.GetComponentsInChildren<PseudoDynamicObject>();
			ObjectRespawner[] array = UnityEngine.Object.FindObjectsOfType<ObjectRespawner>();
			TrafficLight[] array2 = UnityEngine.Object.FindObjectsOfType<TrafficLight>();
			EffectArea[] array3 = UnityEngine.Object.FindObjectsOfType<EffectArea>();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				list.Add(componentsInChildren[i].gameObject);
			}
			for (int j = 0; j < array.Length; j++)
			{
				list.Add(array[j].gameObject);
			}
			for (int k = 0; k < array2.Length; k++)
			{
				list.Add(array2[k].gameObject);
			}
			for (int l = 0; l < array3.Length; l++)
			{
				list.Add(array3[l].gameObject);
			}
			List<GameObject> list2 = new List<GameObject>();
			for (int m = 0; m < this.ObjectsToManage.Length; m++)
			{
				if (list.Contains(this.ObjectsToManage[m]))
				{
					list.Remove(this.ObjectsToManage[m]);
				}
				if (this.ObjectsToManage[m] != null)
				{
					list2.Add(this.ObjectsToManage[m]);
				}
			}
			list2.AddRange(list);
			this.ObjectsToManage = list2.ToArray();
			UnityEngine.Debug.LogFormat("Added {0}/{1} Managing objects found. \nManaging objects: {2}", new object[]
			{
				list.Count,
				componentsInChildren.Length,
				this.ObjectsToManage.Length
			});
		}

		private void Awake()
		{
			if (StashManager.instance == null)
			{
				StashManager.instance = this;
			}
		}

		private void Start()
		{
			HashSet<int> hashSet = new HashSet<int>(SectorManager.Instance.GetAllActiveSectors());
			for (int i = 0; i < this.ObjectsToManage.Length; i++)
			{
				if (!(this.ObjectsToManage[i] == null))
				{
					GameObject gameObject = this.ObjectsToManage[i];
					int sector = SectorManager.Instance.GetSector(gameObject.transform.position);
					List<GameObject> list;
					if (!this.objectsMap.TryGetValue(sector, out list))
					{
						list = new List<GameObject>();
						this.objectsMap.Add(sector, list);
					}
					list.Add(gameObject);
					gameObject.SetActive(hashSet.Contains(sector));
				}
			}
			SectorManager.Instance.AddOnActivateListener(new SectorManager.SectorStatusChange(this.SectorsToUnstash));
			SectorManager.Instance.AddOnDeactivateListener(new SectorManager.SectorStatusChange(this.SectorsToStash));
			this.ObjectsToManage = null;
		}

		private void ToggleSectorsObjects(int[] sectors, bool toggle)
		{
			for (int i = 0; i < sectors.Length; i++)
			{
				List<GameObject> list;
				if (this.objectsMap.TryGetValue(sectors[i], out list))
				{
					for (int j = 0; j < list.Count; j++)
					{
						list[j].SetActive(toggle);
					}
				}
			}
		}

		private void SectorsToUnstash(int[] sectors)
		{
			this.ToggleSectorsObjects(sectors, true);
		}

		private void SectorsToStash(int[] sectors)
		{
			this.ToggleSectorsObjects(sectors, false);
		}

		private static StashManager instance;

		public GameObject RootCity;

		public GameObject[] ObjectsToManage;

		private IDictionary<int, List<GameObject>> objectsMap = new Dictionary<int, List<GameObject>>();
	}
}
