using System;
using UnityEngine;

namespace Roulette
{
	[CreateAssetMenu(fileName = "Distribution of Prize", menuName = "Roulette/Distribution of Prize")]
	public class PrizeDistribution : ScriptableObject
	{
		public PrizeProbability[] Probabilities
		{
			get
			{
				return this.m_Distributions.Clone() as PrizeProbability[];
			}
		}

		public int Count
		{
			get
			{
				return this.m_Distributions.Length;
			}
		}

		public PrizeProbability this[int key]
		{
			get
			{
				return this.m_Distributions[key];
			}
		}

		public Prize GetRandomPrize()
		{
			return this.m_Distributions.GetRandomPrize();
		}

		[ContextMenu("Normalize")]
		private void Normalize()
		{
			this.m_Distributions = this.m_Distributions.Normalized();
		}

		[SerializeField]
		private PrizeProbability[] m_Distributions;
	}
}
