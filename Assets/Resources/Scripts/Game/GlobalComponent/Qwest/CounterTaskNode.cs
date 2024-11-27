using System;
using System.Collections.Generic;

namespace Game.GlobalComponent.Qwest
{
	public class CounterTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			CounterTask counterTask = new CounterTask
			{
				TaskChecksCount = this.Counts
			};
			base.ToPoBase(counterTask);
			return counterTask;
		}

		public override void ProceedTaskLinks(BaseTask task, List<BaseTask> tasks, List<TaskNode> nodes)
		{
			base.ProceedTaskLinks(task, tasks, nodes);
			CounterTask counterTask = task as CounterTask;
			if (counterTask != null)
			{
				counterTask.ReturnTask = base.GetBaseTask(this.ReturnTask, tasks, nodes);
			}
		}

		[Separator("Specific")]
		public int Counts;

		public TaskNode ReturnTask;
	}
}
