using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameAchievements
{
	public class AchievementPanelItem : MonoBehaviour
	{
		public void Init(Achievement achievement)
		{
			int achiveTarget = achievement.achiveParams.achiveTarget;
			int achiveCounter = achievement.achiveParams.achiveCounter;
			this.StatusSlider.gameObject.SetActive(achiveTarget != 0);
			if (achiveTarget != 0)
			{
				this.StatusSlider.maxValue = (float)achiveTarget;
				this.StatusSlider.value = (float)achiveCounter;
				this.StatusSlider.interactable = false;
				this.StatusText.text = achiveCounter + " / " + achiveTarget;
			}
			this.AchievName.text = achievement.achievementName;
			this.AchievDescription.text = achievement.achievementDiscription;
			this.AchievIcon.sprite = achievement.achievementPicture;
			this.DoneContrainer.gameObject.SetActive(achievement.achiveParams.isDone);
			this.NotDoneContainer.gameObject.SetActive(!achievement.achiveParams.isDone);
		}

		public void InitInfo(AchievmentInfo achievmentInfo)
		{
			if (achievmentInfo.showFullInfo)
			{
				int targetValue = achievmentInfo.targetValue;
				int currentProgress = achievmentInfo.currentProgress;
				this.StatusSlider.gameObject.SetActive(targetValue != 0);
				if (targetValue != 0)
				{
					this.StatusSlider.maxValue = (float)targetValue;
					this.StatusSlider.value = (float)currentProgress;
					this.StatusSlider.interactable = false;
					this.StatusText.text = currentProgress + " / " + targetValue;
				}
				this.AchievName.text = achievmentInfo.id;
				this.AchievDescription.text = achievmentInfo.description;
				this.AchievIcon.sprite = achievmentInfo.icon;
				this.DoneContrainer.gameObject.SetActive(achievmentInfo.isDone);
				this.NotDoneContainer.gameObject.SetActive(!achievmentInfo.isDone);
			}
			else
			{
				this.AchievName.text = achievmentInfo.id;
				this.AchievDescription.text = achievmentInfo.description;
				this.AchievIcon.sprite = achievmentInfo.icon;
				this.DoneContrainer.gameObject.SetActive(false);
				this.NotDoneContainer.gameObject.SetActive(false);
				this.StatusSlider.gameObject.SetActive(false);
			}
		}

		public Text AchievName;

		public Text AchievDescription;

		public Image AchievIcon;

		public Slider StatusSlider;

		public Text StatusText;

		public RectTransform DoneContrainer;

		public RectTransform NotDoneContainer;
	}
}
