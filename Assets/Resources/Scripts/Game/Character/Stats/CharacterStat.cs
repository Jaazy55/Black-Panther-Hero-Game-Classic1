using System;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character.Stats
{
	[Serializable]
	public class CharacterStat
	{
		public float RegenModifier
		{
			get
			{
				if (this.regenStore == 0f)
				{
					this.regenStore = this.RegenPerSecond * Time.fixedDeltaTime;
				}
				return this.regenStore;
			}
		}

		public void Setup()
		{
			this.Setup(this.Max, this.Max);
		}

		public void Setup(float current, float max)
		{
			if (this.Max != max && max > 0f)
			{
				this.Max = max;
			}
			if (this.Current != current)
			{
				this.Current = current;
			}
			if (this.StatDisplay != null)
			{
				this.StatDisplay.Setup(this.Max, this.Current);
			}
		}

		public void DoFixedUpdate()
		{
			if (this.RegenModifier == 0f)
			{
				return;
			}
			this.InitSlowUpdateProc();
			if (this.Current >= this.Max)
			{
				this.Current = this.Max;
				return;
			}
			this.Current += this.RegenModifier;
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		public void Change(float amount)
		{
			this.Current += amount;
			if (this.Current > this.Max)
			{
				this.Current = this.Max;
			}
			if (this.Current < 0f)
			{
				this.Current = 0f;
			}
			if (this.StatDisplay != null)
			{
				this.StatDisplay.SetCurrent(this.Current);
				this.StatDisplay.OnChanged(amount);
			}
		}

		public void SetAmount(float amount)
		{
			this.Current += amount;
			if (this.Current > this.Max)
			{
				this.Current = this.Max;
			}
			if (this.Current < 0f)
			{
				this.Current = 0f;
			}
			if (this.StatDisplay != null)
			{
				this.StatDisplay.SetCurrent(this.Current);
			}
		}

		public void Set(float value)
		{
			this.Current = value;
			if (this.Current > this.Max)
			{
				this.Current = this.Max;
			}
			if (this.Current < 0f)
			{
				this.Current = 0f;
			}
			if (this.StatDisplay != null)
			{
				this.StatDisplay.SetCurrent(this.Current);
			}
		}

		private void InitSlowUpdateProc()
		{
			if (this.slowUpdateProc != null)
			{
				return;
			}
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 1f);
		}

		private void SlowUpdate()
		{
			if (this.StatDisplay != null)
			{
				this.StatDisplay.SetCurrent(this.Current);
			}
		}

		private const int SlowUpdatePeriod = 1;

		public string Name;

		public float Max = 100f;

		public float Current;

		public float RegenPerSecond;

		private float regenStore;

		private SlowUpdateProc slowUpdateProc;

		public CharacterStatDisplay StatDisplay;
	}
}
