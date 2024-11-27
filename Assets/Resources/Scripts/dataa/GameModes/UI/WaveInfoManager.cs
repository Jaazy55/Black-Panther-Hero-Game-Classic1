using System;
using UnityEngine;

namespace Naxeex.GameModes.UI
{
	[CreateAssetMenu(fileName = "WaveInfoManager", menuName = "Game Modes/UI/Wave Info Manager")]
	public class WaveInfoManager : InfoManager
	{
		public override void BeginProcess(GameModeInfo infoPanel)
		{
			base.BeginProcess(infoPanel);
			this.InfoPanel = infoPanel;
			ArenaWave.OnNumberUpdate += this.UpdateWave;
			this.UpdateWave(ArenaWave.Number);
			if (infoPanel != null)
			{
				infoPanel.TextInfo.gameObject.SetActive(true);
			}
		}

		public override void EndProcess(GameModeInfo infoPanel)
		{
			base.EndProcess(infoPanel);
			this.InfoPanel = infoPanel;
			ArenaWave.OnNumberUpdate -= this.UpdateWave;
			if (infoPanel != null)
			{
				infoPanel.TextInfo.gameObject.SetActive(false);
			}
		}

		private void UpdateWave(int currentWave)
		{
			if (this.InfoPanel != null)
			{
				this.InfoPanel.TextInfo.text = (currentWave + 1).ToString() + " Wave";
			}
		}

		private GameModeInfo InfoPanel;

		private const string WaveString = " Wave";
	}
}
