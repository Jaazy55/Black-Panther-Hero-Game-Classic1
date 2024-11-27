using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SuicideAchievment : Achievement
{
	public override void Init()
	{
		this.DeathList = this.deathYouNeed;
		this.achiveParams = new Achievement.SaveLoadAchievmentStruct(false, 0, this.DeathList.Count);
	}

	public override void SaveAchiev()
	{
		BaseProfile.StoreValue<Achievement.SaveLoadAchievmentStruct>(this.achiveParams, this.achievementName);
		BaseProfile.StoreArray<SuicideAchievment.DethType>(this.deathYouNeed.ToArray(), this.achievementName + "deathYouNeed");
	}

	public override void LoadAchiev()
	{
		try
		{
			this.achiveParams = BaseProfile.ResolveValue<Achievement.SaveLoadAchievmentStruct>(this.achievementName, this.achiveParams);
		}
		catch (Exception)
		{
			UnityEngine.Debug.Log("Oops achiveParams");
			this.Init();
			this.SaveAchiev();
		}
		try
		{
			this.deathYouNeed = BaseProfile.ResolveArray<SuicideAchievment.DethType>(this.achievementName + "deathYouNeed").ToList<SuicideAchievment.DethType>();
		}
		catch (Exception)
		{
			UnityEngine.Debug.Log("Oops, No dethYouNeed");
		}
	}

	public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
	{
		UnityEngine.Debug.Log("deth = " + i);
		if (this.deathYouNeed.Contains(i))
		{
			this.deathYouNeed.Remove(i);
			this.achiveParams.achiveCounter = this.achiveParams.achiveCounter + 1;
			if (this.deathYouNeed.Count == 0)
			{
				this.AchievComplite();
			}
		}
	}

	private const int ACHIEVTARGERT = 20;

	public List<SuicideAchievment.DethType> DeathList = new List<SuicideAchievment.DethType>();

	public List<SuicideAchievment.DethType> deathYouNeed = new List<SuicideAchievment.DethType>();

	public enum DethType
	{
		None,
		Drowing,
		Falling,
		Explosion,
		CarAccident,
		Shooting
	}
}
