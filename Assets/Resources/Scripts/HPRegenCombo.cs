using System;
using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

public class HPRegenCombo : ComboManager.BaseComboEffect
{
	private new void Start()
	{
		this.player = PlayerInteractionsManager.Instance.Player;
	}

	public override void StartEffect(ComboManager.ComboInfo comboInfo)
	{
		float num = this.PerKillHPRegen * (float)comboInfo.ComboMultiplier;
		if (num > this.MaxAdditionalHPRegen)
		{
			num = this.MaxAdditionalHPRegen;
		}
		this.player.Health.Current += num * this.player.Health.Max;
		if (this.DebugLog)
		{
			UnityEngine.Debug.Log(num * this.player.Health.Max);
		}
	}

	public override void StopEffect()
	{
	}

	[Space(10f)]
	public float PerKillHPRegen = 0.05f;

	public float MaxAdditionalHPRegen = 0.5f;

	private Player player;
}
