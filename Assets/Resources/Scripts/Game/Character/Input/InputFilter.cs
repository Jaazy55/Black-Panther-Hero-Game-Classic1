using System;
using UnityEngine;

namespace Game.Character.Input
{
	public class InputFilter
	{
		public InputFilter(int samplesNum, float coef)
		{
			this.value = default(Vector2);
			this.weightCoef = coef;
			this.numSamples = samplesNum;
			this.samples = new Vector2[samplesNum];
		}

		public void AddSample(Vector2 sample)
		{
			Vector2 a = default(Vector2);
			float num = 0f;
			float num2 = 1f;
			float num3 = 1f;
			Vector2 vector = this.samples[0];
			this.samples[0] = sample;
			for (int i = 1; i < this.numSamples; i++)
			{
				num += num3;
				a += this.samples[i - 1] * num3;
				Vector2 vector2 = this.samples[i];
				this.samples[i] = vector;
				vector = vector2;
				num3 = num2 * this.weightCoef;
				num2 = num3;
			}
			this.value = a / num;
		}

		public Vector2 GetValue()
		{
			return this.value;
		}

		public Vector2[] GetSamples()
		{
			return this.samples;
		}

		public void Reset(Vector2 resetVal)
		{
			for (int i = 0; i < this.numSamples; i++)
			{
				this.samples[i] = resetVal;
			}
		}

		private Vector2 value;

		private readonly Vector2[] samples;

		private readonly float weightCoef;

		private readonly int numSamples;
	}
}
