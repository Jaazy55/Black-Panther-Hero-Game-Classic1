using System;
using Game.Character;
using Game.Items;
using Game.Shop;
using UnityEngine.UI;

namespace Game.UI
{
	public class PlayerVipLevelDisplay : PlayerInfoDisplayBase
	{
		protected override PlayerInfoType GetInfoType()
		{
			return PlayerInfoType.VipLvL;
		}

		protected override void Display()
		{
			if (PlayerInfoManager.VipLevel > 0)
			{
				GameItem shopItemByType = ShopManager.Instance.GetShopItemByType<GameItemBonus>(ItemsTypes.Bonus, new object[]
				{
					BonusTypes.VIP,
					PlayerInfoManager.VipLevel
				});
				this.ImageLink.sprite = shopItemByType.ShopVariables.ItemIcon;
			}
		}

		public Image ImageLink;
	}
}
