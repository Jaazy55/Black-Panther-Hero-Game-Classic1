using System;
using Game.GlobalComponent.HelpfulAds;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class FreeMoneyManagerImproved : FreeMoneyManager
	{
		protected override void Awake()
		{
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 1f);
			this.button = this.MoneyButton.GetComponent<Button>();
			if (this.button == null)
			{
				this.button = this.MoneyButton.GetComponentInChildren<Button>();
			}
		}

		public override void ButtonClick()
		{
			if (this.ByGems)
			{
				HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.FreeGems, null);
			}
			else
			{
				HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.FreeMoney, null);
			}
			if (this.HideOnDeactivate)
			{
				this.MoneyButton.SetActive(false);
			}
			else if (this.button != null)
			{
				this.button.interactable = false;
			}
		}

		protected override void SlowUpdate()
		{
			bool isReady = HelpfullAdsManager.Instance.IsReady;
			if (this.HideOnDeactivate)
			{
				this.MoneyButton.SetActive(isReady);
			}
			else if (this.button != null)
			{
				this.button.interactable = isReady;
			}
		}

		public bool HideOnDeactivate;

		private Button button;
	}
}
