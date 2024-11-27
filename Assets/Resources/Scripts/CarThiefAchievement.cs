using System;
using System.Collections.Generic;
using System.Linq;
using Game.Managers;
using Game.Vehicle;
using UnityEngine;

public class CarThiefAchievement : Achievement
{
	public override void Init()
	{
		this.vehicleList = this.carsYouNeed;
		this.achiveParams = new Achievement.SaveLoadAchievmentStruct(false, 0, this.vehicleList.Count);
	}

	public override void SaveAchiev()
	{
		BaseProfile.StoreValue<Achievement.SaveLoadAchievmentStruct>(this.achiveParams, this.achievementName);
		BaseProfile.StoreArray<VehicleList>(this.carsYouNeed.ToArray(), this.achievementName + "CarsList");
	}

	public override void LoadAchiev()
	{
		try
		{
			this.achiveParams = BaseProfile.ResolveValue<Achievement.SaveLoadAchievmentStruct>(this.achievementName, this.achiveParams);
		}
		catch (Exception)
		{
			if (GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("Oops achiveParams");
			}
			this.Init();
			this.SaveAchiev();
		}
		if (!this.achiveParams.isDone)
		{
			VehicleList[] array = BaseProfile.ResolveArray<VehicleList>(this.achievementName + "CarsList");
			if (array != null)
			{
				this.carsYouNeed = array.ToList<VehicleList>();
			}
		}
	}

	public override void GetIntoVehicleEvent(DrivableVehicle vehicle)
	{
		if (this.carsYouNeed.Contains(vehicle.VehicleSpecificPrefab.Name))
		{
			if (GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("Dude, You stole my " + vehicle.VehicleSpecificPrefab.Name.ToString());
			}
			this.carsYouNeed.Remove(vehicle.VehicleSpecificPrefab.Name);
			this.achiveParams.achiveCounter = this.achiveParams.achiveCounter + 1;
			if (this.carsYouNeed.Count == 0)
			{
				this.AchievComplite();
			}
		}
	}

	public List<VehicleList> vehicleList = new List<VehicleList>();

	public List<VehicleList> carsYouNeed = new List<VehicleList>();
}
