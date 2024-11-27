using System;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Items
{
	public class GameItemAbility : GameItem
	{
		public override bool CanBeEquiped
		{
			get
			{
				return PlayerManager.Instance.PlayerIsDefault;
			}
		}

		public virtual void Enable()
		{
		}

		public virtual void Disable()
		{
		}

		public override bool SameParametrWithOther(object[] parametrs)
		{
			return true;
		}

		public GameItemSkin[] RelatedSkins;

		[HideInInspector]
		public bool IsActive;
	}
}
