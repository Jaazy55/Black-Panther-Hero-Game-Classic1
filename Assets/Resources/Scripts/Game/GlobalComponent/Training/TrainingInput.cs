using System;
using UnityEngine;

namespace Game.GlobalComponent.Training
{
	public class TrainingInput : TrainingBase
	{
		public override void ClickOnPanel()
		{
		}

		public override string GetContinueMessage()
		{
			return "Try it yourself to continue";
		}

		public override void StartTraining()
		{
		}

		public override void EndTraining()
		{
			foreach (TrainingInput.TestAxis testAxis in this.TestableAxis)
			{
				testAxis.CurrentHoldTime = 0f;
				testAxis.TestComplete = false;
			}
			TrainingManager.Instance.TrainingEnd();
		}

		private void Update()
		{
			foreach (TrainingInput.TestAxis testAxis in this.TestableAxis)
			{
				if (!testAxis.TestComplete)
				{
					if (this.CheckUsedAxis(testAxis.AxisName, testAxis.CanBeNegative, testAxis.IsButton))
					{
						testAxis.CurrentHoldTime += Time.deltaTime;
						if (testAxis.CurrentHoldTime >= testAxis.HoldTime)
						{
							testAxis.TestComplete = true;
						}
					}
				}
			}
			bool flag = true;
			foreach (TrainingInput.TestAxis testAxis2 in this.TestableAxis)
			{
				if (!testAxis2.TestComplete)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this.EndTraining();
			}
		}

		private bool CheckUsedAxis(string axisName, bool canBeNegative, bool isButton)
		{
			if (!isButton)
			{
				float num = Controls.GetAxis(axisName);
				if (canBeNegative)
				{
					num = Mathf.Abs(num);
				}
				return num > 0f;
			}
			return Controls.GetButton(axisName);
		}

		public TrainingInput.TestAxis[] TestableAxis;

		[Serializable]
		public class TestAxis
		{
			public string AxisName;

			public float HoldTime = 2f;

			public bool CanBeNegative;

			public bool IsButton;

			[HideInInspector]
			public float CurrentHoldTime;

			[HideInInspector]
			public bool TestComplete;
		}
	}
}
