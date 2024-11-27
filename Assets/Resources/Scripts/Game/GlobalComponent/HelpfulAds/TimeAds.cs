using System;
using Game.GlobalComponent.Qwest;
using UnityEngine;

namespace Game.GlobalComponent.HelpfulAds
{
	public class TimeAds : HelpfulAds
	{
		public override HelpfullAdsType HelpType()
		{
			return HelpfullAdsType.Time;
		}

		public override void HelpAccepted()
		{
			base.HelpAccepted();
			QwestTimerManager.Instance.AddAdditionalTimeOfProcentMaxTime(this.ProcentTime);
		}

		[Range(0f, 1f)]
		public float ProcentTime = 0.25f;
	}
}
