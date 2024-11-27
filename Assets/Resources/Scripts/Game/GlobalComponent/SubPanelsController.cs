using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class SubPanelsController : ControlPanel
	{
		public override void OnOpen()
		{
		}

		public void OnOpen(int index)
		{
			if (index == this.currentPanelIndex || this.ControlPanels.Length <= index)
			{
				return;
			}
			GameObject gameObject = this.ControlPanels[this.currentPanelIndex];
			gameObject.SetActive(false);
			this.CheckControlPanel(gameObject, false);
			GameObject gameObject2 = this.ControlPanels[index];
			gameObject2.SetActive(true);
			this.CheckControlPanel(gameObject2, true);
			this.currentPanelIndex = index;
		}

		public void OnOpen(ControlsType controlsType)
		{
		}

		private void CheckControlPanel(GameObject panel, bool open = true)
		{
			ControlPanel component = panel.GetComponent<ControlPanel>();
			if (!component)
			{
				return;
			}
			if (open)
			{
				component.OnOpen();
			}
			else
			{
				component.OnClose();
			}
		}

		public GameObject[] ControlPanels;

		public int IndexForFirstPanel;

		private int currentPanelIndex;
	}
}
