using System;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class EnterArenaTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			EnterArenaTask enterArenaTask = new EnterArenaTask
			{
				HighlightPoint = ((!(this.HighlightPoint == null)) ? this.HighlightPoint.position : base.transform.position),
				AdditionalPointRadius = this.AdditionalPointRadius
			};
			base.ToPoBase(enterArenaTask);
			return enterArenaTask;
		}

		private void OnDrawGizmos()
		{
			if (this.IsDebug)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
				Gizmos.DrawSphere((!(this.HighlightPoint == null)) ? this.HighlightPoint.position : base.transform.position, 2f);
			}
		}

		public Transform HighlightPoint;

		public float AdditionalPointRadius;
	}
}
