using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class PerformanceDetectingManager : MonoBehaviour
	{
		private void Start()
		{
			if (BaseProfile.ResolveValue<bool>(SystemSettingsList.PerformanceDetected.ToString(), false))
			{
				return;
			}
			this.MenuPanelManager.FirstOpen.gameObject.SetActive(false);
			this.MenuPanelManager.FirstOpen = null;
			base.GetComponent<LoadSceneController>().Load();
		}

		public MenuPanelManager MenuPanelManager;
	}
}
