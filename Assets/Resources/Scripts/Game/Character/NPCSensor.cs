using System;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.Managers;
using Game.Vehicle;
using UnityEngine;

namespace Game.Character
{
	public class NPCSensor : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			if (this.SmartNpcController == null || !this.SmartNpcController.IsInited)
			{
				return;
			}
			HitEntity hitEntity = other.GetComponentInParent<HitEntity>();
			DrivableVehicle componentInParent = other.GetComponentInParent<DrivableVehicle>();
			if (hitEntity != null)
			{
				this.SmartNpcController.AddTarget(hitEntity, false);
			}
			if (componentInParent != null && componentInParent.CurrentDriver != null)
			{
				if (componentInParent.DriverIsVulnerable)
				{
					this.SmartNpcController.AddTarget(componentInParent.CurrentDriver, true);
				}
				else
				{
					this.SmartNpcController.AddTarget(componentInParent.GetVehicleStatus(), false);
				}
			}
			if (this.SmartNpcController.VehiclesAsTargets)
			{
				if (componentInParent == null)
				{
					return;
				}
				hitEntity = componentInParent.GetVehicleStatus();
				if (hitEntity == null && GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log("Какая-то херь");
				}
				this.SmartNpcController.AddTarget(hitEntity, true);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (this.SmartNpcController == null || !this.SmartNpcController.IsInited)
			{
				return;
			}
			HitEntity componentInParent = other.GetComponentInParent<HitEntity>();
			if (componentInParent != null)
			{
				this.SmartNpcController.RemoveTarget(componentInParent, true);
				if (this.DebugLog && GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log(componentInParent.name);
				}
			}
		}

		public bool DebugLog;

		public SmartHumanoidController SmartNpcController;
	}
}
