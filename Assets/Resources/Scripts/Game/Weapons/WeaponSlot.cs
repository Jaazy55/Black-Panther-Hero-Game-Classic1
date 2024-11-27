using System;
using Game.Items;
using Game.Shop;
using UnityEngine;

namespace Game.Weapons
{
	[Serializable]
	public class WeaponSlot
	{
		public bool Available
		{
			get
			{
				return this.BuyToUnlock == null || ShopManager.Instance.BoughtAlredy(this.BuyToUnlock);
			}
		}

		public WeaponSlotTypes WeaponSlotType;

		public Weapon WeaponPrefab;

		public Weapon WeaponInstance;

		public GameObject Placeholder;

		public GameItem BuyToUnlock;
	}
}
