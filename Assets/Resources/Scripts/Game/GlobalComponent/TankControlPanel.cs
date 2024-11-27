using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class TankControlPanel : ControlPanel
	{
		public static TankControlPanel Instance
		{
			get
			{
				return TankControlPanel.instance;
			}
		}

		private void Awake()
		{
			TankControlPanel.instance = this;
		}

		public override void OnOpen()
		{
			for (int i = 0; i < this.Joysticks.Length; i++)
			{
				this.Joysticks[i].enabled = true;
			}
			this.ButtonManager.enabled = true;
			base.Invoke("DefferedGetOutButtonActivate", this.GetInAnimationLength);
		}

		public override void OnClose()
		{
			for (int i = 0; i < this.Joysticks.Length; i++)
			{
				this.Joysticks[i].enabled = false;
			}
			this.ButtonManager.enabled = false;
			this.GetOutButton.SetActive(false);
		}

		private void DefferedGetOutButtonActivate()
		{
			this.GetOutButton.SetActive(true);
		}

		private static TankControlPanel instance;

		public Joystick[] Joysticks;

		public ControlButtonManager ButtonManager;

		public Image ReloadIndicator;

		public GameObject GetOutButton;

		public float GetInAnimationLength;
	}
}
