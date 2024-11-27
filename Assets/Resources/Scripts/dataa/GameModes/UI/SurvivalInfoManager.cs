using System;
using UnityEngine;

namespace Naxeex.GameModes.UI
{
	[CreateAssetMenu(fileName = "SurvivalInfoManager", menuName = "Game Modes/UI/Survival Info Manager")]
	public class SurvivalInfoManager : InfoManager
	{
		public override void BeginProcess(GameModeInfo infoPanel)
		{
			base.BeginProcess(infoPanel);
			this.UIMinutes = -1;
			this.UISeconds = -1;
			if (infoPanel != null)
			{
				infoPanel.TextInfo.text = string.Empty;
				infoPanel.TextInfo.gameObject.SetActive(true);
				infoPanel.SliderInfo.value = ZombiePosionTimer.Value / ZombiePosionTimer.ActivateValue;
				infoPanel.SliderInfo.gameObject.SetActive(true);
				infoPanel.SliderText.text = "Zombie Posion";
				infoPanel.SliderText.gameObject.SetActive(true);
			}
		}

		public override void UpdateProcess(GameModeInfo infoPanel)
		{
			base.UpdateProcess(infoPanel);
			int currentTimeToInt = SurvivalTimer.CurrentTimeToInt;
			int num = currentTimeToInt / 60;
			int num2 = currentTimeToInt % 60;
			if (this.UISeconds != num2 || this.UIMinutes != num)
			{
				this.UIMinutes = num;
				this.UISeconds = num2;
				if (infoPanel != null)
				{
					if (num > 0)
					{
						infoPanel.TextInfo.text = string.Concat(new object[]
						{
							this.UIMinutes,
							" M ",
							this.UISeconds,
							" S "
						});
					}
					else
					{
						infoPanel.TextInfo.text = this.UISeconds + " S ";
					}
				}
			}
			if (ZombiePosionTimer.Value > 0f)
			{
				if (!ZombiePosionTimer.Activate)
				{
					infoPanel.SliderInfo.value = ZombiePosionTimer.Value / ZombiePosionTimer.ActivateValue;
				}
				else if (infoPanel.SliderInfo.value != 1f)
				{
					infoPanel.SliderInfo.value = 1f;
				}
			}
		}

		public override void EndProcess(GameModeInfo infoPanel)
		{
			base.EndProcess(infoPanel);
			if (infoPanel != null)
			{
				infoPanel.TextInfo.gameObject.SetActive(false);
				infoPanel.SliderInfo.gameObject.SetActive(false);
				infoPanel.SliderText.gameObject.SetActive(false);
			}
		}

		private const string MinutesString = " M ";

		private const string SecondsString = " S ";

		private const string SliderTextTitle = "Zombie Posion";

		private int UIMinutes = -1;

		private int UISeconds = -1;
	}
}
