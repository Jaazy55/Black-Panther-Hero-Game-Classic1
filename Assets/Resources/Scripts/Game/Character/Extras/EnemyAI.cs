using System;
using Game.Character.CharacterController;
using Game.Character.Input;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Character.Extras
{
	public class EnemyAI : HitEntity
	{
		protected override void Start()
		{
			base.Start();
			this.agent = base.GetComponent<NavMeshAgent>();
			this.animState = EnemyAI.AnimationState.Idle;
			this.player = EntityManager.Instance.Player;
			this.velocityFilter = new InputFilter(10, 1f);
		}

		protected virtual void UpdateAnimState()
		{
		}

		protected override void Update()
		{
			if (base.IsDead)
			{
				this.deadTimeout += Time.deltaTime;
				if (EnemyAI.corpseCounter > 10 && this.deadTimeout > 5f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					EnemyAI.corpseCounter--;
				}
				return;
			}
			if (!this.player || this.player.IsDead)
			{
				this.animState = EnemyAI.AnimationState.Idle;
			}
			this.UpdateAnimState();
			if (!this.player)
			{
				return;
			}
			float sqrMagnitude = (this.player.transform.position - base.transform.position).sqrMagnitude;
			if (sqrMagnitude < this.AttackDistance * this.AttackDistance)
			{
				this.animState = EnemyAI.AnimationState.Attack;
				this.agent.Stop();
				this.attackTimer -= Time.deltaTime;
				if (this.attackTimer < 0f)
				{
					this.player.OnHit(DamageType.Bullet, this, 10f, base.transform.position + base.transform.forward + Vector3.up * 0.5f, Vector3.zero, 0f);
					this.attackTimer = this.AttackTimeout;
				}
			}
			else
			{
				Vector3 destination = this.player.transform.position - (this.player.transform.position - base.transform.position).normalized;
				destination = this.player.transform.position;
				this.agent.SetDestination(destination);
				if (this.agent.hasPath)
				{
					if (this.Aggresive)
					{
						this.animState = EnemyAI.AnimationState.Run;
					}
					else
					{
						this.animState = EnemyAI.AnimationState.Walk;
					}
					Vector3 normalized = (-(base.transform.position - this.agent.steeringTarget)).normalized;
					Vector3 a = this.SmoothVelocityVector(normalized);
					base.transform.position += a * this.agentSpeed * Time.deltaTime;
					normalized.y = 0f;
					if (normalized != Vector3.zero)
					{
						base.transform.forward = normalized;
					}
				}
				else
				{
					this.animState = EnemyAI.AnimationState.Idle;
				}
			}
		}

		protected Vector3 SmoothVelocityVector(Vector3 v)
		{
			this.velocityFilter.AddSample(new Vector2(v.x, v.z));
			Vector2 value = this.velocityFilter.GetValue();
			Vector3 vector = new Vector3(value.x, 0f, value.y);
			return vector.normalized;
		}

		protected override void OnDie()
		{
			base.OnDie();
			this.animState = EnemyAI.AnimationState.Dead;
			this.UpdateAnimState();
			base.GetComponent<Collider>().enabled = false;
			this.agent.enabled = false;
			EnemyAI.corpseCounter++;
			this.deadTimeout = 0f;
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
			this.Aggresive = true;
		}

		protected override void FixedUpdate()
		{
			if (this.animState == EnemyAI.AnimationState.Attack)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(this.player.transform.position - base.transform.position), Time.deltaTime * 10f);
			}
		}

		public float AttackDistance = 2f;

		public float AttackTimeout = 1f;

		public bool Aggresive;

		protected NavMeshAgent agent;

		protected float attackTimer;

		protected HitEntity player;

		protected InputFilter velocityFilter;

		protected float targetSpeed;

		protected static int corpseCounter;

		protected float deadTimeout;

		protected float agentSpeed;

		protected EnemyAI.AnimationState animState;

		protected enum AnimationState
		{
			Idle,
			Walk,
			Run,
			Attack,
			Dead
		}
	}
}
