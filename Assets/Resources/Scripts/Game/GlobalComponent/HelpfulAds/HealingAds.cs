using System;
using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.GlobalComponent.HelpfulAds
{
	public class HealingAds : HelpfulAds
	{
		public override HelpfullAdsType HelpType()
		{
			return HelpfullAdsType.Heal;
		}

		public override void HelpAccepted()
		{
			Player player = PlayerInteractionsManager.Instance.Player;
			if (player.Health.Current < 0f)
			{
				player.Health.Current = 1f;
			}
			float amount = player.Health.Max * this.HealProcent;
			player.AddHealth(amount);
			InGameLogManager.Instance.RegisterNewMessage(MessageType.HealthPack, (int)(this.HealProcent * 100f) + "% ");
		}

		[Range(0f, 1f)]
		public float HealProcent = 0.25f;
	}
}
