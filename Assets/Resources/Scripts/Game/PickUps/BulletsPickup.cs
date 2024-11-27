using System;
using Game.GlobalComponent;
using Game.Weapons;

namespace Game.PickUps
{
	public class BulletsPickup : PickUp
	{
		protected override void TakePickUp()
		{
			AmmoManager.Instance.AddAmmo(this.BulletType);
			InGameLogManager.Instance.RegisterNewMessage(MessageType.Bullets, this.BulletType.ToString());
			base.TakePickUp();
		}

		public AmmoTypes BulletType;
	}
}
