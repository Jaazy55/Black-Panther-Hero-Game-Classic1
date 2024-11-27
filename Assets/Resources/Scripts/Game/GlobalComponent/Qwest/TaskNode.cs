using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class TaskNode : MonoBehaviour
	{
		public virtual BaseTask ToPo()
		{
			return new BaseTask();
		}

		public virtual void ProceedTaskLinks(BaseTask task, List<BaseTask> tasks, List<TaskNode> nodes)
		{
			task.NextTask = this.GetBaseTask(this.NextTask, tasks, nodes);
			task.PrevTask = this.GetBaseTask(this.PrevTask, tasks, nodes);
		}

		protected BaseTask GetBaseTask(TaskNode node, List<BaseTask> tasks, List<TaskNode> nodes)
		{
			if (node == null)
			{
				return null;
			}
			for (int i = 0; i < nodes.Count; i++)
			{
				TaskNode taskNode = nodes[i];
				if (taskNode.Equals(node))
				{
					return tasks[i];
				}
			}
			throw new Exception(string.Format("TaskNode-Task corresponding failed {4}->{0}({1})-{2}({3}) ", new object[]
			{
				base.name,
				base.GetInstanceID(),
				this.NextTask.name,
				this.NextTask.GetInstanceID(),
				base.transform.parent.name
			}));
		}

		protected void ToPoBase(BaseTask task)
		{
			task.TaskText = this.TaskText;
			task.DialogData = this.DialogData;
			task.EndDialogData = this.EndDialogData;
			task.AdditionalTimer = this.AdditionalTimer;
		}

		public string TaskText;

		public TaskNode PrevTask;

		public TaskNode NextTask;

		public float AdditionalTimer;

		[TextArea]
		public string DialogData;

		[TextArea]
		public string EndDialogData;

		[Separator("Debug")]
		public bool IsDebug;
	}
}
