using System;

namespace Roulette
{
	[Serializable]
	public struct PrizeProbability
	{
		public PrizeProbability(Prize prize, float probability)
		{
			this.Prize = prize;
			this.Probability = probability;
		}

		public Prize Prize;

		public float Probability;
	}
}
