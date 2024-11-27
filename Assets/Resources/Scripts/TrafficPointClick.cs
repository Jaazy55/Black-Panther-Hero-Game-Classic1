using System;
using System.Collections.Generic;
using Game.Traffic;
using UnityEngine;

[ExecuteInEditMode]
public class TrafficPointClick : MonoBehaviour
{
	public static TrafficPointClick Instance
	{
		get
		{
			if (TrafficPointClick.instance == null)
			{
				TrafficPointClick.instance = UnityEngine.Object.FindObjectOfType<TrafficPointClick>();
				if (TrafficPointClick.instance == null)
				{
					UnityEngine.Debug.LogError("TrafficManagerBCKP/TrafficPointClick отсутствует на сцене");
				}
			}
			return TrafficPointClick.instance;
		}
	}

	public void CtrlZ()
	{
		if (this.tmpArray.Count > 0)
		{
			UnityEngine.Object.DestroyImmediate(this.tmpArray[this.tmpArray.Count - 1]);
			this.tmpArray.Remove(this.tmpArray[this.tmpArray.Count - 1]);
			if (this.tmpArray.Count <= 0)
			{
				return;
			}
			GameObject gameObject = this.tmpArray[this.tmpArray.Count - 1];
			if (gameObject.transform.parent.name == "VehicleNodes")
			{
				this.CarsNode = gameObject;
			}
			if (gameObject.transform.parent.name == "PedestrianNodes")
			{
				this.PedestrainNode = gameObject;
			}
		}
	}

	public void ClearLastLinks()
	{
		if (this.tmpArray.Count > 0)
		{
			this.tmpArray[this.tmpArray.Count - 1].GetComponent<Node>().NodeLinks.Clear();
		}
	}

	public GameObject CarsNode;

	public GameObject PedestrainNode;

	public static bool SelectCarTraffic;

	public static bool SelectPedestrianTraffic;

	public static bool SelectPedestrianNodeNet;

	public static int netLenth = 5;

	public static float distanceBetweenNodes = 10f;

	public static bool connectNextNode = true;

	public bool CarTrafficSelected;

	public bool PedestrianTrafficSelected;

	public bool PedestrianNodeNetSelected;

	public int NodeNetLenth = 5;

	public float DistanceBetweenNodes = 5f;

	public bool ConnectNextNode = true;

	[InspectorButton("CtrlZ")]
	public string DeleteLastPoint = string.Empty;

	[InspectorButton("ClearLastLinks")]
	public string ClearLastPoint = string.Empty;

	public List<GameObject> tmpArray = new List<GameObject>();

	public List<GameObject> pedestrianList = new List<GameObject>();

	private bool selectCarTrafficCurrState;

	private bool selectPedestrainTrafficCurrState;

	private bool selectPedestrianNodeNetCurrState;

	private static TrafficPointClick instance;
}
