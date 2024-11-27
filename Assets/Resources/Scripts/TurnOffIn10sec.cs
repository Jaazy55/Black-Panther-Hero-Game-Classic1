using System;
using UnityEngine;

public class TurnOffIn10sec : MonoBehaviour
{
	private void OnEnable()
	{
		this.isTimerOn = true;
		this.timer = this.timeDelay;
	}

	private void Update()
	{
		if (this.isTimerOn)
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.timer = this.timeDelay;
				this.isTimerOn = false;
				base.gameObject.SetActive(false);
			}
		}
	}

	public float timeDelay = 5f;

	private bool isTimerOn;

	private float timer;
}
