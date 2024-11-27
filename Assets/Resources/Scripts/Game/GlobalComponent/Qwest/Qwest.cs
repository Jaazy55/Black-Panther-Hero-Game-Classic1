using System;
using System.Collections.Generic;
using Game.DialogSystem;
using Game.MiniMap;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class Qwest
	{
		public Qwest()
		{
		}

		public Qwest(BaseTask[] tasksList)
		{
			this.TasksList = tasksList;
			this.currentTask = tasksList[0];
		}

		public bool IsTimeQwest
		{
			get
			{
				return this.TimerValue > 0f;
			}
		}

		public void Init()
		{
			this.StartSelectedDialog(this.StartDialog);
			foreach (BaseTask baseTask in this.TasksList)
			{
				baseTask.Init(this);
			}
			if (this.TimerValue > 0f)
			{
				QwestTimerManager.Instance.StartCountdown(this.TimerValue, this);
			}
			InGameLogManager.Instance.RegisterNewMessage(MessageType.QuestStart, this.QwestTitle);
			this.currentTask = this.TasksList[0];
			this.currentTask.TaskStart();
		}

		public void MoveToTask(BaseTask task)
		{
			this.currentTask.Finished();
			if (task == null)
			{
				this.StartSelectedDialog(this.EndDialog);
				InGameLogManager.Instance.RegisterNewMessage(MessageType.QuestFinished, this.QwestTitle);
				if (this.TimerValue > 0f)
				{
					QwestTimerManager.Instance.EndCountdown();
				}
				this.Rewards.GiveReward();
				if (this.ShowQwestCompletePanel)
				{
					QwestCompletePanel.Instance.ShowCompletedQwestInfo("Mission complete", this.Rewards);
				}
				GameEventManager.Instance.QwestResolved(this);
			}
			else
			{
				this.currentTask = task;
				if (this.TimerValue > 0f && task.AdditionalTimer > 0f)
				{
					QwestTimerManager.Instance.AddAdditionalTime(task.AdditionalTimer);
					InGameLogManager.Instance.RegisterNewMessage(MessageType.AddQuestTime, ((int)task.AdditionalTimer).ToString());
				}
				task.TaskStart();
				GameEventManager.Instance.RefreshQwestArrow();
			}
		}

		public BaseTask GetCurrentTask()
		{
			return this.currentTask;
		}

		public string GetQwestStatus()
		{
			return this.QwestTitle + ": " + this.currentTask.TaskStatus();
		}

		public Transform GetQwestTarget()
		{
			if (this.currentTask != null)
			{
				return this.currentTask.TaskTarget();
			}
			return null;
		}

		private void StartSelectedDialog(string dialog)
		{
			if (!string.IsNullOrEmpty(dialog) && dialog.Length > Qwest.MinCharDialogLength)
			{
				DialogManager.Instance.StartDialog(dialog);
			}
		}

		public static int MinCharDialogLength = 2;

		public string Name;

		public string QwestTitle;

		public UniversalReward Rewards;

		public bool ShowQwestCompletePanel;

		public float TimerValue;

		public bool RepeatableQuest;

		public int MMMarkId;

		public MarkForMiniMap MarkForMiniMap;

		public BaseTask[] TasksList;

		public Qwest ParentQwest;

		public List<Qwest> QwestTree = new List<Qwest>();

		public Vector3 StartPosition;

		public float AdditionalStartPointRadius;

		public string StartDialog;

		public string EndDialog;

		private BaseTask currentTask;
	}
}
