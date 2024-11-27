using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class BikeControlPanel : ControlPanel
	{
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

		public Joystick[] Joysticks;

		public ControlButtonManager ButtonManager;

		public GameObject GetOutButton;

		public float GetInAnimationLength;
	}
}
