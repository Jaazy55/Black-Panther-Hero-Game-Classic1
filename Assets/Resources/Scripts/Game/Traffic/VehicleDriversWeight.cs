using System;
using Game.Managers;
using Game.Vehicle;
using UnityEngine;

namespace Game.Traffic
{
	public class VehicleDriversWeight : MonoBehaviour
	{
		public DummyDriver GetVehicleDriver(DrivableVehicle vehicle)
		{
			DummyDriver dummyDriver = null;
			foreach (VehicleDriversWeight.VehicleDistribution vehicleDistribution in this.VehicleDistributions)
			{
				if (vehicleDistribution.Vehicle == vehicle)
				{
					dummyDriver = vehicleDistribution.Distribution.GetRandomPrefab().GetComponent<DummyDriver>();
					break;
				}
			}
			if (dummyDriver == null)
			{
				if (GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log("Driver for '" + vehicle.name + "' not find");
				}
				dummyDriver = this.DefaultDriver;
			}
			return dummyDriver;
		}

		public DummyDriver DefaultDriver;

		public VehicleDriversWeight.VehicleDistribution[] VehicleDistributions;

		[Serializable]
		public class VehicleDistribution
		{
			public DrivableVehicle Vehicle;

			public PrefabDistribution Distribution;
		}
	}
}
