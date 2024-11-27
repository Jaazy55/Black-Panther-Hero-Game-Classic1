using System;
using UnityEngine;

public class WinnerStarsController : MonoBehaviour
{
	private void Awake()
	{
		foreach (GameObject gameObject in this.Stars)
		{
			gameObject.SetActive(false);
		}
	}

	public void SetStarts(int starsCount)
	{
		foreach (GameObject gameObject in this.Stars)
		{
			if (--starsCount < 0)
			{
				break;
			}
			gameObject.SetActive(true);
		}
	}

	public GameObject[] Stars;
}
