using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	public class AnimatedAttackBehaviour : MonoBehaviour, IAttackSource
	{
		public int CurrentAttackIndex
		{
			get
			{
				if (this.m_AttackIndex >= 0 && this.m_AttackIndex < this.m_Attacks.Length)
				{
					return this.m_AttackIndex;
				}
				return -1;
			}
			set
			{
				if ((this.m_AttackIndex != value && value >= 0 && value < this.m_Attacks.Length) || value == -1)
				{
					this.m_AttackIndex = value;
				}
			}
		}

		public AnimatedAttackData CurrentAttackData
		{
			get
			{
				if (this.m_AttackIndex >= 0 && this.m_AttackIndex < this.m_Attacks.Length)
				{
					return this.m_Attacks[this.m_AttackIndex];
				}
				return null;
			}
		}

		public IEnumerable<AttackModifier> Modifiers
		{
			get
			{
				if (this.CurrentAttackData != null)
				{
					return this.m_Attacks[this.m_AttackIndex].Modifiers;
				}
				return null;
			}
		}

		public AnimatedAttackData[] Attacks
		{
			get
			{
				return this.m_Attacks;
			}
			set
			{
				this.m_Attacks = value;
			}
		}

		protected AnimatorOverrideController AnimatorController
		{
			get
			{
				if (this.m_Animator != null && (this.m_AnimatorController == null || this.m_Animator.runtimeAnimatorController != this.m_AnimatorController))
				{
					this.m_AnimatorController = (this.m_Animator.runtimeAnimatorController as AnimatorOverrideController);
					if (this.m_AnimatorController == null)
					{
						this.m_AnimatorController = new AnimatorOverrideController(this.m_Animator.runtimeAnimatorController);
						this.m_Animator.runtimeAnimatorController = this.m_AnimatorController;
					}
				}
				return this.m_AnimatorController;
			}
		}

		public float Damage
		{
			get
			{
				if (this.CurrentAttackData != null)
				{
					return this.m_Attacks[this.m_AttackIndex].Damage;
				}
				return 0f;
			}
		}

		public bool CanAttack(IAttackReceiver receiver)
		{
			return true;
		}

		public void Attack(IAttackReceiver receiver)
		{
			AnimatedAttackData currentAttackData = this.CurrentAttackData;
			if (this.m_Animator == null && currentAttackData == null)
			{
				return;
			}
			this.AnimatorController[currentAttackData.OverrideName] = currentAttackData.Clip;
			currentAttackData.EnterConditions(this.m_Animator);
			this.GenerateAttack(currentAttackData, receiver);
		}

		public void Attack(IAttackReceiver receiver, int attackIndex)
		{
			this.CurrentAttackIndex = attackIndex;
			this.Attack(receiver);
		}

		public void Stop()
		{
			if (this.CurrentAttackData != null)
			{
				this.CurrentAttackData.ExitCondition(this.m_Animator);
			}
			this.CurrentAttackIndex = -1;
		}

		public void ResultHandler(Attack attack, AttackResult result)
		{
			if (attack != null && this.attackInfos.ContainsKey(attack))
			{
				this.attackInfos[attack].ExitCondition(this.m_Animator);
				this.attackInfos.Remove(attack);
			}
		}

		private void GenerateAttack(AnimatedAttackData attackData, IAttackReceiver receiver)
		{
			Attack key = AttackManager.Create(receiver, this, Time.time + attackData.Delay);
			this.RepeatAttackTimer = attackData.Length;
			this.LastReceiver = receiver;
			this.attackInfos.Add(key, attackData);
		}

		private void Update()
		{
			if (this.CurrentAttackData != null)
			{
				this.CurrentAttackData.StayConditions(this.m_Animator);
			}
		}

		private void FixedUpdate()
		{
			if (this.RepeatAttackTimer >= 0f)
			{
				this.RepeatAttackTimer -= Time.fixedDeltaTime;
				if (this.RepeatAttackTimer <= 0f && this.CurrentAttackData != null && this.LastReceiver != null)
				{
					this.GenerateAttack(this.CurrentAttackData, this.LastReceiver);
				}
			}
		}

		[SerializeField]
		private Animator m_Animator;

		[SerializeField]
		private AnimatedAttackData[] m_Attacks;

		private const int ErrorAttackIndex = -1;

		private AnimatorOverrideController m_AnimatorController;

		private int m_AttackIndex = -1;

		private Dictionary<Attack, AnimatedAttackData> attackInfos = new Dictionary<Attack, AnimatedAttackData>();

		private float RepeatAttackTimer = -1f;

		private IAttackReceiver LastReceiver;
	}
}
