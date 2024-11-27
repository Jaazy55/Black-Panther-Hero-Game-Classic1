using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(MenuPanelManager))]
	public class ControlsPanelManager : MonoBehaviour
	{
		public static ControlsPanelManager Instance
		{
			get
			{
				if (ControlsPanelManager.instance == null)
				{
					ControlsPanelManager.instance = GameObject.Find("UI/Canvas/Game/Controls").GetComponent<ControlsPanelManager>();
				}
				return ControlsPanelManager.instance;
			}
		}

		private void Awake()
		{
			ControlsPanelManager.instance = this;
			this.SetUpPanels();
			this.activePanel = this.GetControlPanel(base.GetComponent<MenuPanelManager>());
			this.activePanel.OnOpen();
		}

		private void SetUpPanels()
		{
			this.panels.Clear();
			foreach (ControlPanel controlPanel in base.GetComponentsInChildren<ControlPanel>(true))
			{
				if (!(controlPanel is SubPanel))
				{
					this.panels.Add(controlPanel.GetPanelType(), controlPanel);
				}
			}
		}

		public void SwitchPanel(ControlsType controlsType)
		{
			ControlPanel controlPanel = this.panels[controlsType];
			if (!controlPanel || controlPanel == this.activePanel)
			{
				return;
			}
			this.activePanel.OnClose();
			Animator panelAnimator = controlPanel.GetPanelAnimator();
			if (this.panelManager == null)
			{
				this.panelManager = base.GetComponent<MenuPanelManager>();
			}
			this.panelManager.OpenPanel(panelAnimator);
			controlPanel.OnOpen();
			this.activePanel = controlPanel;
		}

		public void SwitchSubPanel(ControlsType controlsType, int subPanelIndex)
		{
			this.SwitchPanel(controlsType);
			SubPanelsController subPanelsController = this.activePanel as SubPanelsController;
			if (subPanelsController)
			{
				subPanelsController.OnOpen(subPanelIndex);
			}
		}

		public Transform GetControlPanel(ControlsType controlsType)
		{
			this.SetUpPanels();
			return this.panels[controlsType].transform;
		}

		private ControlPanel GetControlPanel(MenuPanelManager MPM)
		{
			Animator firstOpen = MPM.FirstOpen;
			ControlPanel component = firstOpen.GetComponent<ControlPanel>();
			if (component)
			{
				return component;
			}
			MenuPanelManager component2 = firstOpen.GetComponent<MenuPanelManager>();
			if (component2)
			{
				return this.GetControlPanel(component2);
			}
			UnityEngine.Debug.LogFormat(firstOpen, "Check this GameObject for having 'ControlPanel' or 'MenuPanelManager' component.", new object[0]);
			throw new ArgumentNullException("Can't find ControlPanel component or MenuPanelManager");
		}

		private static ControlsPanelManager instance;

		private ControlPanel activePanel;

		private Dictionary<ControlsType, ControlPanel> panels = new Dictionary<ControlsType, ControlPanel>();

		private MenuPanelManager panelManager;
	}
}
