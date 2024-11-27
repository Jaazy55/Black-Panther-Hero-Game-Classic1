using System;
using Code.Managers.QuestsManager.Tasks;
using Game.GlobalComponent.Qwest;
using UnityEngine;

namespace Code.Managers.QuestsManager.QwestMaker
{
	public class RaceTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			RaceTask raceTask = new RaceTask
			{
				RaceNumber = this.raceNumber
			};
			base.ToPoBase(raceTask);
			return raceTask;
		}

		private void OnDrawGizmos()
		{
			if (this.IsDebug)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
				Gizmos.DrawSphere(base.transform.position, 2f);
			}
		}

		[SerializeField]
		private int raceNumber;
	}
}
