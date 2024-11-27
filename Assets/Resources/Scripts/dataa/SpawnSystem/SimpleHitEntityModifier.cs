using System;
using Game.Character.CharacterController;
using Naxeex.AttaskSystem;
using UnityEngine;

namespace Naxeex.SpawnSystem
{
	[CreateAssetMenu(fileName = "SimpleHitEntityModifier", menuName = "NPC/Create Simple Hit Entity Modifier", order = 2)]
	public class SimpleHitEntityModifier : HitEntityModifier
	{
		public override void Modify(HitEntity entity)
		{
			entity.Health.Max = this.m_Health;
			entity.Health.Current = this.m_Health;
			AnimatedAttackBehaviour component = entity.GetComponent<AnimatedAttackBehaviour>();
			if (component != null)
			{
				component.Attacks = this.m_Attacks;
			}
		}

		[SerializeField]
		private float m_Health;

		[SerializeField]
		private AnimatedAttackData[] m_Attacks;
	}
}
