using System;
using Game.Character;
using Game.Shop;

namespace Game.GlobalComponent.HelpfulAds
{
	public class MoneyAds : HelpfulAds
	{
		public override HelpfullAdsType HelpType()
		{
			return this.AdsType;
		}

		public override void HelpAccepted()
		{
			if (this.ByGems)
			{
				PlayerInfoManager.Gems += this.AddedMoney;
			}
			else
			{
				PlayerInfoManager.Money += this.AddedMoney;
				InGameLogManager.Instance.RegisterNewMessage(MessageType.Money, this.AddedMoney.ToString());
			}
			if (ShopManager.Instance != null)
			{
				ShopManager.Instance.UpdateInfo();
			}
		}

		public bool ByGems;

		public int AddedMoney = 300;

		public HelpfullAdsType AdsType = HelpfullAdsType.Money;
	}
}
