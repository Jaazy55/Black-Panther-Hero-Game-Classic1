using System;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	[Serializable]
	public class AttackModifierProbability
	{
		public AttackModifierProbability(AttackModifier modifier, float probability)
		{
			this.m_modifier = modifier;
			this.m_probability = probability;
		}

		public AttackModifier Modifier
		{
			get
			{
				return this.m_modifier;
			}
		}

		public float Probability
		{
			get
			{
				return this.m_probability;
			}
			set
			{
				this.m_probability = Mathf.Clamp(value, 0f, 100f);
			}
		}

		[SerializeField]
		private AttackModifier m_modifier;

		[SerializeField]
		[Range(0f, 100f)]
		private float m_probability;
	}
}
