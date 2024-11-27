using System;
using Game.GlobalComponent.HelpfulAds;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class FreeMoneyManager : MonoBehaviour
	{
		public float UpdateTime
		{
			get
			{
				return HelpfullAdsManager.Instance.HelpTimerLength;
			}
		}

		protected virtual void Awake()
		{
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 1f);
		}

		protected void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		public virtual void ButtonClick()
		{
			if (this.ByGems)
			{
				HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.FreeGems, null);
			}
			else
			{
				HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.FreeMoney, null);
			}
		}

		protected virtual void SlowUpdate()
		{
			this.MoneyButton.SetActive(HelpfullAdsManager.Instance.IsReady);
		}

		public bool ByGems;

		public GameObject MoneyButton;

		protected SlowUpdateProc slowUpdateProc;
	}
}
