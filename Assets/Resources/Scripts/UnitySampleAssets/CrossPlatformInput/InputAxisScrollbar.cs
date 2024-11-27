using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnitySampleAssets.CrossPlatformInput
{
	public class InputAxisScrollbar : Scrollbar
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			base.onValueChanged.AddListener(new UnityAction<float>(this.HandleInput));
		}

		private void HandleInput(float arg0)
		{
			CrossPlatformInputManager.SetAxis(this.axis, base.value * 2f - 1f);
		}

		public string axis;
	}
}
