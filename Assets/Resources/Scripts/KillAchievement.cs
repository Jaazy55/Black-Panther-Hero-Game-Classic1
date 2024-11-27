using System;
using Game.Character.CharacterController;
using Game.Vehicle;
using UnityEngine;

public class KillAchievement : Achievement
{
	public override void Init()
	{
		this.achiveParams = new Achievement.SaveLoadAchievmentStruct(false, 0, 50);
	}

	public override void NpcKilledEvent(Vector3 position, Faction npcFaction, HitEntity victim, HitEntity killer)
	{
		if (victim is VehicleStatus || !(killer is Player))
		{
			return;
		}
		this.achiveParams.achiveCounter = this.achiveParams.achiveCounter + 1;
		if (this.achiveParams.achiveCounter >= this.achiveParams.achiveTarget)
		{
			this.AchievComplite();
		}
	}

	private const int ACHIEVTARGERT = 50;
}
