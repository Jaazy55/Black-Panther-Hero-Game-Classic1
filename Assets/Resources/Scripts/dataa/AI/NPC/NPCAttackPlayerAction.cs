using System;
using Naxeex.AttaskSystem;
using UnityEngine;

namespace Naxeex.AI.NPC
{
	[CreateAssetMenu(fileName = "NPCAttackPlayer", menuName = "NPC/AttackPlayer State Action", order = 10)]
	public class NPCAttackPlayerAction : NPCInteractionPlayerAction
	{
		protected IAttackReceiver PlayerAttackReceiver
		{
			get
			{
				if (this.m_PlayerAttackReceiver == null && base.Player != null)
				{
					this.m_PlayerAttackReceiver = base.Player.GetComponent<AttackableHitEntity>();
				}
				return this.m_PlayerAttackReceiver;
			}
		}

		internal override void OnStateEnter(NPCBehaviour entity)
		{
			if (this.PlayerAttackReceiver != null)
			{
				entity.Attack(this.AttackIndex, this.PlayerAttackReceiver);
			}
		}

		internal override void OnStateExit(NPCBehaviour entity)
		{
			if (this.PlayerAttackReceiver != null)
			{
				entity.StopAttack();
			}
		}

		internal override void OnStateUpdate(NPCBehaviour entity)
		{
		}

		protected override void PlayerToRagdollAction(NPCBehaviour entity)
		{
		}

		protected override void RagdollToPlayerAction(NPCBehaviour entity)
		{
		}

		[SerializeField]
		private int AttackIndex;

		private AttackableHitEntity m_PlayerAttackReceiver;
	}
}
