using System;
using Game.GlobalComponent.Qwest;
using UnityEngine;

namespace Game.GameAchievements
{
	public class AchievmentsViewController : MonoBehaviour
	{
		private void OnEnable()
		{
			this.achievHolder.DestroyChildrens();
			if (this.GameEventManagerPrefab != null)
			{
				Achievement[] componentsInChildren = this.GameEventManagerPrefab.GetComponentsInChildren<Achievement>();
				foreach (Achievement achievment in componentsInChildren)
				{
					AchievementPanelItem achievementPanelItem = UnityEngine.Object.Instantiate<AchievementPanelItem>(this.AchievPanelItem, this.achievHolder, false);
					achievementPanelItem.InitInfo(this.GenerateInfo(achievment));
				}
			}
		}

		private AchievmentInfo GenerateInfo(Achievement achievment)
		{
			Achievement.SaveLoadAchievmentStruct defaultValue = new Achievement.SaveLoadAchievmentStruct(false, 0, 0);
			AchievmentInfo achievmentInfo = new AchievmentInfo
			{
				id = achievment.achievementName,
				description = achievment.achievementDiscription,
				icon = achievment.achievementPicture,
				showFullInfo = false
			};
			if (BaseProfile.HasKey(achievment.achievementName))
			{
				defaultValue = BaseProfile.ResolveValue<Achievement.SaveLoadAchievmentStruct>(achievment.achievementName, defaultValue);
				achievmentInfo.showFullInfo = true;
				achievmentInfo.isDone = defaultValue.isDone;
				achievmentInfo.currentProgress = defaultValue.achiveCounter;
				achievmentInfo.targetValue = defaultValue.achiveTarget;
			}
			return achievmentInfo;
		}

		private void OnDisable()
		{
		}

		public RectTransform achievHolder;

		public AchievementPanelItem AchievPanelItem;

		public GameEventManager GameEventManagerPrefab;
	}
}
