using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.Enemy;
using Naxeex.AI.StateSystem;
using UnityEngine;

namespace Naxeex.AI.NPC
{
	public abstract class InteractionPlayerDecision : ProcessedDecision<NPCBehaviour>
	{
		private void OnEnable()
		{
			this.nextTryTime = 0f;
			this.m_ragdollTransform = null;
		}

		protected Transform PlayerOrRagdollTransform
		{
			get
			{
				if (this.Player == null)
				{
					return null;
				}
				if (this.Player.gameObject.activeSelf)
				{
					return this.Player.transform;
				}
				if (this.m_ragdollTransform == null && Time.time > this.nextTryTime)
				{
					GameObject currentRagdoll = this.Player.GetCurrentRagdoll();
					if (currentRagdoll != null)
					{
						RagdollWakeUper componentInChildren = currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
						if (componentInChildren != null)
						{
							this.m_ragdollTransform = componentInChildren.transform;
						}
					}
					this.nextTryTime = Time.time + 0.1f;
				}
				return this.m_ragdollTransform;
			}
		}

		protected Player Player
		{
			get
			{
				if (this.m_player == null && PlayerInteractionsManager.HasInstance)
				{
					this.m_player = PlayerInteractionsManager.Instance.Player;
				}
				return this.m_player;
			}
		}

		private const float tryPeriod = 0.1f;

		private Player m_player;

		private Transform m_ragdollTransform;

		private float nextTryTime;
	}
}
