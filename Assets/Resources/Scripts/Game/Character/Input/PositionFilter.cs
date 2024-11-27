using System;
using UnityEngine;

namespace Game.Character.Input
{
	public class PositionFilter
	{
		public PositionFilter(int samplesNum, float coef)
		{
			this.value = default(Vector3);
			this.weightCoef = coef;
			this.numSamples = samplesNum;
			this.samples = new Vector3[samplesNum];
		}

		public void AddSample(Vector3 sample)
		{
			Vector3 a = default(Vector3);
			float num = 0f;
			float num2 = 1f;
			float num3 = 1f;
			Vector3 vector = this.samples[0];
			this.samples[0] = sample;
			for (int i = 1; i < this.numSamples; i++)
			{
				num += num3;
				a += this.samples[i - 1] * num3;
				Vector3 vector2 = this.samples[i];
				this.samples[i] = vector;
				vector = vector2;
				num3 = num2 * this.weightCoef;
				num2 = num3;
			}
			this.value = a / num;
		}

		public Vector3 GetValue()
		{
			return this.value;
		}

		public Vector3[] GetSamples()
		{
			return this.samples;
		}

		public void Reset(Vector3 resetVal)
		{
			for (int i = 0; i < this.numSamples; i++)
			{
				this.samples[i] = resetVal;
			}
		}

		private Vector3 value;

		private readonly Vector3[] samples;

		private readonly float weightCoef;

		private readonly int numSamples;
	}
}
