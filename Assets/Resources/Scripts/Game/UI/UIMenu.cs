using System;
using UnityEngine;

namespace Game.UI
{
	public class UIMenu : MonoBehaviour
	{
		public void StartNewGame()
		{
            //PlayerPrefs.DeleteAll();
            UniversalYesNoPanel.Instance.DisplayOffer("New Game", "Start a new game? All progress will be lost.", delegate()
			{
				this.PanelManager.ResetSaves();
				this.PanelManager.OpenPanel(this.LoadingPanel);
				this.SceneController.Load();
				this.Background1.SetActive(false);
			}, null, false);
		}

		public void CloseApplication()
		{
			UniversalYesNoPanel.Instance.DisplayOffer("Are you sure you want to quit?", delegate()
			{
				this.PanelManager.ExitApplication();
			});
		}

		public MenuPanelManager PanelManager;

		public LoadSceneController SceneController;

		public Animator LoadingPanel;

		public GameObject Background1;
	}
}
