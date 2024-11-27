using System;
using Game.GlobalComponent;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vehicle
{
	public class HelicopterControlPanel : ControlPanel
	{
		public static HelicopterControlPanel Instance
		{
			get
			{
				if (HelicopterControlPanel.instance == null)
				{
					throw new Exception("HelicopterControlPanel is not initialized");
				}
				return HelicopterControlPanel.instance;
			}
		}

		public override void OnOpen()
		{
			this.SetComponentActiveStatus(true);
			base.Invoke("DefferedGetOutButtonActivate", 5f);
		}

		public override void OnClose()
		{
			this.SetComponentActiveStatus(false);
		}

		private void SetComponentActiveStatus(bool status)
		{
			this.AimJoystick.enabled = false;
			this.GetOutButton.SetActive(false);
			this.RotatePad.enabled = status;
			this.MoveJoystick.enabled = status;
		}

		private void Awake()
		{
			HelicopterControlPanel.instance = this;
		}

		private void DefferedGetOutButtonActivate()
		{
			this.GetOutButton.SetActive(true);
		}

		private const float GetInAnimationLength = 5f;

		private static HelicopterControlPanel instance;

		public JoyPad AimJoystick;

		public TouchPad RotatePad;

		public Joystick MoveJoystick;

		public Image ReloadIndicator;

		public GameObject GetOutButton;
	}
}
