using System;
using Game.GlobalComponent.HelpfulAds;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Qwest
{
	public class QwestTimerManager : MonoBehaviour
	{
		public static QwestTimerManager Instance
		{
			get
			{
				if (QwestTimerManager.instance == null)
				{
					throw new Exception("Qwest Timer Manager not found!");
				}
				return QwestTimerManager.instance;
			}
		}

		public void StartCountdown(float timerValue, Qwest qwest)
		{
			this.countdown = true;
			this.maxTimerValue = timerValue;
			this.currentTimerValue = timerValue;
			this.currentQwest = qwest;
			this.UpdateDisplayedTime();
			this.TimerText.gameObject.SetActive(true);
		}

		public void AddAdditionalTime(float value)
		{
			this.currentTimerValue += value;
		}

		public void AddAdditionalTimeOfProcentMaxTime(float procent)
		{
			this.currentTimerValue += this.maxTimerValue * procent;
			InGameLogManager.Instance.RegisterNewMessage(MessageType.AddQuestTime, ((int)(this.maxTimerValue * procent)).ToString());
		}

		public void EndCountdown()
		{
			this.countdown = false;
			this.TimerText.text = string.Empty;
			this.displayedMin = 0f;
			this.displayedSec = 0f;
			this.currentTimerValue = 0f;
			this.currentQwest = null;
			this.TimerText.gameObject.SetActive(false);
		}

		public void QwestCanceled(Qwest qwest)
		{
			if (qwest != this.currentQwest)
			{
				return;
			}
			this.EndCountdown();
		}

		private void Awake()
		{
			QwestTimerManager.instance = this;
		}

		private void FixedUpdate()
		{
			if (!this.countdown)
			{
				return;
			}
			if (this.currentTimerValue > 0f)
			{
				this.currentTimerValue -= Time.deltaTime;
				if (this.currentTimerValue < 5f && HelpfullAdsManager.Instance != null)
				{
					HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Time, null);
				}
				this.UpdateDisplayedTime();
			}
			else
			{
				GameEventManager.Instance.QwestFailed(this.currentQwest);
				this.EndCountdown();
			}
		}

		private void UpdateDisplayedTime()
		{
			float num = Mathf.Floor(this.currentTimerValue / 60f);
			float num2 = Mathf.Floor(this.currentTimerValue % 60f);
			if (Math.Abs(this.displayedMin - num) > 0f || Math.Abs(this.displayedSec - num2) > 0f)
			{
				this.TimerText.text = num.ToString("00") + ":" + num2.ToString("00");
				this.displayedMin = num;
				this.displayedSec = num2;
			}
		}

		private static QwestTimerManager instance;

		public Text TimerText;

		private bool countdown;

		private float currentTimerValue;

		private float maxTimerValue;

		private Qwest currentQwest;

		private float displayedMin;

		private float displayedSec;
	}
}
