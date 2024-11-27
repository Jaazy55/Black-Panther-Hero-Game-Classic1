using System;
using UnityEngine;

namespace Game.Vehicle
{
	public class CollisionTrigger : MonoBehaviour
	{
		private void Awake()
		{
			if (this.autopilot == null)
			{
				this.autopilot = base.GetComponentInParent<Autopilot>();
			}
		}

		private void Update()
		{
			if (this.autopilot == null)
			{
				this.autopilot = base.GetComponentInParent<Autopilot>();
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (this.autopilot != null)
			{
				this.autopilot.OnSensorStay(other, this.SensorType);
			}
		}

		public CollisionTrigger.Sensor SensorType;

		private Autopilot autopilot;

		public enum Sensor
		{
			Front,
			Right,
			Left,
			RightBlocking,
			LeftBlocking
		}
	}
}
