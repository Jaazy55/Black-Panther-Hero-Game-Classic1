using System;
using UnityEngine;

namespace Roulette
{
	[Serializable]
	public class LuckPrize
	{
		public Prize Prize
		{
			get
			{
				return this.m_Prize;
			}
		}

		public float LuckDistribution
		{
			get
			{
				return this.m_LuckDistribution;
			}
		}

		[SerializeField]
		private Prize m_Prize;

		[SerializeField]
		private float m_LuckDistribution;
	}
}
