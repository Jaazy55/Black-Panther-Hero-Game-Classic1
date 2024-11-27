using System;
using UnityEngine;

namespace Naxeex.AI.NPC
{
	[CreateAssetMenu(fileName = "NearPlayerRotationDecision", menuName = "NPC/NearPlayerRotation Process Decision", order = 20)]
	public class NearPlayerRotationDecision : InteractionPlayerDecision
	{
		public override bool GetDecision(NPCBehaviour entity)
		{
			if (base.PlayerOrRagdollTransform != null)
			{
				Vector3 from = base.PlayerOrRagdollTransform.position - entity.transform.position;
				from.y = 0f;
				return Vector3.Angle(from, entity.transform.forward) < this.m_ViewingAngle;
			}
			return false;
		}

		[SerializeField]
		private float m_ViewingAngle;
	}
}
