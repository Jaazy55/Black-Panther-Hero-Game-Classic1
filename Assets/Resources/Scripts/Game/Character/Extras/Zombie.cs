using System;
using Game.Character.Demo;
using UnityEngine;

namespace Game.Character.Extras
{
	public class Zombie : EnemyAI
	{
		protected override void Start()
		{
			base.Start();
			this.animator = base.GetComponent<Animator>();
			this.animator.applyRootMotion = false;
			this.injured = (UnityEngine.Random.value > 0.5f);
		}

		protected override void UpdateAnimState()
		{
			float b = this.targetSpeed;
			switch (this.animState)
			{
			case EnemyAI.AnimationState.Idle:
				b = 0f;
				this.agent.Stop();
				break;
			case EnemyAI.AnimationState.Walk:
				b = 0.5f;
				break;
			case EnemyAI.AnimationState.Run:
				b = 1f;
				break;
			case EnemyAI.AnimationState.Attack:
				b = 0.5f;
				break;
			case EnemyAI.AnimationState.Dead:
				this.targetSpeed = 0f;
				this.agent.Stop();
				break;
			}
			this.targetSpeed = Mathf.Lerp(this.targetSpeed, b, Time.deltaTime * 5f);
			float targetSpeed = this.targetSpeed;
			this.agentSpeed = 0f;
			if (this.targetSpeed <= 0.5f)
			{
				this.agentSpeed = this.targetSpeed / 0.5f;
			}
			else
			{
				this.agentSpeed = (this.targetSpeed - 0.5f) / 0.5f * 3.5f + 0.5f;
			}
			this.animator.SetFloat("Speed", targetSpeed);
			this.animator.SetBool("Injured", this.injured);
		}

		protected override void OnDie()
		{
			base.OnDie();
			if (ZombieHUD.Instance)
			{
				ZombieHUD.Instance.ZombieKilled();
			}
			this.animator.SetBool("Die", true);
			EnemyAI.corpseCounter++;
			this.deadTimeout = 0f;
		}

		private Animator animator;

		private bool injured;
	}
}
