using System;
using System.Collections.Generic;
using Game.MiniMap;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class QwestNode : MonoBehaviour
	{
		public Qwest ToPo()
		{
			List<BaseTask> list = new List<BaseTask>();
			List<TaskNode> list2 = new List<TaskNode>();
			BaseTask baseTask = null;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				TaskNode component = base.transform.GetChild(i).GetComponent<TaskNode>();
				if (component != null)
				{
					BaseTask baseTask2 = component.ToPo();
					if (baseTask != null)
					{
						baseTask.NextTask = baseTask2;
						baseTask2.PrevTask = baseTask;
					}
					list.Add(baseTask2);
					list2.Add(component);
					baseTask = baseTask2;
				}
			}
			for (int j = 0; j < list2.Count; j++)
			{
				list2[j].ProceedTaskLinks(list[j], list, list2);
			}
			int mmmarkId = -1;
			if (this.MMMark != null)
			{
				mmmarkId = this.MMMark.transform.GetSiblingIndex();
			}
			return new Qwest(list.ToArray())
			{
				QwestTitle = this.QwestTitle,
				Rewards = this.Rewards,
				ShowQwestCompletePanel = this.ShowQwestCompletePanel,
				TimerValue = this.TimerValue,
				RepeatableQuest = this.RepeatableQuest,
				StartDialog = this.StartDialog,
				EndDialog = this.EndDialog,
				MMMarkId = mmmarkId,
				StartPosition = ((!(this.StartPosition == null)) ? this.StartPosition.transform.position : base.transform.position),
				AdditionalStartPointRadius = this.AdditionalPointRadius
			};
		}

		private void OnDrawGizmos()
		{
			if (this.IsDebug)
			{
				Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
				Gizmos.DrawSphere((!(this.StartPosition == null)) ? this.StartPosition.transform.position : base.transform.position, 2f);
			}
		}

		public string QwestTitle;

		public UniversalReward Rewards;

		public bool ShowQwestCompletePanel;

		[Header("If timer not used, value must be zero.")]
		public float TimerValue;

		[Header("After finishing quest, it will appear again.")]
		public bool RepeatableQuest;

		[Header("Mark from a GameEventManager.MapMarksList")]
		public MarkForMiniMap MMMark;

		[TextArea]
		public string StartDialog;

		[TextArea]
		public string EndDialog;

		[Header("Will use its own position if null")]
		public Transform StartPosition;

		public float AdditionalPointRadius;

		[Separator("Debug")]
		public bool IsDebug;
	}
}
