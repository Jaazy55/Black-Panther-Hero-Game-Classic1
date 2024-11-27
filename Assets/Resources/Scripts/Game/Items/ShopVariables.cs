using System;
using UnityEngine;

namespace Game.Items
{
	[Serializable]
	public class ShopVariables
	{
		public bool isDivision;

		public bool HideInShop;

		[Space(10f)]
		public int price;

		public bool gemPrice;

		[Space(10f)]
		[Tooltip("Все что не идет в лодаут должно \"екипироваться\" сразу при покупке.")]
		public bool InstantEquip;

		[Space(10f)]
		public string Name;

		public Sprite ItemIcon;

		[Tooltip("0 для бесконечного стака")]
		public int MaxAmount = 1;

		public int PerStackAmount = 1;

		[Multiline]
		public string Description;

		[Space(10f)]
		public int playerLvl = 1;

		public int VipLvl;

		public GameItemHider Hider;
	}
}
