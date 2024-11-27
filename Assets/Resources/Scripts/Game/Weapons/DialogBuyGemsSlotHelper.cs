using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Weapons
{
	public class DialogBuyGemsSlotHelper : DialogSlotHelper
	{
		[Space(10f)]
		public Text ValueText;

		public Text PriceText;

		public bool ForFree;
	}
}
