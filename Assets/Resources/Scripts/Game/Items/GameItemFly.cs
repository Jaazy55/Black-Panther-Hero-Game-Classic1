using System;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Items
{
	public class GameItemFly : GameItemAbility
	{
		public override void Enable()
		{
			if (this.ExplosionInvulnerInFly)
			{
				PlayerManager.Instance.AnimationController.SetExplosionInvulStatus(true);
			}
			else if (this.CollisionInvulnerInFly)
			{
				PlayerManager.Instance.AnimationController.SetCollisionInvulStatus(true);
			}
			else
			{
				PlayerManager.Instance.AnimationController.UseSuperFly = true;
				this.FlyButton.SetActive(true);
			}
		}

		public override void Disable()
		{
			if (this.ExplosionInvulnerInFly)
			{
				PlayerManager.Instance.AnimationController.SetExplosionInvulStatus(false);
			}
			else if (this.CollisionInvulnerInFly)
			{
				PlayerManager.Instance.AnimationController.SetCollisionInvulStatus(false);
			}
			else
			{
				PlayerManager.Instance.AnimationController.UseSuperFly = false;
				this.FlyButton.SetActive(false);
			}
			PlayerManager.Instance.AnimationController.Reset();
			PlayerManager.Instance.Player.ResetMoveState();
		}

		public GameObject FlyButton;

		public bool ExplosionInvulnerInFly;

		public bool CollisionInvulnerInFly;
	}
}
