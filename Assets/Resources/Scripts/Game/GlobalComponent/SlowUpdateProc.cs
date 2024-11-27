using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class SlowUpdateProc
	{
		public SlowUpdateProc(SlowUpdateProc.SlowUpdateDelegate slowUpdate, float updateTime)
		{
			this.slowUpdate = slowUpdate;
			this.updateTime = updateTime;
		}

		public void ProceedOnFixedUpdate()
		{
			if (this.updateCurrentTime <= 0f)
			{
				this.slowUpdate();
				this.updateCurrentTime += this.updateTime;
			}
			this.updateCurrentTime -= Time.deltaTime;
		}

		public float DeltaTime
		{
			get
			{
				return this.updateTime - this.updateCurrentTime;
			}
		}

		public float UpdateTime
		{
			get
			{
				return this.updateTime;
			}
			set
			{
				this.updateTime = value;
			}
		}

		private readonly SlowUpdateProc.SlowUpdateDelegate slowUpdate;

		private float updateTime;

		private float updateCurrentTime;

		public delegate void SlowUpdateDelegate();
	}
}
