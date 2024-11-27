using System;
using Game.Character.CharacterController;

namespace Game.Items
{
	public class GameItemWallClimb : GameItemAbility
	{
		public override void Enable()
		{
			PlayerManager.Instance.AnimationController.EnableClimbWalls = true;
		}

		public override void Disable()
		{
			PlayerManager.Instance.AnimationController.EnableClimbWalls = false;
			PlayerManager.Instance.AnimationController.Reset();
		}
	}
}
