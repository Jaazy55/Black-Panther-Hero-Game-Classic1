using System;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class ReachPointTask : BaseTask
	{
		public override void TaskStart()
		{
			base.TaskStart();
			this.point = PoolManager.Instance.GetFromPool<QwestPoint>(GameEventManager.Instance.QwestPointPrefab.gameObject);
			PoolManager.Instance.AddBeforeReturnEvent(this.point, delegate(GameObject poolingObject)
			{
				this.Target = null;
				if (this.point != null)
				{
					this.point.Task = null;
					this.point = null;
				}
			});
			this.point.Task = this;
			this.point.transform.parent = GameEventManager.Instance.transform;
			this.point.transform.position = this.PointPosition;
			this.point.transform.localScale = new Vector3(1f + this.AdditionalPointRadius, 1f + this.AdditionalPointRadius, 1f + this.AdditionalPointRadius);
			this.Target = this.point.transform;
		}

		public override void Finished()
		{
			if (this.point != null)
			{
				PoolManager.Instance.ReturnToPool(this.point);
			}
			base.Finished();
		}

		public override void PointReachedEvent(Vector3 position, BaseTask task)
		{
			if (this.Equals(task))
			{
				this.point = null;
				this.CurrentQwest.MoveToTask(this.NextTask);
			}
		}

		public Vector3 PointPosition;

		public float AdditionalPointRadius;

		private QwestPoint point;
	}
}
