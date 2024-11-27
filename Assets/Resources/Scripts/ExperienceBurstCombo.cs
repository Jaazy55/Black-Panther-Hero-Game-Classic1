using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.Managers;
using UnityEngine;

public class ExperienceBurstCombo : ComboManager.BaseComboEffect
{
	private new void Start()
	{
		this.player = PlayerInteractionsManager.Instance.Player;
	}

	public override void StartEffect(ComboManager.ComboInfo comboInfo)
	{
		float num = this.PerKillExpMult * (float)comboInfo.ComboMultiplier;
		if (num > this.MaxAdditionalExpMult)
		{
			num = this.MaxAdditionalExpMult;
		}
		LevelManager.Instance.AddExperience((int)(num * comboInfo.LastVictim.ExperienceForAKill), false);
		if (this.DebugLog && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log(num * comboInfo.LastVictim.ExperienceForAKill);
		}
	}

	public override void StopEffect()
	{
	}

	[Space(10f)]
	public float PerKillExpMult = 0.05f;

	public float MaxAdditionalExpMult = 1f;

	private Player player;
}
