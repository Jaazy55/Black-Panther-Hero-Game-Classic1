using System;
using Game.Character;
using UnityEngine;

namespace Game.UI
{
	public class LevelUpedButton : MonoBehaviour
	{
		private void Awake()
		{
			this.Activator(PlayerInfoManager.UpgradePoints);
			PlayerInfoManager.Instance.AddOnValueChangedEvent(PlayerInfoType.UpgradePoints, new PlayerInfoManager.OnInfoValueChanged(this.Activator));
		}

		private void Activator(int upgradePoints)
		{
			base.gameObject.SetActive(upgradePoints > 0);
		}
	}
}
