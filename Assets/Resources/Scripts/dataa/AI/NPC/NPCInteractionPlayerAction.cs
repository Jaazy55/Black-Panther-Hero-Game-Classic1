using System;
using System.Collections.Generic;
using Game.Character;
using Game.Character.CharacterController;
using Game.Enemy;
using Naxeex.AI.StateSystem;
using UnityEngine;
using Action = System.Action;

namespace Naxeex.AI.NPC
{
	public abstract class NPCInteractionPlayerAction : StateAction<NPCBehaviour>
	{
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
				return this.RagdollTransform;
			}
		}

		protected Transform RagdollTransform { get; private set; }

		protected Transform PlayerTransform
		{
			get
			{
				return (!(this.Player != null)) ? null : this.Player.transform;
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

		internal override void OnStateEnter(NPCBehaviour entity)
		{
			if (!this.RagdollHandlers.ContainsKey(entity))
			{
				Action<RagdollWakeUper> ragdollHandler = this.GetRagdollHandler(entity);
				this.RagdollHandlers.Add(entity, ragdollHandler);
				this.Player.OnRagdoll += ragdollHandler;
			}
		}

		internal override void OnStateExit(NPCBehaviour entity)
		{
			if (this.RagdollHandlers.ContainsKey(entity))
			{
				this.Player.OnRagdoll -= this.RagdollHandlers[entity];
				this.RagdollHandlers.Remove(entity);
			}
		}

		private Action<RagdollWakeUper> GetRagdollHandler(NPCBehaviour entity)
		{
			return delegate(RagdollWakeUper ragdollWakeUper)
			{
				if (ragdollWakeUper != null)
				{
					RagdollTransform = ragdollWakeUper.transform;
					Action action = null;
					action = delegate
					{
						RagdollToPlayerAction(entity);
						ragdollWakeUper.OnDeInit -= action;
						RagdollTransform = null;
					};
					ragdollWakeUper.OnDeInit += action;
				}
				PlayerToRagdollAction(entity);
			};
		}

		protected abstract void RagdollToPlayerAction(NPCBehaviour entity);

		protected abstract void PlayerToRagdollAction(NPCBehaviour entity);

		private Player m_player;

		private Dictionary<NPCBehaviour, Action<RagdollWakeUper>> RagdollHandlers = new Dictionary<NPCBehaviour, Action<RagdollWakeUper>>();
	}
}
