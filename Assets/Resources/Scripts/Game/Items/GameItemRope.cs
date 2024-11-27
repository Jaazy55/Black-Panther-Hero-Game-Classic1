using System;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Items
{
	public class GameItemRope : GameItemAbility
	{
		public override void Enable()
		{
			if (this.UseInFly)
			{
				PlayerManager.Instance.AnimationController.UseRopeWhileFlying = true;
			}
			else
			{
				PlayerManager.Instance.AnimationController.UseRope = true;
				this.ShotRopeButton.SetActive(true);
			}
		}

		public override void Disable()
		{
			if (this.UseInFly)
			{
				PlayerManager.Instance.AnimationController.UseRopeWhileFlying = false;
			}
			else
			{
				PlayerManager.Instance.AnimationController.UseRope = false;
				this.ShotRopeButton.SetActive(false);
			}
		}

		public GameObject ShotRopeButton;

		public bool UseInFly;
	}
}
