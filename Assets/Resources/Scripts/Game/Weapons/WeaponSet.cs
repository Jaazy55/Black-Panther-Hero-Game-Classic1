using System;

namespace Game.Weapons
{
	[Serializable]
	public class WeaponSet
	{
		public bool SlotIsEmpty(int slotIndex)
		{
			return this.WeaponInSlot(slotIndex) == null;
		}

		public Weapon WeaponInSlot(int slotIndex)
		{
			if (slotIndex < 0 || slotIndex >= this.Slots.Length)
			{
				return null;
			}
			return this.Slots[slotIndex].WeaponPrefab;
		}

		public int FirstSlotOfType(WeaponSlotTypes wantedType)
		{
			for (int i = 0; i < this.Slots.Length; i++)
			{
				WeaponSlot weaponSlot = this.Slots[i];
				if (weaponSlot.WeaponSlotType == wantedType)
				{
					return i;
				}
			}
			return -1;
		}

		public WeaponSlot GetFirstSlotOfType(WeaponSlotTypes wantedType)
		{
			for (int i = 0; i < this.Slots.Length; i++)
			{
				WeaponSlot weaponSlot = this.Slots[i];
				if (weaponSlot.WeaponSlotType == wantedType)
				{
					return weaponSlot;
				}
			}
			return null;
		}

		public WeaponSlot GetEmptySlotOfType(WeaponSlotTypes wantedType)
		{
			foreach (WeaponSlot weaponSlot in this.Slots)
			{
				if (weaponSlot.WeaponSlotType == wantedType && weaponSlot.WeaponPrefab == null)
				{
					return weaponSlot;
				}
			}
			return null;
		}

		public int GetEmptySlotOfTypeInt(WeaponSlotTypes wantedType)
		{
			for (int i = 0; i < this.Slots.Length; i++)
			{
				WeaponSlot weaponSlot = this.Slots[i];
				if ((weaponSlot.WeaponSlotType == wantedType || weaponSlot.WeaponSlotType == WeaponSlotTypes.Universal) && weaponSlot.Available && weaponSlot.WeaponPrefab == null)
				{
					return i;
				}
			}
			return -1;
		}

		public WeaponSlot[] Slots;

		public bool Locked;
	}
}
