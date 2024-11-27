using System;
using Game.Character;
using Game.Character.CharacterController;

namespace Game.GlobalComponent.HelpfulAds
{
	public class AmmoAds : HelpfulAds
	{
		public override HelpfullAdsType HelpType()
		{
			return HelpfullAdsType.Ammo;
		}

		public override void HelpAccepted()
		{
			Player player = PlayerInteractionsManager.Instance.Player;
			player.AddAmmoForCurrentWeapon();
		}
	}
}
