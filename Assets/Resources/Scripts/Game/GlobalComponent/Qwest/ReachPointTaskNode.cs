using System;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class ReachPointTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			ReachPointTask reachPointTask = new ReachPointTask
			{
				PointPosition = ((!(this.PointPosition == null)) ? this.PointPosition.position : base.transform.position),
				AdditionalPointRadius = this.AdditionalPointRadius
			};
			base.ToPoBase(reachPointTask);
			return reachPointTask;
		}

		private void OnDrawGizmos()
		{
			if (this.IsDebug)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
				Gizmos.DrawSphere((!(this.PointPosition == null)) ? this.PointPosition.position : base.transform.position, 2f);
			}
		}

		[Separator("Specific")]
		[Space]
		[Header("Will use its own position if null")]
		public Transform PointPosition;

		public float AdditionalPointRadius;
	}
}
