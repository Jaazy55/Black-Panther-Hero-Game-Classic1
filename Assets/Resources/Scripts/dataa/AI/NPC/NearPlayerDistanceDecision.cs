using System;
using UnityEngine;

namespace Naxeex.AI.NPC
{
	[CreateAssetMenu(fileName = "NearPlayerDistanceDecision", menuName = "NPC/NearPlayerDistance Process Decision", order = 20)]
	public class NearPlayerDistanceDecision : InteractionPlayerDecision
	{
		public override bool GetDecision(NPCBehaviour entity)
		{
			return base.PlayerOrRagdollTransform != null && Vector3.Distance(entity.transform.position, base.PlayerOrRagdollTransform.position) < this.m_Range;
		}

		[SerializeField]
		private float m_Range;
	}
}
