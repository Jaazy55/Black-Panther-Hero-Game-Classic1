using System;
using System.Collections.Generic;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using UnityEngine;

namespace Game.GameAchievements
{
	public class AchievmentPanel : MonoBehaviour
	{
		private void OnEnable()
		{
			foreach (Achievement achievement in GameEventManager.Instance.allAchievements)
			{
				AchievementPanelItem fromPool = PoolManager.Instance.GetFromPool<AchievementPanelItem>(this.AchievPanelItem);
				fromPool.transform.parent = this.achievHolder;
				fromPool.transform.localScale = Vector3.one;
				fromPool.Init(achievement);
				this.achievItems.Add(fromPool);
			}
		}

		private void OnDisable()
		{
			foreach (AchievementPanelItem o in this.achievItems)
			{
				PoolManager.Instance.ReturnToPool(o);
			}
			this.achievItems.Clear();
		}

		public RectTransform achievHolder;

		public AchievementPanelItem AchievPanelItem;

		private readonly List<AchievementPanelItem> achievItems = new List<AchievementPanelItem>();
	}
}
