using System;
using Game.Character;
using UnityEngine;

public class HideMe : MonoBehaviour
{
	private void Update()
	{
		if (!this.isHiden)
		{
			if (PlayerInteractionsManager.Instance.sitInVehicle)
			{
				foreach (GameObject gameObject in this.Models)
				{
					gameObject.SetActive(false);
					this.isHiden = true;
				}
			}
		}
		else if (!PlayerInteractionsManager.Instance.sitInVehicle)
		{
			foreach (GameObject gameObject2 in this.Models)
			{
				gameObject2.SetActive(true);
				this.isHiden = false;
			}
		}
	}

	public GameObject[] Models;

	private bool isHiden;
}
