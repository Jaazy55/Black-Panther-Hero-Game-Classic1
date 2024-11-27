using System;
using UnityEngine;

namespace Game.Character.Stats
{
	public abstract class CharacterStatDisplay : MonoBehaviour
	{
		protected virtual void Awake()
		{
			if (this.DoSlowUpdate)
			{
				base.InvokeRepeating("SlowUpdate", this.SlowUpdateRate, this.SlowUpdateRate);
			}
		}

		protected virtual void Start()
		{
		}

		protected virtual void OnEnable()
		{
		}

		public void Setup(float max, float current)
		{
			this.SetMax(max);
			this.SetCurrent(current);
		}

		public void SetCurrent(float value)
		{
			if (this.max > 0f)
			{
				this.current = Mathf.Clamp(value, 0f, this.max);
			}
			else
			{
				this.current = value;
			}
			this.SlowUpdate();
		}

		public void SetMax(float value)
		{
			if (value > 0f)
			{
				this.max = value;
			}
		}

		protected void SlowUpdate()
		{
			this.UpdateDisplayValue();
		}

		protected abstract void UpdateDisplayValue();

		public abstract void OnChanged(float amount);

		protected float current;

		protected float max = 100f;

		public bool DoSlowUpdate;

		public float SlowUpdateRate = 0.5f;
	}
}
