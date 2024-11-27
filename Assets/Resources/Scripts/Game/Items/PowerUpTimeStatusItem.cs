using System;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Items
{
	public class PowerUpTimeStatusItem : MonoBehaviour
	{
		public GameItemPowerUp CurrentPowerUp { get; private set; }

		public void ShowDescription()
		{
			if (!this.ShowDescriptionOnClick)
			{
				return;
			}
			string str = string.Concat(new string[]
			{
				" (",
				this.GetRemainingMinutes().ToString("00"),
				":",
				this.GetRemainingSeconds().ToString("00"),
				")"
			});
			UIDescriptor.Instance.ShowInfo(this.myRectTransform, this.CurrentPowerUp.ShopVariables.Description + str, 3f);
		}

		public void Init(GameItemPowerUp displayedPowerUp)
		{
			this.CurrentPowerUp = displayedPowerUp;
			this.ShowedImage.sprite = displayedPowerUp.ShopVariables.ItemIcon;
			this.remainingTime = (float)displayedPowerUp.RemainingDuration;
		}

		private void Awake()
		{
			this.myRectTransform = (base.transform as RectTransform);
		}

		private void Update()
		{
			this.SyncTime();
			this.UpdateTimer();
			this.UpdateFiller();
		}

		private void SyncTime()
		{
			this.remainingTime -= Time.deltaTime;
			if (Math.Abs(this.remainingTime - (float)this.CurrentPowerUp.RemainingDuration) > 4f)
			{
				this.remainingTime = (float)this.CurrentPowerUp.RemainingDuration;
			}
		}

		private void UpdateTimer()
		{
			if (this.TimerText == null)
			{
				return;
			}
			bool flag = false;
			string text = string.Empty;
			int remainingMinutes = this.GetRemainingMinutes();
			if (remainingMinutes > 0)
			{
				if (remainingMinutes != this.lastRemMin)
				{
					text = remainingMinutes + "m";
					this.lastRemMin = remainingMinutes;
					flag = true;
				}
			}
			else
			{
				int remainingSeconds = this.GetRemainingSeconds();
				if (remainingSeconds != this.lastRemSec)
				{
					text = remainingSeconds.ToString("00");
					this.lastRemSec = remainingSeconds;
					flag = true;
				}
			}
			if (flag)
			{
				this.TimerText.text = text;
			}
		}

		private int GetRemainingMinutes()
		{
			return (int)Mathf.Floor(this.remainingTime / 60f);
		}

		private int GetRemainingSeconds()
		{
			return (int)Mathf.Clamp(Mathf.Floor(this.remainingTime % 60f), 0f, 60f);
		}

		private void UpdateFiller()
		{
			if (this.Filler == null)
			{
				return;
			}
			float fillAmount = 1f - Mathf.Clamp01(this.remainingTime / (float)this.CurrentPowerUp.Duration);
			this.Filler.fillAmount = fillAmount;
		}

		private const int MaxTimeDiff = 4;

		private const int DescriptionShowedTime = 3;

		public Image ShowedImage;

		public Image Filler;

		public Text TimerText;

		public bool ShowDescriptionOnClick;

		private RectTransform myRectTransform;

		private float remainingTime;

		private int lastRemMin;

		private int lastRemSec;
	}
}
