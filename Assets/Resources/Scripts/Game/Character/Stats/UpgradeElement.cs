using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.Stats
{
	public class UpgradeElement : MonoBehaviour
	{
		private void Start()
		{
			this.progressBar.value = (float)StatsManager.GetStat(this.stat);
		}

		public void Upgrade()
		{
			UpgradeManager.Instance.UpgradeStat(this);
			this.UpdateButtonImage();
		}

		public void SwitchButton()
		{
			UpgradeManager.Instance.SwitchBackground(this.panel);
		}

		public void UpdateButtonImage()
		{
			this.ButtonImage.color = ((!UpgradeManager.Instance.EnoughLevelForStatUping(this)) ? new Color(1f, 1f, 1f, 0.3f) : Color.white);
		}

		private void OnEnable()
		{
			this.UpdateButtonImage();
		}

		public PanelsList panel;

		public StatsList stat;

		public Image ButtonImage;

		public Slider progressBar;
	}
}
