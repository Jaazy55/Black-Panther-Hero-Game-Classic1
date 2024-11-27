using System;
using UnityEngine;

namespace Game.GlobalComponent.Training
{
	public class TrainingObjectTracker : MonoBehaviour
	{
		public void TrackedObjectActivate()
		{
			TrainingManager.Instance.TrackedObjectActivated(this);
		}

		private void OnEnable()
		{
			this.TrackedObjectActivate();
		}

		private void OnDestroy()
		{
			TrainingManager.Instance.TrackedObjectDestroyed(this);
		}
	}
}
