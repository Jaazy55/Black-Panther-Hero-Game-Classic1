using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Manager : MonoBehaviour
{
	public void Start()
	{
		//PlayerPrefs.DeleteAll();
	}

	public void GemFiveHundred()
	{
		Game.Character.PlayerInfoManager.Gems = Game.Character.PlayerInfoManager.Gems + 500;
	}
	public void GemFifteenHundred()
	{
		Game.Character.PlayerInfoManager.Gems = Game.Character.PlayerInfoManager.Gems + 1500;
	}
	public void GemThreeThousand()
	{
		Game.Character.PlayerInfoManager.Gems = Game.Character.PlayerInfoManager.Gems + 3000;
	}
	public void GemThirtyFiveThousand()
	{
		Game.Character.PlayerInfoManager.Gems = Game.Character.PlayerInfoManager.Gems + 5000;
	}
	public void UnlockEveryThing()
	{
		InApp_Manager.instance.Buy_UnlockAll_Game();
		//PreferenceManager.SetAdsStatus(1);
		//if (CapricornAdsManager.instance)
		//	CapricornAdsManager.instance.HideLargeAdmobBanner();
		//	CapricornAdsManager.instance.HideSmallAdmobBanner();
		//Game.Character.PlayerInfoManager.Gems = Game.Character.PlayerInfoManager.Gems + 5000;
		//PlayerPrefs.SetInt("RemoveAds", 1);
		//if (AdsManager.Instance)
		//{
		//	AdsManager.Instance.HideBannerAd();
		//	AdsManager.Instance.HideMediumRectangleAd();
		//	AdsManager.Instance.DestroyInterstitialAd();
		//}
		//Game.Character.PlayerInfoManager.Gems = +50000;
		//Game.Character.PlayerInfoManager.Money = +50000;

	}
	public void RemoveAds()
	{
		InApp_Manager.instance.Buy_UnlockAll_Removeads();
		//PlayerPrefs.SetInt("RemoveAds", 1);
		//if (AdsManager.Instance)
		//{
		//	AdsManager.Instance.HideBannerAd();
		//	AdsManager.Instance.HideMediumRectangleAd();
		//	AdsManager.Instance.DestroyInterstitialAd();
		//}

	}
}
