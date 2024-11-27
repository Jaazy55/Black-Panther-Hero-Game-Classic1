using System;
using Game.GlobalComponent;
using Game.Traffic;
using UnityEngine;

namespace Game.Enemy
{
	public class PedestrianHumanoidController : SimpleHumanoidController
	{
		public override void Init(BaseNPC controlledNPC)
		{
			base.Init(controlledNPC);
			if (this.currentFromPoint == null)
			{
				this.currentFromPoint = TrafficManager.Instance.FindClosestPedestrianPoint(controlledNPC.transform.position);
				if (this.currentFromPoint != null)
				{
					this.currentLine = UnityEngine.Random.Range(0, this.currentFromPoint.LineCount) + 1;
					this.currentToPoint = TrafficManager.BestDestinationPoint(this.currentFromPoint);
					this.RecalcMovePoint();
				}
			}
		}

		public override void DeInit()
		{
			this.currentFromPoint = null;
			this.currentToPoint = null;
			base.DeInit();
		}

		public void InitPedestrianPath(RoadPoint fromPoint, RoadPoint toPoint, int line)
		{
			this.currentFromPoint = fromPoint;
			this.currentToPoint = toPoint;
			this.currentLine = line;
			this.RecalcMovePoint();
		}

		private void Awake()
		{
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 0.5f);
		}

		private void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if (this.currentFromPoint == null)
			{
				return;
			}
			if (!base.IsMoving && this.ObstacleSensor.CanMove)
			{
				TrafficManager.Instance.GetNextRoute(ref this.currentFromPoint, ref this.currentToPoint, ref this.currentLine);
				this.RecalcMovePoint();
			}
			if (this.stayingTime > 4f)
			{
				RoadPoint roadPoint = this.currentToPoint;
				this.currentToPoint = this.currentFromPoint;
				this.currentFromPoint = roadPoint;
				this.RecalcMovePoint();
			}
			if (!base.IsMoving)
			{
				this.stayingTime += this.slowUpdateProc.DeltaTime;
			}
			else
			{
				this.stayingTime = 0f;
			}
		}

		private void RecalcMovePoint()
		{
			Vector3 vector;
			Vector3 movePoint;
			TrafficManager.Instance.CalcTargetSidewalkPoint(this.currentFromPoint, this.currentToPoint, this.currentLine, out vector, out movePoint);
			base.SetMovePoint(movePoint);
			this.stayingTime = 0f;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(this.MovePoint, 0.5f);
		}

		private const float StayTimeout = 4f;

		private SlowUpdateProc slowUpdateProc;

		private RoadPoint currentFromPoint;

		private RoadPoint currentToPoint;

		private int currentLine;

		private float stayingTime;
	}
}
