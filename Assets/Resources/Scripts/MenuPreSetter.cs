using System;
using UnityEngine;

public class MenuPreSetter : MonoBehaviour
{
	private void Awake()
	{
		Time.timeScale = 1f;
		if (MApplication.MenuState == Constants.MenuState.Levels)
		{
			this.MenuPanelManager.FirstOpen = this.LevelsPanel;
			MApplication.MenuState = Constants.MenuState.None;
		}
	}

	public MenuPanelManager MenuPanelManager;

	public Animator LevelsPanel;
}
