using System;
using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.GlobalComponent.HelpfulAds
{
	public class SecondBreathAds : HelpfulAds
	{
		public override HelpfullAdsType HelpType()
		{
			return HelpfullAdsType.Stamina;
		}

		public override void HelpAccepted()
		{
			Player player = PlayerInteractionsManager.Instance.Player;
			float amount = player.stats.stamina.Max * this.RecoveryProcent;
			player.stats.stamina.Change(amount);
		}

		[Range(0f, 1f)]
		public float RecoveryProcent = 0.5f;
	}
}
