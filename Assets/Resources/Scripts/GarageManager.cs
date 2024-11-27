using System;
using Game.GlobalComponent;
using Game.Items;
using Game.Shop;
using Game.Vehicle;
using UnityEngine;

public class GarageManager : MonoBehaviour
{
	public static GarageManager Instance
	{
		get
		{
			return (!(GarageManager.instance != null)) ? (GarageManager.instance = UnityEngine.Object.FindObjectOfType<GarageManager>()) : GarageManager.instance;
		}
	}

	private void Awake()
	{
		GarageManager.instance = this;
	}

	private void Start()
	{
		this.ResetVehicle();
	}

	public void SetVehicle(GameItemVehicle vehicle)
	{
		if (vehicle.VehiclePrefab == this.MainRespawner.ObjectPrefab)
		{
			return;
		}
		BaseProfile.StoreValue<int>(vehicle.ID, "MainVehicle");
		this.GarageSensor.SpawnNewVehicle(vehicle.VehiclePrefab);
	}

	private void ResetVehicle()
	{
		int num = BaseProfile.ResolveValue<int>("MainVehicle", 0);
		if (num != 0 && ShopManager.Instance.BoughtAlredy(num))
		{
			GameItemVehicle gameItemVehicle = ItemsManager.Instance.GetItem(num) as GameItemVehicle;
			if (gameItemVehicle != null)
			{
				this.MainRespawner.SetNewObject(gameItemVehicle.VehiclePrefab, RespawnedObjectType.None, true);
			}
		}
	}

	private const string VehicleKey = "MainVehicle";

	private static GarageManager instance;

	public ControlableObjectRespawner MainRespawner;

	public GarageSensor GarageSensor;
}
