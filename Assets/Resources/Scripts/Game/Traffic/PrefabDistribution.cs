using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Traffic
{
	[Serializable]
	public class PrefabDistribution
	{
		public static PrefabDistribution AverageDistanceDistribution(IDictionary<PrefabDistribution, float> distributionToDistance)
		{
			float num = distributionToDistance.Values.Sum((float distance) => 1f / distance);
			float num2 = 1f / num;
			PrefabDistribution[] array = new PrefabDistribution[distributionToDistance.Count];
			int num3 = 0;
			foreach (KeyValuePair<PrefabDistribution, float> keyValuePair in distributionToDistance)
			{
				PrefabDistribution key = keyValuePair.Key;
				float value = keyValuePair.Value;
				float num4 = 1f / value;
				PrefabDistribution prefabDistribution = new PrefabDistribution();
				foreach (PrefabDistribution.Chance chance in key.Chances)
				{
					PrefabDistribution.Chance chance2 = new PrefabDistribution.Chance();
					chance2.Percent = chance.Percent;
					chance2.Prefab = chance.Prefab;
					prefabDistribution.Chances.Add(chance2);
				}
				array[num3] = prefabDistribution;
				prefabDistribution.Normalize();
				foreach (PrefabDistribution.Chance chance3 in prefabDistribution.Chances)
				{
					chance3.Percent = chance3.Percent * num2 * num4;
					if (chance3.Percent < 0.1f)
					{
						chance3.Percent = 0f;
					}
				}
				num3++;
			}
			return PrefabDistribution.AverageDistribution(array).Normalize();
		}

		public static PrefabDistribution AverageDistribution(params PrefabDistribution[] distributions)
		{
			PrefabDistribution prefabDistribution = new PrefabDistribution();
			foreach (PrefabDistribution prefabDistribution2 in distributions)
			{
				foreach (PrefabDistribution.Chance chance in prefabDistribution2.Chances)
				{
					bool flag = false;
					foreach (PrefabDistribution.Chance chance2 in prefabDistribution.Chances)
					{
						if (chance2.Prefab.Equals(chance.Prefab))
						{
							chance2.Percent += chance.Percent;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						prefabDistribution.Chances.Add(new PrefabDistribution.Chance
						{
							Percent = chance.Percent,
							Prefab = chance.Prefab
						});
					}
				}
			}
			return prefabDistribution.Normalize();
		}

		public GameObject GetRandomPrefab()
		{
			float max = this.Chances.Sum((PrefabDistribution.Chance chance) => chance.Percent);
			float num = UnityEngine.Random.Range(0f, max);
			float num2 = 0f;
			for (int i = 0; i < this.Chances.Count; i++)
			{
				if (num2 + this.Chances[i].Percent > num)
				{
					return this.Chances[i].Prefab;
				}
				num2 += this.Chances[i].Percent;
			}
			return this.Chances[this.Chances.Count - 1].Prefab;
		}

		public PrefabDistribution Normalize()
		{
			float num = this.Chances.Sum((PrefabDistribution.Chance chance) => chance.Percent);
			float num2 = 100f / num;
			foreach (PrefabDistribution.Chance chance2 in this.Chances)
			{
				chance2.Percent *= num2;
				chance2.Percent = Mathf.Round(chance2.Percent * 100f) / 100f;
			}
			return this;
		}

		public string GetStatusForLog()
		{
			string text = string.Empty;
			foreach (PrefabDistribution.Chance chance in this.Chances)
			{
				text += string.Format("{0}% - {1}\n", chance.Percent, chance.Prefab.name);
			}
			return text;
		}

		public List<PrefabDistribution.Chance> Chances = new List<PrefabDistribution.Chance>();

		[Serializable]
		public class Chance
		{
			public GameObject Prefab;

			public float Percent;
		}
	}
}
