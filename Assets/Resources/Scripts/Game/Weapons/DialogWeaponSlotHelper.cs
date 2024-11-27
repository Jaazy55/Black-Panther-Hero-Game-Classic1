using System;
using Game.Character.CharacterController;

namespace Game.Weapons
{
	public class DialogWeaponSlotHelper : DialogSlotHelper
	{
		public WeaponSlot RelatedSlot
		{
			get
			{
				WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
				return defaultWeaponController.WeaponSet.Slots[this.SlotIndex];
			}
		}
	}
}
