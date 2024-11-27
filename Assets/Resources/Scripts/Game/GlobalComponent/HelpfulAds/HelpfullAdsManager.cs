using System;
using Game.UI;
using UnityEngine;

namespace Game.GlobalComponent.HelpfulAds
{
	public class HelpfullAdsManager : MonoBehaviour
	{
		public static HelpfullAdsManager Instance
		{
			get
			{
				if (HelpfullAdsManager.instance == null)
				{
					UnityEngine.Debug.Log("HelpfullAdsManager is not initialized");
				}
				return HelpfullAdsManager.instance;
			}
		}

		public bool IsReady
		{
			get
			{
				//return Time.time > this.lastHelpTime + this.HelpTimerLength && AdsManager.IsRewardReady();
				return false;
			}
		}

		public HelpfulAds GetAdsByType(HelpfullAdsType helpType)
		{
			if (this.helps == null || this.helps.Length == 0)
			{
				return null;
			}
			foreach (HelpfulAds helpfulAds in this.helps)
			{
				if (helpfulAds.HelpType() == helpType)
				{
					return helpfulAds;
				}
			}
			return null;
		}

		public void OfferAssistance(HelpfullAdsType helpType, Action<bool> helpCallback)
		{
			if (!this.IsReady)
			{
				if (helpCallback != null)
				{
					helpCallback(false);
				}
				return;
			}
			this.lastHelpfulAds = null;
			foreach (HelpfulAds helpfulAds in this.helps)
			{
				if (helpfulAds.HelpType() == helpType)
				{
					this.lastHelpfulAds = helpfulAds;
					break;
				}
			}
			if (this.lastHelpfulAds == null)
			{
				throw new Exception("HelpfullAds for " + helpType + " type not found!");
			}
			this.lastCallback = helpCallback;
			UniversalYesNoPanel.Instance.DisplayOffer(null, this.lastHelpfulAds.Message, delegate()
			{
				this.SelectionChoosed(true);
			}, delegate()
			{
				this.SelectionChoosed(false);
			}, false);
		}

		public void SelectionChoosed(bool accepted)
		{
			this.lastHelpTime = Time.time;
			if (!accepted)
			{
				if (this.lastCallback != null)
				{
					this.lastCallback(false);
				}
				return;
			}
//			AdsManager.ShowRewardAd(new Action<AdsResult>(this.Callback));
		}

		//private void Callback(AdsResult result)
		//{
		//	MainThreadExecuter.Instance.Run(delegate
		//	{
		//		this.CallbackResolver(result);
		//	});
		//}

		//private void CallbackResolver(AdsResult result)
		//{
		//	if (result != AdsResult.Finished)
		//	{
		//		if (this.lastCallback != null)
		//		{
		//			this.lastCallback(false);
		//		}
		//		return;
		//	}
		//	this.lastHelpfulAds.HelpAccepted();
		//	if (this.lastCallback != null)
		//	{
		//		this.lastCallback(true);
		//	}
		//}

		private void Awake()
		{
			HelpfullAdsManager.instance = this;
			this.helps = base.GetComponentsInChildren<HelpfulAds>();
			this.lastHelpTime = Time.time - this.HelpTimerLength;
		}

		private static HelpfullAdsManager instance;

		public float HelpTimerLength;

		private HelpfulAds[] helps;

		private float lastHelpTime;

		private Action<bool> lastCallback;

		private HelpfulAds lastHelpfulAds;
	}
}
