using System;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	[CreateAssetMenu(fileName = "AttackData", menuName = "Attack System/Attack Data", order = 10)]
	public class AttackData : ScriptableObject
	{
		public float Damage
		{
			get
			{
				return UnityEngine.Random.Range(this.m_MinDamage, this.m_MaxDamage);
			}
		}

		public AttackModifier[] Modifiers
		{
			get
			{
				return this.m_ModifierCollection.GenerateModifiers();
			}
		}

		[SerializeField]
		private float m_MinDamage;

		[SerializeField]
		private float m_MaxDamage;

		[SerializeField]
		private AttackModifierCollection m_ModifierCollection;
	}
}
