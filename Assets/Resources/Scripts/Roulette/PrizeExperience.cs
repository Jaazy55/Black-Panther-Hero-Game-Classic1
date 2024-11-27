using System;
using Game.Character;
using UnityEngine;

namespace Roulette
{
	[CreateAssetMenu(fileName = "Prize Experience", menuName = "Roulette/Prize Experience")]
	public class PrizeExperience : Prize
	{
		public override void WillBeGiven()
		{
			if (this.m_Value > 0)
			{
				PlayerInfoManager.Experience += this.m_Value;
			}
		}

		public int m_Value;
	}
}
