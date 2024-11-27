using System;
using Game.Character.CharacterController;

namespace Game.Items
{
	public class GameItemSuperLanding : GameItemAbility
	{
		public override void Enable()
		{
			if (this.SuperLandingExplosion)
			{
				PlayerManager.Instance.Player.UseSuperLandingExplosion = true;
			}
			else
			{
				PlayerManager.Instance.AnimationController.UseSuperheroLandings = true;
				PlayerManager.Instance.Player.UpdateOnFallImpact();
			}
		}

		public override void Disable()
		{
			if (this.SuperLandingExplosion)
			{
				PlayerManager.Instance.Player.UseSuperLandingExplosion = false;
			}
			else
			{
				PlayerManager.Instance.AnimationController.UseSuperheroLandings = false;
				PlayerManager.Instance.Player.UpdateOnFallImpact();
			}
		}

		public bool SuperLandingExplosion;
	}
}
