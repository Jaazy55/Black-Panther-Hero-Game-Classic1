using System;
using System.Collections.Generic;
using UnityEngine;

public class BonusInfoPanelManager : MonoBehaviour
{
	public static BonusInfoPanelManager Instance
	{
		get
		{
			if (BonusInfoPanelManager.instance == null)
			{
				BonusInfoPanelManager.instance = UnityEngine.Object.FindObjectOfType<BonusInfoPanelManager>();
			}
			return BonusInfoPanelManager.instance;
		}
	}

	public void ShowInfo(Dictionary<string, Sprite> output, bool isAvailable)
	{
		int childCount = this.ContentContainer.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			UnityEngine.Object.Destroy(this.ContentContainer.transform.GetChild(i));
		}
	}

	private static BonusInfoPanelManager instance;

	public GameObject InfoPrimitivePrefab;

	public GameObject GetBonusButton;

	public GameObject ContentContainer;
}
