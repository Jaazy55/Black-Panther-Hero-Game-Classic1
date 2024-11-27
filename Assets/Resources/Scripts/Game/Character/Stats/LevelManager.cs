using System;
using Game.GlobalComponent.Qwest;
using Game.Managers;
using UnityEngine;

namespace Game.Character.Stats
{
	public class LevelManager : MonoBehaviour
	{
		public void AddExperience(int exp, bool useVIPmult = false)
		{
			PlayerInfoManager.Instance.ChangeInfoValue(PlayerInfoType.Experience, exp, useVIPmult);
			if (PlayerInfoManager.Experience >= this.ExpForNextLevel)
			{
				this.LevelUp();
				int exp2 = PlayerInfoManager.Experience - this.ExpForNextLevel;
				this.ExpForNextLevel = this.CalculateExpForLvl(PlayerInfoManager.Level, this.ExpForFirstLevel);
				PlayerInfoManager.Experience = 0;
				this.AddExperience(exp2, false);
			}
		}

		private void Awake()
		{
			LevelManager.Instance = this;
			this.ExpForNextLevel = this.CalculateExpForLvl(PlayerInfoManager.Level, this.ExpForFirstLevel);
		}

		private int CalculateExpForLvl(int lvl, int exp)
		{
			for (;;)
			{
				int num = (int)((float)exp + (float)exp * this.PercenOfExpForNextLevel);
				if (lvl == 0)
				{
					break;
				}
				lvl--;
				exp = num;
			}
			return exp;
		}

		private void LevelUp()
		{
			PlayerInfoManager.UpgradePoints++;
			PlayerInfoManager.Level++;
			int level = PlayerInfoManager.Level;
			GameEventManager.Instance.Event.GetLevelEvent(level);
			if (this.OnLevelUpAction != null)
			{
				this.OnLevelUpAction();
			}
			if (GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("new lvl: " + level);
			}
		}

		public int ExpForFirstLevel = 1000;

		public float PercenOfExpForNextLevel = 0.1f;

		public static LevelManager Instance;

		[HideInInspector]
		public int ExpForNextLevel;

		public Action OnLevelUpAction;
	}
}
