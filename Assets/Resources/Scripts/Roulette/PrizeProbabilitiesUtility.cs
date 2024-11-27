using System;
using System.Collections.Generic;
using UnityEngine;

namespace Roulette
{
	public static class PrizeProbabilitiesUtility
	{
		public static int IndexOf(this IList<PrizeProbability> probabilities, Prize prize)
		{
			for (int i = 0; i < probabilities.Count; i++)
			{
				if (probabilities[i].Prize == prize)
				{
					return i;
				}
			}
			return -1;
		}

		public static void Normalize(this List<PrizeProbability> probabilities)
		{
			PrizeProbability[] collection = probabilities.Normalized();
			probabilities.Clear();
			probabilities.AddRange(collection);
		}

		public static PrizeProbability[] Normalized(this IList<PrizeProbability> probabilities)
		{
			Dictionary<Prize, float> dictionary = new Dictionary<Prize, float>();
			float num = 0f;
			foreach (PrizeProbability prizeProbability in probabilities)
			{
				if (!(prizeProbability.Prize == null))
				{
					if (!dictionary.ContainsKey(prizeProbability.Prize))
					{
						dictionary.Add(prizeProbability.Prize, prizeProbability.Probability);
					}
					else
					{
						Dictionary<Prize, float> dictionary2= dictionary;
						Prize prize= prizeProbability.Prize;
						(dictionary2 )[prize ] = dictionary2[prize] + prizeProbability.Probability;
					}
					num += prizeProbability.Probability;
				}
			}
			Dictionary<Prize, float>.KeyCollection keys = dictionary.Keys;
			foreach (Prize key in keys)
			{
				if (dictionary[key] < 0f)
				{
					num -= dictionary[key];
					dictionary.Remove(key);
				}
			}
			PrizeProbability[] array = new PrizeProbability[dictionary.Count];
			UnityEngine.Debug.Log("Sum = " + num);
			int num2 = -1;
			if (num != 0f)
			{
				foreach (KeyValuePair<Prize, float> keyValuePair in dictionary)
				{
					num2++;
					array[num2] = new PrizeProbability(keyValuePair.Key, keyValuePair.Value / num);
				}
			}
			else
			{
				float probability = num / (float)dictionary.Count;
				foreach (KeyValuePair<Prize, float> keyValuePair2 in dictionary)
				{
					num2++;
					array[num2] = new PrizeProbability(keyValuePair2.Key, probability);
				}
			}
			return array;
		}

		public static Prize GetRandomPrize(this IList<PrizeProbability> probabilities)
		{
			Prize prize = null;
			do
			{
				float num = UnityEngine.Random.Range(0f, 1f);
				for (int i = 0; i < probabilities.Count; i++)
				{
					num -= probabilities[i].Probability;
					if (num <= 0f)
					{
						prize = probabilities[i].Prize;
						break;
					}
				}
			}
			while (prize == null || !prize.CanBeGiven);
			return prize;
		}
	}
}
