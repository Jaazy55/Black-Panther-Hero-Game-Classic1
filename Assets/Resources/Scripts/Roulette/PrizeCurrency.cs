using System;
using Game.Character;
using UnityEngine;

namespace Roulette
{
	[CreateAssetMenu(fileName = "Prize Currency", menuName = "Roulette/Prize Currency")]
	public class PrizeCurrency : Prize
	{
		public int Money
		{
			get
			{
				return this.m_Money;
			}
		}

		public int Gems
		{
			get
			{
				return this.m_Gems;
			}
		}

		public bool HasMoney
		{
			get
			{
				return this.m_Money > 0;
			}
		}

		public bool HasGems
		{
			get
			{
				return this.m_Gems > 0;
			}
		}

		public override void WillBeGiven()
		{
			if (this.m_Money > 0)
			{
				PlayerInfoManager.Money += this.m_Money;
			}
			if (this.m_Gems > 0)
			{
				PlayerInfoManager.Gems += this.m_Gems;
			}
		}

		[SerializeField]
		private int m_Money;

		[SerializeField]
		private int m_Gems;
	}
}
