using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class GarageSensor : MonoBehaviour
	{
		public void SpawnNewVehicle(GameObject newVehiclePrefab)
		{
			this.newVehicle = newVehiclePrefab;
			if (this.CheckSpawnAvaible())
			{
				this.GarageRespawner.SetNewObject(this.newVehicle, RespawnedObjectType.None, !this.spawnRequest);
				this.newVehicle = null;
				this.spawnRequest = false;
			}
			else
			{
				this.GarageRespawner.ObjectPrefab = this.newVehicle;
				if (base.isActiveAndEnabled)
				{
					this.spawnRequest = true;
				}
			}
		}

		public bool CheckSpawnAvaible()
		{
			GameObject controlledObject = this.GarageRespawner.GetControlledObject();
			if (controlledObject != null && PlayerManager.Instance.Player.transform.IsChildOf(controlledObject.transform))
			{
				return false;
			}
			if (controlledObject != null && !this.enteredVehicles.Contains(controlledObject.GetComponent<DrivableVehicle>().GetVehicleSource()))
			{
				return false;
			}
			if (this.garageOccupied)
			{
				return false;
			}
			for (int i = 0; i < this.enteredVehicles.Count; i++)
			{
				VehicleSource vehicleSource = this.enteredVehicles[i];
				if (!PlayerManager.Instance.Player.transform.IsChildOf(vehicleSource.RootVehicle.transform))
				{
					if (PoolManager.Instance.ReturnToPool(vehicleSource.RootVehicle))
					{
						this.toRemove.Add(vehicleSource);
					}
				}
			}
			this.enteredVehicles.RemoveAll(this.toRemovePredicate);
			this.toRemove.Clear();
			return this.enteredVehicles.Count == 0;
		}

		private void Awake()
		{
			GarageSensor.Instance = this;
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 1f);
			this.toRemovePredicate = ((VehicleSource source) => this.toRemove.Contains(source));
		}

		private void OnEnable()
		{
			if (this.newVehicle != null)
			{
				this.SpawnNewVehicle(this.newVehicle);
			}
		}

		private void OnDisable()
		{
			this.spawnRequest = false;
		}

		private void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if (this.spawnRequest)
			{
				this.SpawnNewVehicle(this.newVehicle);
			}
			this.garageOccupied = false;
		}

		private void OnTriggerEnter(Collider other)
		{
			VehicleSource component = other.GetComponent<VehicleSource>();
			if (component == null)
			{
				return;
			}
			if (!this.enteredVehicles.Contains(component))
			{
				this.enteredVehicles.Add(component);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			VehicleSource component = other.GetComponent<VehicleSource>();
			if (component == null)
			{
				return;
			}
			if (this.enteredVehicles.Contains(component))
			{
				this.enteredVehicles.Remove(component);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			VehicleSource component = other.GetComponent<VehicleSource>();
			if (component != null)
			{
				return;
			}
			this.garageOccupied = true;
		}

		public static GarageSensor Instance;

		public ControlableObjectRespawner GarageRespawner;

		private readonly List<VehicleSource> enteredVehicles = new List<VehicleSource>();

		private readonly List<VehicleSource> toRemove = new List<VehicleSource>();

		private Predicate<VehicleSource> toRemovePredicate;

		private SlowUpdateProc slowUpdateProc;

		private bool garageOccupied;

		private bool spawnRequest;

		private GameObject newVehicle;
	}
}
