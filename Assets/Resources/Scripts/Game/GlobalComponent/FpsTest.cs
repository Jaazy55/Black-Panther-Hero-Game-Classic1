using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class FpsTest : PerformanceTest
	{
		public override void Init()
		{
			this.isInit = true;
			this.timer = this.DetectingTime;
		}

		private void ChangeFps()
		{
			this.fps = 1f / Time.deltaTime;
			this.sumFps += this.fps;
			this.fpsCounter += 1f;
			this.absFps = this.sumFps / this.fpsCounter;
		}

		private void Update()
		{
			if (!this.isInit)
			{
				return;
			}
			this.Timer();
			this.ChangeFps();
		}

		private void Timer()
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.isInit = false;
				this.absFps = this.fpsCounter / this.DetectingTime;
				this.EndTesting();
			}
		}

		public override void EndTesting()
		{
			this.Result = this.absFps * (100f / this.MaxFps);
			base.CallEndTestingEvent(this.Result, this);
		}

		public float MaxFps = 58f;

		private float timer;

		private float sumFps;

		private float fpsCounter;

		private float fps;

		private float absFps;

		private bool isInit;
	}
}
