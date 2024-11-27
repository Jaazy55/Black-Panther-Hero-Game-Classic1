using System;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonusUIButton : MonoBehaviour
{
	public void OnClick()
	{
		DailyBonusesManager.Instance.SelectBonusUIButton(this);
	}

	public int BonusID;

	public Image Image;
}
