using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Traffic
{
	public class TrafficLight : MonoBehaviour
	{
		public void ActivateLight(TrafficLight.TrafficLightColor trafficLightType)
		{
			if (this.currentTrafficLight != TrafficLight.TrafficLightColor.None)
			{
				this.SetLightObjectsStatus(this.currentTrafficLight, false);
			}
			this.SetLightObjectsStatus(trafficLightType, true, out this.currentSignalLength);
			this.currentTrafficLight = trafficLightType;
			this.lastSignalChangeTime = Time.time;
		}

		private void Awake()
		{
			this.LightAndObjects[0].ActiveTime /= 2f;
			this.LightAndObjects[this.LightAndObjects.Length - 1].ActiveTime /= 2f;
			foreach (TrafficLight.LightObjects lightObjects in this.LightAndObjects)
			{
				this.cycleTime += lightObjects.ActiveTime;
				foreach (GameObject gameObject in lightObjects.Objects)
				{
					gameObject.SetActive(false);
				}
			}
		}

		private void OnEnable()
		{
			int num = (int)Mathf.Floor(Time.time / this.cycleTime);
			float phaseTime = Time.time % this.cycleTime;
			float num2;
			TrafficLight.TrafficLightColor trafficLightType = this.CalculateCurrentColor(phaseTime, num % 2 == 0, out num2, out this.redPhase);
			this.ActivateLight(trafficLightType);
			this.currentSignalLength = num2;
		}

		private void FixedUpdate()
		{
			if (Time.time >= this.lastSignalChangeTime + this.currentSignalLength)
			{
				this.NextSignal();
			}
		}

		private void NextSignal()
		{
			int num = this.CalculateNextLightIndex();
			if (num < 0 || num > this.LightAndObjects.Length - 1)
			{
				this.redPhase = !this.redPhase;
				this.lastSignalChangeTime = Time.time;
			}
			else
			{
				this.ActivateLight((TrafficLight.TrafficLightColor)num);
			}
		}

		private int CalculateNextLightIndex()
		{
			int num = (!this.redPhase) ? -1 : 1;
			return (int)(this.currentTrafficLight + num);
		}

		private void SetLightObjectsStatus(TrafficLight.TrafficLightColor trafficLightType, bool activate)
		{
			float num;
			this.SetLightObjectsStatus(trafficLightType, activate, out num);
		}

		private void SetLightObjectsStatus(TrafficLight.TrafficLightColor trafficLightType, bool activate, out float signalLength)
		{
			List<GameObject> list = null;
			signalLength = 0f;
			foreach (TrafficLight.LightObjects lightObjects in this.LightAndObjects)
			{
				if (lightObjects.TrafficLight == trafficLightType)
				{
					list = lightObjects.Objects;
					signalLength = lightObjects.ActiveTime;
					break;
				}
			}
			if (list != null && list.Count > 0)
			{
				foreach (GameObject gameObject in list)
				{
					gameObject.SetActive(activate);
				}
			}
		}

		private TrafficLight.TrafficLightColor CalculateCurrentColor(float phaseTime, bool evenCycle, out float signalLength, out bool localRedPhase)
		{
			localRedPhase = ((!this.RedFirst) ? (!evenCycle) : evenCycle);
			int num = (!localRedPhase) ? -1 : 1;
			int num2 = (!localRedPhase) ? (this.LightAndObjects.Length - 1) : 0;
			float num3 = 0f;
			for (;;)
			{
				if (phaseTime < num3 + this.LightAndObjects[num2].ActiveTime)
				{
					int num4 = num2 + num;
					if (num4 < 0 || num4 > this.LightAndObjects.Length - 1 || phaseTime > num3 - this.LightAndObjects[num4].ActiveTime)
					{
						break;
					}
				}
				num3 += this.LightAndObjects[num2].ActiveTime;
				num2 += num;
			}
			TrafficLight.TrafficLightColor trafficLight = this.LightAndObjects[num2].TrafficLight;
			signalLength = this.LightAndObjects[num2].ActiveTime - (phaseTime - num3);
			return trafficLight;
		}

		private void OnDrawGizmos()
		{
			if (!TrafficLight.IsDebug)
			{
				return;
			}
			int num = LayerMask.NameToLayer("TrafficPseudoObstacles");
			foreach (Transform transform in base.GetComponentsInChildren<Transform>())
			{
				if (transform.gameObject.layer == num)
				{
					Gizmos.matrix = transform.localToWorldMatrix;
					if (Application.isEditor)
					{
						Gizmos.color = ((!this.RedFirst) ? Color.green : Color.red);
					}
					if (Application.isPlaying)
					{
						Gizmos.color = this.colorsByType[this.currentTrafficLight];
					}
					BoxCollider component = transform.GetComponent<BoxCollider>();
					Gizmos.DrawCube(component.center, component.size);
				}
			}
		}

		private const string TrafficObstaclesLayerName = "TrafficPseudoObstacles";

		public static bool IsDebug;

		public bool RedFirst;

		public TrafficLight.LightObjects[] LightAndObjects;

		private float lastSignalChangeTime;

		private TrafficLight.TrafficLightColor currentTrafficLight;

		private float currentSignalLength;

		private bool redPhase;

		private float cycleTime;

		private readonly IDictionary<TrafficLight.TrafficLightColor, Color> colorsByType = new Dictionary<TrafficLight.TrafficLightColor, Color>
		{
			{
				TrafficLight.TrafficLightColor.Red,
				Color.red
			},
			{
				TrafficLight.TrafficLightColor.Yellow,
				Color.yellow
			},
			{
				TrafficLight.TrafficLightColor.Green,
				Color.green
			}
		};

		[Serializable]
		public class LightObjects
		{
			public TrafficLight.TrafficLightColor TrafficLight;

			public float ActiveTime;

			public List<GameObject> Objects;
		}

		public enum TrafficLightColor
		{
			None = -1,
			Red,
			Yellow,
			Green
		}
	}
}
