using System;
using System.Collections.Generic;
using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;

namespace Game.PickUps
{
	public class HealthPickup : PickUp
	{
		protected override void TakePickUp()
		{
			Player player = PlayerInteractionsManager.Instance.Player;
			if (player.Health.Current >= player.Health.Max)
			{
				return;
			}
			float num = HealthPickup.healthPacksValue[this.HealthPackType];
			int num2 = (int)(player.Health.Max * num);
			int num3 = (int)(num * 100f);
			InGameLogManager.Instance.RegisterNewMessage(MessageType.HealthPack, num3.ToString());
			PlayerInteractionsManager.Instance.Player.AddHealth((float)num2);
			base.TakePickUp();
		}

		private static IDictionary<PickUpManager.HealthPackType, float> healthPacksValue = new Dictionary<PickUpManager.HealthPackType, float>
		{
			{
				PickUpManager.HealthPackType.Small,
				0.2f
			},
			{
				PickUpManager.HealthPackType.Medium,
				0.4f
			},
			{
				PickUpManager.HealthPackType.Large,
				0.8f
			}
		};

		public PickUpManager.HealthPackType HealthPackType;
	}
}
