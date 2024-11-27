using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.PickUps
{
	public class CollectionPickup : PickUp
	{
		protected override void TakePickUp()
		{
			CollectionPickUpsManager.Instance.ElementWasTaken(base.gameObject);
			int amount = 300;
			InGameLogManager.Instance.RegisterNewMessage(MessageType.Money, amount.ToString());
			PlayerInfoManager.Instance.ChangeInfoValue(PlayerInfoType.Money, amount, true);
			if (this.collectionPickupSound)
			{
				PointSoundManager.Instance.PlaySoundAtPoint(PlayerManager.Instance.Player.transform.position, TypeOfSound.GetCollectionItem);
			}
			base.TakePickUp();
		}

		public CollectionPickUpsManager.CollectionTypes CollectionType;

		public AudioClip collectionPickupSound;
	}
}
