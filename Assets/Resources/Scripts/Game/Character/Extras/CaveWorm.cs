using System;
using Game.Character.Demo;
using UnityEngine;

namespace Game.Character.Extras
{
	public class CaveWorm : EnemyAI
	{
		protected override void UpdateAnimState()
		{
			float b = this.targetSpeed;
			switch (this.animState)
			{
			case EnemyAI.AnimationState.Idle:
				base.GetComponent<Animation>().clip = this.ClipIdle;
				base.GetComponent<Animation>().Play();
				this.agent.Stop();
				b = 0f;
				break;
			case EnemyAI.AnimationState.Walk:
				base.GetComponent<Animation>().clip = this.ClipMove;
				base.GetComponent<Animation>().Play();
				b = 2f;
				break;
			case EnemyAI.AnimationState.Attack:
				base.GetComponent<Animation>().clip = this.ClipAttack;
				base.GetComponent<Animation>().Play();
				b = 0.5f;
				break;
			case EnemyAI.AnimationState.Dead:
				base.GetComponent<Animation>().clip = this.ClipDead;
				base.GetComponent<Animation>().wrapMode = WrapMode.Once;
				base.GetComponent<Animation>().Play();
				this.agent.Stop();
				b = 0.5f;
				break;
			}
			this.targetSpeed = Mathf.Lerp(this.targetSpeed, b, Time.deltaTime * 5f);
			this.agentSpeed = 0f;
			if (this.targetSpeed <= 0.5f)
			{
				this.agentSpeed = this.targetSpeed / 0.5f;
			}
			else
			{
				this.agentSpeed = (this.targetSpeed - 0.5f) / 0.5f * 3.5f + 0.5f;
			}
			this.agentSpeed = this.targetSpeed;
		}

		protected override void OnDie()
		{
			base.OnDie();
			if (ZombieHUD.Instance)
			{
				ZombieHUD.Instance.WormKilled();
			}
		}

		public AnimationClip ClipIdle;

		public AnimationClip ClipMove;

		public AnimationClip ClipAttack;

		public AnimationClip ClipDead;
	}
}
