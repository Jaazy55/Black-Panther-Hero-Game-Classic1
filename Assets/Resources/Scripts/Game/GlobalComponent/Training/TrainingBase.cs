using System;
using UnityEngine;

namespace Game.GlobalComponent.Training
{
	public class TrainingBase : MonoBehaviour
	{
		public virtual string GetContinueMessage()
		{
			return "Tap anywhere to continue.";
		}

		public virtual void StartTraining()
		{
			if (!GameplayUtils.OnPause)
			{
				GameplayUtils.PauseGame();
				this.timeWasFreezed = true;
			}
		}

		public virtual void EndTraining()
		{
			if (this.timeWasFreezed)
			{
				GameplayUtils.ResumeGame();
				this.timeWasFreezed = false;
			}
			TrainingManager.Instance.TrainingEnd();
		}

		public virtual void ClickOnPanel()
		{
			this.EndTraining();
		}

		public string TrainingName;

		public RectTransform ObjectForActive;

		public string ObjectDescription;

		public float AdditionalPointerScalling;

		private bool timeWasFreezed;
	}
}
