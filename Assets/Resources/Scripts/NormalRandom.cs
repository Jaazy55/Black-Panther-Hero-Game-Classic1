using System;
using UnityEngine;

public static class NormalRandom
{
	public static float Range(NormalRandom.GausParam gausParam = default(NormalRandom.GausParam), int MaxAttemptNumber = 10)
	{
		float num = -1f;
		float num2;
		do
		{
			num += 1f;
			num2 = Mathf.Cos(6.28318548f * UnityEngine.Random.value) * Mathf.Sqrt(-2f * Mathf.Log(UnityEngine.Random.value));
			num2 = num2 * gausParam.Deviation + gausParam.Mean;
			if (num2 > gausParam.LeftBord && num2 < gausParam.RightBord)
			{
				break;
			}
		}
		while (num < (float)MaxAttemptNumber);
		if (num >= (float)MaxAttemptNumber)
		{
			num2 = 0f;
		}
		return num2;
	}

	public struct GausParam
	{
		public GausParam(float deviation = 0.125f, float mean = 0.5f, float leftBord = 0f, float rightBord = 1f)
		{
			this.Deviation = deviation;
			this.Mean = mean;
			this.LeftBord = leftBord;
			this.RightBord = rightBord;
		}

		public float Deviation;

		public float Mean;

		public float LeftBord;

		public float RightBord;
	}
}
