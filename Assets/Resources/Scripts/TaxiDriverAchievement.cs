using System;
using Game.Vehicle;
using UnityEngine;

public class TaxiDriverAchievement : Achievement
{
	public override void Init()
	{
		this.timeLeft = 600f;
		this.achiveParams = new Achievement.SaveLoadAchievmentStruct(false, 600 - (int)this.timeLeft, 600);
	}

	public override void SaveAchiev()
	{
		this.achiveParams.achiveCounter = 600 - (int)this.timeLeft;
		BaseProfile.StoreValue<Achievement.SaveLoadAchievmentStruct>(this.achiveParams, this.achievementName);
	}

	public override void LoadAchiev()
	{
		try
		{
			this.achiveParams = BaseProfile.ResolveValue<Achievement.SaveLoadAchievmentStruct>(this.achievementName, this.achiveParams);
			this.timeLeft = (float)(600 - this.achiveParams.achiveCounter);
		}
		catch (Exception)
		{
			UnityEngine.Debug.Log("Oops achiveParams");
			this.Init();
			this.SaveAchiev();
		}
	}

	public override void GetIntoVehicleEvent(DrivableVehicle vehicle)
	{
		if (vehicle.VehicleSpecificPrefab.Name == VehicleList.Taxi)
		{
			this.isTimerOn = true;
		}
	}

	public override void GetOutVehicleEvent(DrivableVehicle vehicle)
	{
		if (this.isTimerOn = true)
		{
			this.isTimerOn = false;
		}
	}

	public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
	{
		if (this.isTimerOn = true)
		{
			this.isTimerOn = false;
		}
	}

	private void LateUpdate()
	{
		if (this.isTimerOn)
		{
			this.timeLeft -= Time.deltaTime;
			if (this.timeLeft <= 0f)
			{
				this.isTimerOn = false;
				this.AchievComplite();
			}
		}
	}

	private void OnDestroy()
	{
		if (this.isTimerOn = true)
		{
			this.isTimerOn = false;
		}
	}

	private const int StartTime = 600;

	private float timeLeft;

	private bool isTimerOn;
}
