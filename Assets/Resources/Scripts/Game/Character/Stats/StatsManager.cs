using System;
using System.Collections.Generic;

namespace Game.Character.Stats
{
	[Serializable]
	public class StatsManager
	{
		public static void SaveStat(StatsList name, int value)
		{
			BaseProfile.StoreValue<int>(value, name.ToString());
			if (StatsManager.Instance != null)
			{
				StatsManager.Instance.Init();
			}
		}

		public static int GetStat(StatsList name)
		{
			return BaseProfile.ResolveValue<int>(name.ToString(), 0);
		}

		public void Init()
		{
			if (StatsManager.Instance == null)
			{
				StatsManager.Instance = this;
			}
			this.UpdateSpentPoints();
			this.UpdateStats();
		}

		public float GetPlayerStat(StatsList stat)
		{
			float result = 0f;
			for (int i = 0; i < this.upgradeValues.Count; i++)
			{
				StatsMas statsMas = this.upgradeValues[i];
				if (statsMas.stat == stat)
				{
					result = statsMas.ActualValue;
				}
			}
			return result;
		}

		public void UpdateStats()
		{
			float playerStat = this.GetPlayerStat(StatsList.Stamina);
			float regenPerSecond = playerStat / (float)this.TimeForFullStaminaRegeneration;
			this.stamina.Setup(playerStat, playerStat);
			this.stamina.RegenPerSecond = regenPerSecond;
		}

		private void UpdateSpentPoints()
		{
			foreach (StatsMas statsMas in this.upgradeValues)
			{
				statsMas.SpentPoints = StatsManager.GetStat(statsMas.stat);
			}
		}

		public static StatsManager Instance;

		public int TimeForFullStaminaRegeneration = 60;

		public CharacterStat stamina = new CharacterStat();

		public List<StatsMas> upgradeValues = new List<StatsMas>();
	}
}
