using System;
using UnityEngine;
using UnityEngine.UI;

namespace Naxeex.GameModes.UI
{
	public class ChooseGameModePanel : MonoBehaviour
	{
		public void SelectGameModeButton(GameModeButton gameModeButton)
		{
			Manager.CurrentMod = gameModeButton.Mode;
			ResultTable resultTable = gameModeButton.VisualMode.ResultTable;
			this.m_ResultTableShower.ShowTable(resultTable);
			foreach (GameModeButton gameModeButton2 in this.m_GameModeButtons)
			{
				gameModeButton2.SetSelecteStatus(gameModeButton2 == gameModeButton);
				if (gameModeButton2 == gameModeButton)
				{
					this.m_Description.text = gameModeButton2.VisualMode.Description;
				}
			}
			this.lastSelected = gameModeButton;
		}

		public void StartGame()
		{
			if (!Manager.IsActivated)
			{
				Manager.Activate();
			}
			else
			{
				Manager.Restart();
			}
		}

		public void RestartGame()
		{
			Manager.Restart();
		}

		public void EndGame()
		{
			Manager.Deactivate();
		}

		protected void OnEnable()
		{
			if (this.lastSelected == null && this.m_GameModeButtons.Length > 0)
			{
				this.lastSelected = this.m_GameModeButtons[0];
			}
			this.SelectGameModeButton(this.lastSelected);
		}

		protected void Awake()
		{
			GameModeButton[] gameModeButtons = this.m_GameModeButtons;
			for (int i = 0; i < gameModeButtons.Length; i++)
			{
				GameModeButton gameModeButton = gameModeButtons[i];
				ChooseGameModePanel _0024this = this;
				gameModeButton.Button.onClick.AddListener(delegate()
				{
					_0024this.SelectGameModeButton(gameModeButton);
				});
			}
		}

		[SerializeField]
		private Text m_Description;

		[SerializeField]
		private ResultTableShower m_ResultTableShower;

		[Header("Data")]
		[SerializeField]
		private VisualGameMode[] m_VisualGameModes;

		[SerializeField]
		private GameModeButton[] m_GameModeButtons;

		private GameModeButton lastSelected;

		private ResultTable resultTable;
	}
}
