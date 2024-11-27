using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	[Serializable]
	public class AttackModifierCollection
	{
		protected List<AttackModifierProbability> ModifierProbabilities
		{
			get
			{
				if (this.m_modifierProbabilities == null)
				{
					this.m_modifierProbabilities = new List<AttackModifierProbability>();
				}
				return this.m_modifierProbabilities;
			}
		}

		protected Dictionary<AttackModifier, float> ProbabilitiesDictionary
		{
			get
			{
				if (this.m_probabilitiesDictionary == null)
				{
					this.m_probabilitiesDictionary = new Dictionary<AttackModifier, float>();
					foreach (AttackModifierProbability attackModifierProbability in this.ModifierProbabilities)
					{
						this.m_probabilitiesDictionary.Add(attackModifierProbability.Modifier, attackModifierProbability.Probability);
					}
				}
				return this.m_probabilitiesDictionary;
			}
		}

		public bool HasModifier(AttackModifier attackModifier)
		{
			return this.ProbabilitiesDictionary.ContainsKey(attackModifier);
		}

		public float GetPropobility(AttackModifier attackModifier)
		{
			if (this.ProbabilitiesDictionary.ContainsKey(attackModifier))
			{
				return this.ProbabilitiesDictionary[attackModifier];
			}
			return 0f;
		}

		public void SetPropobility(AttackModifier attackModifier, float propobility)
		{
			if (!this.ProbabilitiesDictionary.ContainsKey(attackModifier))
			{
				this.ProbabilitiesDictionary.Add(attackModifier, propobility);
			}
			else
			{
				this.ProbabilitiesDictionary[attackModifier] = Mathf.Clamp(propobility, 0f, 100f);
			}
		}

		public void AddPropobility(AttackModifier attackModifier, float addedPropobility)
		{
			if (!this.ProbabilitiesDictionary.ContainsKey(attackModifier))
			{
				this.ProbabilitiesDictionary.Add(attackModifier, addedPropobility);
			}
			else
			{
				float num = this.ProbabilitiesDictionary[attackModifier];
				this.ProbabilitiesDictionary[attackModifier] = num + (100f - num) / 100f * addedPropobility;
			}
		}

		public AttackModifier[] GenerateModifiers()
		{
			List<AttackModifier> list = new List<AttackModifier>();
			foreach (KeyValuePair<AttackModifier, float> keyValuePair in this.ProbabilitiesDictionary)
			{
				if (UnityEngine.Random.Range(0f, 100f) < keyValuePair.Value)
				{
					list.Add(keyValuePair.Key);
				}
			}
			return list.ToArray();
		}

		public int GenerateModifiersNonAlloc(AttackModifier[] attackModifiers)
		{
			int num = 0;
			int num2 = attackModifiers.Length;
			foreach (KeyValuePair<AttackModifier, float> keyValuePair in this.ProbabilitiesDictionary)
			{
				if (num >= num2)
				{
					break;
				}
				if (UnityEngine.Random.Range(0f, 100f) < keyValuePair.Value)
				{
					attackModifiers[num] = keyValuePair.Key;
					num++;
				}
			}
			return num;
		}

		public void GenerateModifiersToList(List<AttackModifier> attackModifiers)
		{
			attackModifiers.Clear();
			foreach (KeyValuePair<AttackModifier, float> keyValuePair in this.ProbabilitiesDictionary)
			{
				if (UnityEngine.Random.Range(0f, 100f) < keyValuePair.Value)
				{
					attackModifiers.Add(keyValuePair.Key);
				}
			}
		}

		public void Normalize()
		{
			float num = 0f;
			for (int i = this.m_modifierProbabilities.Count - 1; i >= 0; i--)
			{
				if (this.m_modifierProbabilities[i] == null)
				{
					this.m_modifierProbabilities.RemoveAt(i);
				}
				else
				{
					num += this.m_modifierProbabilities[i].Probability;
				}
			}
			if (this.m_modifierProbabilities.Count > 0)
			{
				if (num > 0f)
				{
					float num2 = 100f / num;
					foreach (AttackModifierProbability attackModifierProbability in this.m_modifierProbabilities)
					{
						attackModifierProbability.Probability *= num2;
					}
				}
				else
				{
					float probability = 100f / (float)this.m_modifierProbabilities.Count;
					foreach (AttackModifierProbability attackModifierProbability2 in this.m_modifierProbabilities)
					{
						attackModifierProbability2.Probability = probability;
					}
				}
			}
		}

		[SerializeField]
		private List<AttackModifierProbability> m_modifierProbabilities;

		private Dictionary<AttackModifier, float> m_probabilitiesDictionary;
	}
}
