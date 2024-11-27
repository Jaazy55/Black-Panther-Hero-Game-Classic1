using System;
using UnityEngine;

namespace Naxeex.AI.NPC
{
	[CreateAssetMenu(fileName = "NPCRotateToPlayer", menuName = "NPC/RotateToPlayer State Action", order = 10)]
	public class NPCRotateToPlayerAction : NPCInteractionPlayerAction
	{
		internal override void OnStateEnter(NPCBehaviour entity)
		{
			base.OnStateEnter(entity);
			Transform playerOrRagdollTransform = base.PlayerOrRagdollTransform;
			if (playerOrRagdollTransform != null)
			{
				entity.RotateTo(playerOrRagdollTransform);
			}
			else
			{
				entity.StopMove();
			}
		}

		internal override void OnStateExit(NPCBehaviour entity)
		{
			base.OnStateExit(entity);
			entity.StopMove();
		}

		internal override void OnStateUpdate(NPCBehaviour entity)
		{
			Transform playerOrRagdollTransform = base.PlayerOrRagdollTransform;
			if (playerOrRagdollTransform != null && entity.MoveTarget != playerOrRagdollTransform)
			{
				entity.RotateTo(playerOrRagdollTransform);
			}
		}

		protected override void PlayerToRagdollAction(NPCBehaviour entity)
		{
			entity.RotateTo(base.PlayerTransform);
		}

		protected override void RagdollToPlayerAction(NPCBehaviour entity)
		{
			Transform ragdollTransform = base.RagdollTransform;
			if (ragdollTransform != null)
			{
				entity.RotateTo(ragdollTransform);
			}
			else
			{
				entity.StopMove();
			}
		}
	}
}
