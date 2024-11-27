using System;
using Game.Weapons;
using UnityEngine;

namespace Game.Items
{
	public class GameItemAmmo : GameItem
	{
		public override void Init()
		{
			base.Init();
			AmmoManager.Instance.CreateContainer(this);
		}

		public override bool SameParametrWithOther(object[] parametrs)
		{
			return this.AmmoType == (AmmoTypes)parametrs[0];
		}

		public AmmoTypes AmmoType;

		public GameObject ammoPrefab;
	}
}
