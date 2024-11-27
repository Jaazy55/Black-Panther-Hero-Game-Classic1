using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(Animator))]
	public class CharacterControlPanel : ControlPanel
	{
		public override void OnOpen()
		{
			this.AimJoystick.enabled = false;
			this.RotatePad.enabled = true;
			this.MoveJoystick.enabled = true;
		}

		public override void OnClose()
		{
			this.AimJoystick.enabled = false;
			this.RotatePad.enabled = false;
			this.MoveJoystick.enabled = false;
		}

		public JoyPad AimJoystick;

		public TouchPad RotatePad;

		public Joystick MoveJoystick;
	}
}
