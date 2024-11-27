using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof( Text))]
public class FPSCounter : MonoBehaviour
{
	private void Start()
	{
		this.fpsNextPeriod = Time.realtimeSinceStartup + this.fpsMeasurePeriod;
	}

	private void Update()
	{
		this.fpsAccumulator++;
		if (Time.realtimeSinceStartup > this.fpsNextPeriod)
		{
			this.currentFps = (int)((float)this.fpsAccumulator / this.fpsMeasurePeriod);
			this.fpsAccumulator = 0;
			this.fpsNextPeriod += this.fpsMeasurePeriod;
			base.GetComponent<Text>().text = string.Format(this.display, this.currentFps);
		}
	}

	private float fpsMeasurePeriod = 0.5f;

	private int fpsAccumulator;

	private float fpsNextPeriod;

	private int currentFps;

	private string display = "{0} FPS";
}
