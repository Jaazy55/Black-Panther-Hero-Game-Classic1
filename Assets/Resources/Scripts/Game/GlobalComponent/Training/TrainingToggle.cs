using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Training
{
	public class TrainingToggle : MonoBehaviour
	{
		public void ToggleAction(bool isChecked)
		{
			BaseProfile.SkipTraining = !isChecked;
			if (isChecked)
			{
				TrainingManager.ClearCompletedTrainingsInfo();
			}
			if (TrainingManager.Instance == null)
			{
				return;
			}
			TrainingManager.Instance.SetTrainingStatus(isChecked);
			TrainingManager.Instance.ClearLocalCompletedTrainingsInfo();
		}

		private void OnEnable()
		{
			if (!this.toggle)
			{
				this.toggle = base.GetComponent<Toggle>();
			}
			this.toggle.isOn = !BaseProfile.SkipTraining;
		}

		private Toggle toggle;
	}
}
