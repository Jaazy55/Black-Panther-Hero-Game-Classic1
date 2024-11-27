using System;
using Naxeex.GameModes;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class EnterArenaTask : BaseTask
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
			this.point.transform.position = this.HighlightPoint;
			this.point.transform.localScale = new Vector3(1f + this.AdditionalPointRadius, 1f + this.AdditionalPointRadius, 1f + this.AdditionalPointRadius);
			this.Target = this.point.transform;
			ClosedRoom.OnChangeCurrent += this.ClosedRoomHandler;
		}

		public override void Finished()
		{
			if (this.point != null)
			{
				PoolManager.Instance.ReturnToPool(this.point);
			}
			ClosedRoom.OnChangeCurrent -= this.ClosedRoomHandler;
			base.Finished();
		}

		private void ClosedRoomHandler(ClosedRoom closedRoom)
		{
			if (closedRoom != null)
			{
				this.point = null;
				ClosedRoom.OnChangeCurrent -= this.ClosedRoomHandler;
				ArenaTutorial.State = ArenaTutorial.TutorialState.ClickTutorial;
				this.CurrentQwest.MoveToTask(this.NextTask);
			}
		}

		public Vector3 HighlightPoint;

		public float AdditionalPointRadius;

		private QwestPoint point;
	}
}
