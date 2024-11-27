using System;
using Game.Managers;
using UnityEngine;

namespace Game.Character.Stats
{
	public class UpgradeManager : MonoBehaviour
	{
		public static UpgradeManager Instance
		{
			get
			{
				if (UpgradeManager.instance == null)
				{
					UpgradeManager.instance = UnityEngine.Object.FindObjectOfType<UpgradeManager>();
				}
				return UpgradeManager.instance;
			}
		}

		private void Awake()
		{
			if (UpgradeManager.instance == null)
			{
				UpgradeManager.instance = this;
			}
		}

		public void SwitchBackground(PanelsList panel)
		{
			foreach (UpgradePanel upgradePanel in this.Panels)
			{
				if (upgradePanel.type == panel)
				{
					upgradePanel.OnState.SetActive(true);
					upgradePanel.OffState.SetActive(false);
				}
				else
				{
					upgradePanel.OnState.SetActive(false);
					upgradePanel.OffState.SetActive(true);
				}
			}
		}

		public void UpgradeStat(UpgradeElement element)
		{
			this.Upgrade(element);
		}

		private void Upgrade(UpgradeElement element)
		{
			if (PlayerInfoManager.UpgradePoints >= 1 && this.EnoughLevelForStatUping(element))
			{
				int num = StatsManager.GetStat(element.stat);
				if (num < 5)
				{
					num++;
					PlayerInfoManager.UpgradePoints--;
					StatsManager.SaveStat(element.stat, num);
					element.progressBar.value = (float)num;
					PlayerInteractionsManager.Instance.Player.UpdateStats();
				}
				else if (GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log("max lvl for this stat");
				}
			}
			else if (GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("Not enught upgrade points or not Enough Level For Stat Uping");
			}
		}

		public bool EnoughLevelForStatUping(UpgradeElement element)
		{
			return this.LevelPerStatLevel.Length == 0 || PlayerInfoManager.Level >= this.LevelPerStatLevel[this.LevelPerStatLevel.Length - 1] || this.LevelPerStatLevel[StatsManager.GetStat(element.stat)] <= PlayerInfoManager.Level;
		}

		public UpgradePanel[] Panels;

		public int[] LevelPerStatLevel;

		private static UpgradeManager instance;
	}
}
