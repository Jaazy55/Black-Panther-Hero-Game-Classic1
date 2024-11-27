using System;
using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Vehicle
{
	public class BikeTrigger : MonoBehaviour
	{
		private void Awake()
		{
			if (BikeTrigger.collisionSensorLayerNumber == -1)
			{
				BikeTrigger.collisionSensorLayerNumber = LayerMask.NameToLayer("CollisionSensor");
			}
		}

		public void Init()
		{
			this.controller = base.transform.parent.GetComponent<DrivableVehicle>().controller;
		}

		private void OnTriggerEnter(Collider col)
		{
			if (col.GetComponentInParent<Player>())
			{
				return;
			}
			if (col.gameObject.layer != BikeTrigger.collisionSensorLayerNumber && PlayerInteractionsManager.Instance.inVehicle && this.controller && this.controller.isActiveAndEnabled)
			{
				this.controller.DropFrom();
			}
		}

		private static int collisionSensorLayerNumber = -1;

		private VehicleController controller;
	}
}
