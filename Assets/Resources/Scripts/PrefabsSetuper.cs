using System;
using System.Collections.Generic;
using Game.GlobalComponent;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabsSetuper : MonoBehaviour
{
	public void SetMeUp()
	{
		if (this.lastCreated.Count != 0)
		{
			this.lastCreated.Clear();
		}
		foreach (Transform transform in this.Holders)
		{
			GameObject gameObject = this.Prefab;
			ObjectRespawner component = transform.GetComponent<ObjectRespawner>();
			if (component != null)
			{
				gameObject = component.ObjectPrefab;
			}
			if (!(gameObject == null))
			{
				GameObject item = UnityEngine.Object.Instantiate<GameObject>(gameObject, transform.position, transform.rotation, transform);
				this.lastCreated.Add(item);
			}
		}
	}

	public void DeleteLastCreated()
	{
		foreach (GameObject gameObject in this.lastCreated)
		{
			if (gameObject != null)
			{
				UnityEngine.Object.DestroyImmediate(gameObject);
			}
		}
		this.lastCreated.Clear();
	}

	public GameObject Prefab;

	public Transform[] Holders;

	[InspectorButton("SetMeUp")]
	public bool SetUp;

	[InspectorButton("DeleteLastCreated")]
	public bool Clear;

	private List<GameObject> lastCreated = new List<GameObject>();
}
