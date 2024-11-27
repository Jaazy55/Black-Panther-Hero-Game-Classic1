using System;
using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Items
{
	public class GameItemSuperKick : GameItemAbility
	{
		public override void Enable()
		{
			PlayerManager.Instance.Player.GetComponentInChildren<SuperKick>().enabled = true;
			this.KickButton.SetActive(true);
		}

		public override void Disable()
		{
			PlayerManager.Instance.Player.GetComponentInChildren<SuperKick>().enabled = false;
			this.KickButton.SetActive(false);
		}

		public GameObject KickButton;
	}
}
