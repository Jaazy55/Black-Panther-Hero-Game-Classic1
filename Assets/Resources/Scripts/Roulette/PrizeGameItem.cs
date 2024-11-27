using System;
using Game.Items;
using Game.Shop;
using UnityEngine;

namespace Roulette
{
	[CreateAssetMenu(fileName = "Prize GameItem", menuName = "Roulette/Prize GameItem")]
	public class PrizeGameItem : Prize
	{
		public override bool CanBeGiven
		{
			get
			{
				if (!ShopManager.HasInstance && ItemsManager.HasInstance)
				{
					return false;
				}
				GameItem item = ItemsManager.Instance.GetItem(this.GameItemID);
				return item != null && ShopManager.Instance.ItemAvailableForBuy(item);
			}
		}

		public override void WillBeGiven()
		{
			if (!ShopManager.HasInstance && ItemsManager.HasInstance)
			{
				return;
			}
			GameItem item = ItemsManager.Instance.GetItem(this.GameItemID);
			if (item != null)
			{
				if (item.ShopVariables.Hider != null && item.ShopVariables.Hider is SimpleGameItemHider)
				{
					SimpleGameItemHider simpleGameItemHider = item.ShopVariables.Hider as SimpleGameItemHider;
					simpleGameItemHider.SetHide(false);
				}
				ShopManager.Instance.Give(item, true);
			}
		}

		[SerializeField]
		private int GameItemID;
	}
}
