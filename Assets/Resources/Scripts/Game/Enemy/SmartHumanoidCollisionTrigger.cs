using System;
using UnityEngine;

namespace Game.Enemy
{
	public class SmartHumanoidCollisionTrigger : MonoBehaviour
	{
		private void OnTriggerStay(Collider col)
		{
			if (this.HumanoidController != null)
			{
				this.HumanoidController.UpdateSensorInfo(this.SensorType);
			}
		}

		public SmartHumanoidCollisionTrigger.HumanoidSensorType SensorType;

		public SmartHumanoidController HumanoidController;

		public enum HumanoidSensorType
		{
			Front,
			Right,
			Left
		}
	}
}
