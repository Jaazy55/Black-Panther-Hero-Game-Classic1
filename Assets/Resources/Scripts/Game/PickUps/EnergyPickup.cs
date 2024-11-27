using System;
using Game.Character;
using Game.GlobalComponent;

namespace Game.PickUps
{
	public class EnergyPickup : PickUp
	{
		protected override void TakePickUp()
		{
			InGameLogManager.Instance.RegisterNewMessage(MessageType.Enegry, this.enegryCountInPickUp.ToString());
			PlayerInteractionsManager.Instance.Player.stats.stamina.SetAmount(this.enegryCountInPickUp);
			base.TakePickUp();
		}

		public float enegryCountInPickUp = 35f;
	}
}
