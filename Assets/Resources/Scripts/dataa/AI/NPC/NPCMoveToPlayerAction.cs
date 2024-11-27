using System;
using UnityEngine;

namespace Naxeex.AI.NPC
{
	[CreateAssetMenu(fileName = "NPCMoveToPlayer", menuName = "NPC/MoveToPlayer State Action", order = 10)]
	public class NPCMoveToPlayerAction : NPCInteractionPlayerAction
	{
		internal override void OnStateEnter(NPCBehaviour entity)
		{
			base.OnStateEnter(entity);
			Transform playerOrRagdollTransform = base.PlayerOrRagdollTransform;
			if (playerOrRagdollTransform != null)
			{
				entity.MoveToTarget(playerOrRagdollTransform);
			}
			else
			{
				entity.ToRoam();
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
				entity.MoveToTarget(playerOrRagdollTransform);
			}
		}

		protected override void RagdollToPlayerAction(NPCBehaviour entity)
		{
			entity.MoveToTarget(base.PlayerTransform);
		}

		protected override void PlayerToRagdollAction(NPCBehaviour entity)
		{
			Transform ragdollTransform = base.RagdollTransform;
			if (ragdollTransform != null)
			{
				entity.MoveToTarget(ragdollTransform);
			}
			else
			{
				entity.ToRoam();
			}
		}
	}
}
