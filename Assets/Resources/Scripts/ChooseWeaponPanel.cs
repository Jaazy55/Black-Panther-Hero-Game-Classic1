using System;
using Game.Character;
using Game.UI;
using UnityEngine;

public class ChooseWeaponPanel : MonoBehaviour
{
	public void OpenPanel()
	{
		if (PlayerInteractionsManager.Instance.inVehicle)
		{
			return;
		}
		base.gameObject.SetActive(true);
		UIGame.Instance.Pause();
	}

	public void ClosePanel()
	{
		base.gameObject.SetActive(false);
		UIGame.Instance.Resume();
	}
}
