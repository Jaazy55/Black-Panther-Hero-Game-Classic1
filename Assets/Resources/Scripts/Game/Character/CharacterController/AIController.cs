using System;
using Game.Character.Extras;
using Game.Character.Input;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Character.CharacterController
{
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(AnimationController))]
	public class AIController : MonoBehaviour
	{
		public void Activate(bool status)
		{
			base.enabled = status;
			this.targetPos = base.transform.position;
			this.agent.enabled = status;
		}

		private void Awake()
		{
			this.agent = base.GetComponent<NavMeshAgent>();
			this.agent.enabled = true;
			this.animController = base.GetComponent<AnimationController>();
			this.inputManager = InputManager.Instance;
			this.human = base.GetComponent<Human>();
			this.velocityFilter = new InputFilter(10, 1f);
		}

		private void UpdateInput()
		{
			if (this.human.Remote)
			{
				return;
			}
			if (!this.human.IsDead)
			{
				Game.Character.Input.Input input = this.inputManager.GetInput(InputType.WaypointPos);
				if (input.Valid)
				{
					this.enemyTarget = EntityManager.Instance.Find((Vector3)input.Value, 2f, "Player");
					this.SetTarget((Vector3)input.Value);
					this.stickMove = false;
				}
				else
				{
					Game.Character.Input.Input input2 = this.inputManager.GetInput(InputType.Move);
					bool flag = this.inputManager.InputPreset == InputPreset.RPG;
					if (input2.Valid && flag)
					{
						Vector2 vector = (Vector2)input2.Value;
						Transform transform = CameraManager.Instance.UnityCamera.transform;
						Vector3 normalized = Vector3.Scale(transform.forward, new Vector3(1f, 0f, 1f)).normalized;
						Vector3 a = vector.y * normalized + vector.x * transform.right;
						this.stickMove = true;
						this.SetTarget(base.gameObject.transform.position + a * 10f);
					}
				}
			}
		}

		private void UpdateEnemyTarget()
		{
			this.attackEnemy = false;
			if (this.enemyTarget)
			{
				if (RTSProjector.Instance)
				{
					RTSProjector.Instance.Project(this.enemyTarget.transform.position, Color.red);
				}
				if (this.enemyTarget.IsDead)
				{
					this.enemyTarget = null;
					this.attackEnemy = false;
					this.agent.ResetPath();
				}
				else
				{
					float sqrMagnitude = (this.enemyTarget.transform.position - base.transform.position).sqrMagnitude;
					if (sqrMagnitude < this.AttackDistance * this.AttackDistance)
					{
						this.attackEnemy = true;
					}
				}
			}
		}

		private void Update()
		{
			this.UpdateInput();
			this.UpdateEnemyTarget();
			Vector3 vector = Vector3.zero;
			Vector3 zero = Vector3.zero;
			Vector3 vector2 = base.transform.position + base.transform.forward + Vector3.up * this.TargetLookOffset;
			bool status = false;
			if (this.agent.hasPath)
			{
				if (this.attackEnemy)
				{
					status = true;
					this.human.Attack(this.enemyTarget);
					this.agent.Stop();
					vector = this.SmoothVelocityVector((vector2 - base.transform.position).normalized);
					vector2 = this.enemyTarget.transform.position + Vector3.up * 1.6f;
				}
				else if ((base.transform.position - this.agent.destination).sqrMagnitude > 1f)
				{
					vector = this.SmoothVelocityVector((this.agent.steeringTarget - base.transform.position).normalized) * 1f;
					zero.y = 1f;
					vector2 = base.transform.position + vector + Vector3.up * this.TargetLookOffset;
				}
			}
			this.human.Aim(status);
			bool input = this.inputManager.GetInput<bool>(InputType.Reset, false);
			if (input)
			{
				CameraManager.Instance.ResetCameraMode();
				this.human.Resurrect();
			}
			bool isDead = this.human.IsDead;
			if (isDead)
			{
			}
			bool jump = false;
			this.animController.Move(new Game.Character.CharacterController.Input
			{
				camMove = vector,
				inputMove = zero,
				smoothAimRotation = true,
				crouch = false,
				jump = jump,
				lookPos = vector2,
				die = isDead,
				reset = input
			});
		}

		public void SetTarget(Vector3 target)
		{
			if ((target - this.targetPos).magnitude > this.TargetChangeTolerance)
			{
				this.targetPos = target;
				this.agent.SetDestination(this.targetPos);
				if (!this.enemyTarget && !this.stickMove && RTSProjector.Instance)
				{
					RTSProjector.Instance.Project(target, Color.white);
				}
			}
		}

		private Vector3 SmoothVelocityVector(Vector3 v)
		{
			this.velocityFilter.AddSample(new Vector2(v.x, v.z));
			Vector2 value = this.velocityFilter.GetValue();
			Vector3 vector = new Vector3(value.x, 0f, value.y);
			return vector.normalized;
		}

		public float AttackDistance = 5f;

		public float TargetChangeTolerance = 1f;

		public float Speed = 100f;

		public float TargetLookOffset = 1.4f;

		protected NavMeshAgent agent;

		protected AnimationController animController;

		private Vector3 targetPos;

		private InputManager inputManager;

		private Human human;

		private InputFilter velocityFilter;

		private HitEntity enemyTarget;

		private bool attackEnemy;

		private bool stickMove;
	}
}
