using System;
using UnityEngine;

namespace Game.Items
{
	public class GameItemAccessory : GameItemSkin
	{
		public override bool SameParametrWithOther(object[] parametrs)
		{
			return this.ModelPrefab == (GameObject)parametrs[0];
		}

		[Space(10f)]
		public GameObject ModelPrefab;
	}
}
