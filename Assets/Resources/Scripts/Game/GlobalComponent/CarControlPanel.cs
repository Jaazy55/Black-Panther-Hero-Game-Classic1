using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(Animator))]
	public class CarControlPanel : ControlPanel
	{
		public override void OnOpen()
		{
			foreach (Joystick joystick in base.GetComponentsInChildren<Joystick>(true))
			{
				joystick.enabled = true;
			}
			foreach (JoyPad joyPad in base.GetComponentsInChildren<JoyPad>(true))
			{
				joyPad.enabled = true;
			}
		}

		public override void OnClose()
		{
			foreach (Joystick joystick in base.GetComponentsInChildren<Joystick>(true))
			{
				joystick.enabled = false;
			}
			foreach (JoyPad joyPad in base.GetComponentsInChildren<JoyPad>(true))
			{
				joyPad.enabled = false;
			}
		}
	}
}
