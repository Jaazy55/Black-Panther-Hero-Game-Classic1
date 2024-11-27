using System;
using Game.Character;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class TeleportPlayerToPositionTask : BaseTask
	{
		public override void TaskStart()
		{
			base.TaskStart();
			if (PlayerInteractionsManager.Instance != null && PlayerInteractionsManager.Instance.TeleportPlayerToPosition(this.targetPosition, this.targetRotarion))
			{
				this.CurrentQwest.MoveToTask(this.NextTask);
			}
		}

		public override void Finished()
		{
			base.Finished();
		}

		public Vector3 targetPosition;

		public Quaternion targetRotarion;
	}
}
