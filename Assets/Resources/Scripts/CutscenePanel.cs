using System;
using UnityEngine;

public class CutscenePanel : MonoBehaviour
{
	private void Awake()
	{
		CutscenePanel.Instance = this;
	}

	public void Open()
	{
		this.MenuManager.OpenPanel(this.CutsceneAnimator);
	}

	public void Close()
	{
		this.MenuManager.OpenPanel(this.mainAnimator);
	}

	public static CutscenePanel Instance;

	public Animator CutsceneAnimator;

	public Animator mainAnimator;

	public MenuPanelManager MenuManager;
}
