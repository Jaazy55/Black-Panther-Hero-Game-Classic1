using System;
using Game.Character.CharacterController;
using Naxeex.AttaskSystem;
using UnityEngine;

namespace Naxeex.AI.NPC
{
	public class NPCBehaviour : MonoBehaviour
	{
		public Transform MoveTarget
		{
			get
			{
				return this.m_Movable.MoveTarget;
			}
		}

		public Transform RotateTarget
		{
			get
			{
				return this.m_Movable.RotateTarget;
			}
		}

		public void Attack(int attackIndex, IAttackReceiver attackReceiverreceiver)
		{
			this.m_Attacker.Attack(attackReceiverreceiver, attackIndex);
		}

		public void StopAttack()
		{
			this.m_Attacker.Stop();
		}

		public void MoveToTarget(Transform target)
		{
			this.m_Movable.FollowTo(target);
		}

		public void RotateTo(Transform target)
		{
			this.m_Movable.RotateTo(target);
		}

		public void ToRoam()
		{
			this.m_Movable.ToRoam();
		}

		public void StopMove()
		{
			this.m_Movable.Stop();
		}

		public void Die()
		{
			this.HitEntity.Die();
		}

		protected virtual void Start()
		{
			this.BehaviourController = new NPCBehaviourController(this, this.m_BehaviourGraph);
		}

		protected virtual void FixedUpdate()
		{
			if (Time.fixedTime > this.m_NextBehaviourUpdateTime)
			{
				this.BehaviourController.ProcessUpdate();
				this.m_NextBehaviourUpdateTime = Time.fixedTime + this.m_BehaviourUpdateTime;
			}
		}

		[SerializeField]
		private NPCBehaviourGraph m_BehaviourGraph;

		[SerializeField]
		[Range(0.015f, 0.3f)]
		private float m_BehaviourUpdateTime = 0.05f;

		[SerializeField]
		private NPCMovable m_Movable;

		[SerializeField]
		private AnimatedAttackBehaviour m_Attacker;

		[SerializeField]
		private HitEntity HitEntity;

		private float m_NextBehaviourUpdateTime;

		private NPCBehaviourController BehaviourController;
	}
}
