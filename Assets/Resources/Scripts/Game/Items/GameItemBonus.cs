using System;
using Game.Character;
using UnityEngine;

namespace Game.Items
{
	public class GameItemBonus : GameItem
	{
		public override bool CanBeBought
		{
			get
			{
				if (this.BonusType == BonusTypes.VIP)
				{
					return PlayerInfoManager.VipLevel < this.BonusValue;
				}
				return this.BonusType != BonusTypes.Money || PlayerInfoManager.Money + this.BonusValue <= PlayerInfoManager.Instance.GetMaxValue(PlayerInfoType.Money);
			}
		}

		public override bool SameParametrWithOther(object[] parametrs)
		{
			if (parametrs.Length == 1)
			{
				return this.BonusType == (BonusTypes)parametrs[0];
			}
			return parametrs.Length == 2 && this.BonusType == (BonusTypes)parametrs[0] && this.BonusValue == (int)parametrs[1];
		}

		[Space(10f)]
		public BonusTypes BonusType;

		[Space(10f)]
		public int BonusValue = 1;
	}
}
