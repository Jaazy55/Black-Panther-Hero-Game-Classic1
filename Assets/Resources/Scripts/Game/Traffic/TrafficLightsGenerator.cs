using System;
using UnityEngine;

namespace Game.Traffic
{
	public class TrafficLightsGenerator : MonoBehaviour
	{
		public void GenerateTrafficLightsPacks()
		{
			if (this.VehicleNodesContainer == null)
			{
				throw new Exception("Vehicle nodes container not found!");
			}
			this.InitTrafficLightsContainer();
			this.SpawnTrafficLightsPacks();
		}

		public void GenerateTrafficLightsOnWorkpiece()
		{
			this.InitTrafficLightsContainer();
			this.SpawnTrafficLightsWorkpieces();
		}

		public void ClearTrafficLightsLinks()
		{
			TrafficLight[] array = UnityEngine.Object.FindObjectsOfType<TrafficLight>();
			if (array == null)
			{
				return;
			}
			foreach (TrafficLight trafficLight in array)
			{
				foreach (TrafficLight.LightObjects lightObjects in trafficLight.LightAndObjects)
				{
					lightObjects.Objects.RemoveAll((GameObject x) => x == null);
				}
			}
		}

		private void InitTrafficLightsContainer()
		{
			this.trafficLightsContainer = GameObject.Find("Traffic Lights");
			if (this.trafficLightsContainer != null)
			{
				int childCount = this.trafficLightsContainer.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					UnityEngine.Object.DestroyImmediate(this.trafficLightsContainer.transform.GetChild(0).gameObject);
				}
			}
			else
			{
				this.trafficLightsContainer = new GameObject
				{
					name = "Traffic Lights"
				};
			}
		}

		private void SpawnTrafficLightsPacks()
		{
			Node[] componentsInChildren = this.VehicleNodesContainer.GetComponentsInChildren<Node>();
			foreach (Node node in componentsInChildren)
			{
				if (node.NodeLinks.Count >= 3)
				{
					GameObject original = (node.NodeLinks.Count != 3) ? this.FourthTrafficLights : this.TripleTrafficLights;
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, node.transform.position, Quaternion.identity);
					gameObject.transform.parent = this.trafficLightsContainer.transform;
					gameObject.name = node.name + "_" + gameObject.name;
				}
			}
		}

		private void SpawnTrafficLightsWorkpieces()
		{
			GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
			bool flag = false;
			foreach (GameObject gameObject in array)
			{
				if (gameObject.name == this.WorkpieceName)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.ConfigurateWorkpiecePrefab);
					gameObject2.GetComponent<TrafficLight>().RedFirst = flag;
					flag = !flag;
					Transform transform = gameObject2.transform.Find(this.WorkpieceName);
					gameObject2.transform.parent = this.trafficLightsContainer.transform;
					transform.parent = gameObject2.transform.parent;
					gameObject2.transform.parent = transform;
					transform.position = gameObject.transform.position;
					transform.rotation = gameObject.transform.rotation;
					gameObject2.transform.parent = transform.parent;
					transform.parent = gameObject2.transform;
					gameObject.SetActive(false);
				}
			}
		}

		private const string TrafficLightsGameObjectName = "Traffic Lights";

		public GameObject VehicleNodesContainer;

		[Separator("For Traffic Light On Nodes")]
		public GameObject FourthTrafficLights;

		public GameObject TripleTrafficLights;

		[Separator("For Traffic Lights On Workpieces")]
		public string WorkpieceName;

		public GameObject ConfigurateWorkpiecePrefab;

		private GameObject trafficLightsContainer;
	}
}
