using System;
using UnityEngine;

namespace Game.GlobalComponent.HelpfulAds
{
	public class HelpfulAds : MonoBehaviour
	{
		public virtual HelpfullAdsType HelpType()
		{
			return HelpfullAdsType.None;
		}

		public virtual void HelpAccepted()
		{
		}

		public string Message;
	}
}
