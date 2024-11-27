using System;
using UnityEngine;

namespace UnitySampleAssets.CrossPlatformInput
{
	public class ButtonHandler : MonoBehaviour
	{
		public void SetDownState(string name)
		{
			this.downedStateName = name;
			CrossPlatformInputManager.SetButtonDown(name);
		}

		public void SetUpState(string name)
		{
			this.downedStateName = string.Empty;
			CrossPlatformInputManager.SetButtonUp(name);
		}

		public void SetAxisPositiveState(string name)
		{
			CrossPlatformInputManager.SetAxisPositive(name);
		}

		public void SetAxisNeutralState(string name)
		{
			CrossPlatformInputManager.SetAxisZero(name);
		}

		public void SetAxisNegativeState(string name)
		{
			CrossPlatformInputManager.SetAxisNegative(name);
		}

		private void OnDisable()
		{
			if (this.downedStateName.Length > 0)
			{
				this.SetUpState(this.downedStateName);
			}
		}

		private string downedStateName = string.Empty;
	}
}
