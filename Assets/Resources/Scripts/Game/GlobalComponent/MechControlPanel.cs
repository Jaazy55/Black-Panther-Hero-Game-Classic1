using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class MechControlPanel : ControlPanel
	{
		public static MechControlPanel Instance
		{
			get
			{
				if (MechControlPanel.instance == null)
				{
					throw new Exception("MechControlPanel is not initialized");
				}
				return MechControlPanel.instance;
			}
		}

		private void Awake()
		{
			MechControlPanel.instance = this;
		}

		public override void OnOpen()
		{
			this.AimJoystick.enabled = false;
			this.RotatePad.enabled = true;
			this.MoveJoystick.enabled = true;
			base.Invoke("DefferedGetOutButtonActivate", this.GetInAnimationLength);
		}

		public override void OnClose()
		{
			this.AimJoystick.enabled = false;
			this.RotatePad.enabled = false;
			this.MoveJoystick.enabled = false;
			this.GetOutButton.SetActive(false);
			this.RotateLeft.SetActive(false);
			this.RotateRight.SetActive(false);
		}

		public void EnableToSideRotationButtons()
		{
			this.RotateLeft.SetActive(true);
			this.RotateRight.SetActive(true);
		}

		private void DefferedGetOutButtonActivate()
		{
			this.GetOutButton.SetActive(true);
		}

		private static MechControlPanel instance;

		public JoyPad AimJoystick;

		public TouchPad RotatePad;

		public Joystick MoveJoystick;

		public Image ReloadIndicator;

		public GameObject GetOutButton;

		public GameObject RotateRight;

		public GameObject RotateLeft;

		public bool isBigFoot;

		public float GetInAnimationLength;
	}
}
