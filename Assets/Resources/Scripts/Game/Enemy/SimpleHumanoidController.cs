using System;
using Game.Character.CharacterController;
using Game.Managers;
using UnityEngine;

namespace Game.Enemy
{
	public class SimpleHumanoidController : BaseControllerNPC
	{
		public bool IsMoving
		{
			get
			{
				return this.isMoving;
			}
		}

		public override void Init(BaseNPC controlledNPC)
		{
			base.Init(controlledNPC);
			this.MovePoint = this.CurrentControlledNpc.transform.position;
			BaseStatusNPC statusNpc = this.CurrentControlledNpc.StatusNpc;
			statusNpc.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Combine(statusNpc.OnHitEvent, new HitEntity.HealthChagedEvent(this.BecomeSmartOnHit));
		}

		public override void DeInit()
		{
			BaseStatusNPC statusNpc = this.CurrentControlledNpc.StatusNpc;
			statusNpc.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Remove(statusNpc.OnHitEvent, new HitEntity.HealthChagedEvent(this.BecomeSmartOnHit));
			base.DeInit();
		}

		public void SetMovePoint(Vector3 newPoint)
		{
			this.MovePoint = newPoint;
		}

		protected override void Update()
		{
			if (!base.IsInited)
			{
				return;
			}
			base.Update();
			float num = Vector3.Distance(this.CurrentControlledNpc.transform.position, this.MovePoint);
			this.isMoving = (num > this.StoppingDistance && this.ObstacleSensor.CanMove);
			this.LerpLookToPoint(this.MovePoint);
			if (this.isMoving)
			{
				this.MoveToPoint(this.MovePoint);
			}
			else
			{
				this.StayOnPosition();
			}
		}

		private void LerpLookToPoint(Vector3 point)
		{
			Vector3 a = new Vector3(point.x, this.CurrentControlledNpc.transform.position.y, point.z);
			Vector3 normalized = (a - this.CurrentControlledNpc.transform.position).normalized;
			float num = Vector3.Dot(normalized, this.CurrentControlledNpc.transform.forward);
			float num2 = 0.5f - num * 0.5f;
			if (num2 > 1.401298E-45f)
			{
				float t = Mathf.Min(1f, this.RotateSpeed * Time.deltaTime / num2);
				this.CurrentControlledNpc.transform.forward = Vector3.Slerp(this.CurrentControlledNpc.transform.forward, normalized, t);
			}
		}

		private void MoveToPoint(Vector3 point)
		{
			this.MovePoint = point;
			this.CurrentControlledNpc.NPCAnimator.SetBool("Walk", true);
		}

		private void StayOnPosition()
		{
			this.CurrentControlledNpc.NPCAnimator.SetBool("Walk", false);
		}

		private void BecomeSmartOnHit(HitEntity disturber)
		{
			BaseControllerNPC baseControllerNPC;
			this.CurrentControlledNpc.ChangeController(BaseNPC.NPCControllerType.Smart, out baseControllerNPC);
			SmartHumanoidController smartHumanoidController = baseControllerNPC as SmartHumanoidController;
			if (smartHumanoidController != null)
			{
				smartHumanoidController.InitBackToDummyLogic();
				if (this.DebugLog_SimpleHumanoidController && GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log(disturber);
				}
				smartHumanoidController.AddPersonalTarget(disturber);
			}
		}

		[Separator("Simple Controller Paremetrs")]
		public bool DebugLog_SimpleHumanoidController;

		public float StoppingDistance = 1f;

		public float RotateSpeed = 1f;

		[Separator("Simple Controller Links")]
		public DummyNPCSensor ObstacleSensor;

		protected Vector3 MovePoint;

		private bool isMoving;
	}
}
