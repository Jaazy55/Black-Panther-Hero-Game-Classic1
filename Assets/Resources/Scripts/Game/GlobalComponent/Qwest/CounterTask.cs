using System;

namespace Game.GlobalComponent.Qwest
{
	public class CounterTask : BaseTask
	{
		public override void Init(Qwest qwest)
		{
			base.Init(qwest);
			this.checkedCount = 0;
		}

		public override void TaskStart()
		{
			this.checkedCount++;
			if (this.checkedCount >= this.TaskChecksCount)
			{
				this.CurrentQwest.MoveToTask(this.NextTask);
			}
			else
			{
				base.TaskStart();
				this.CurrentQwest.MoveToTask(this.ReturnTask);
			}
		}

		public int TaskChecksCount;

		public BaseTask ReturnTask;

		private int checkedCount;
	}
}
