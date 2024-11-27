using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(Button))]
	public class InputButton : MonoBehaviour
	{
		private void Update()
		{
			if (this.button == null)
			{
				this.button = base.GetComponent<Button>();
			}
			if (Controls.GetButtonDown(this.InputButtonName))
			{
				this.button.onClick.Invoke();
			}
		}

		public string InputButtonName;

		private Button button;
	}
}
