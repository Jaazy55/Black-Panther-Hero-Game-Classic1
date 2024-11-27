using System;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class SmoothRandom
	{
		public static Vector3 GetVector3(float speed)
		{
			float x = Time.time * 0.01f * speed;
			return new Vector3(SmoothRandom.Get().HybridMultifractal(x, 15.73f, 0f), SmoothRandom.Get().HybridMultifractal(x, 63.94f, 0f), SmoothRandom.Get().HybridMultifractal(x, 0.2f, 0f));
		}

		public static float Get(float speed)
		{
			float num = Time.time * 0.01f * speed;
			return SmoothRandom.Get().HybridMultifractal(num * 0.01f, 15.7f, 0.65f);
		}

		private static FractalNoise Get()
		{
			FractalNoise result;
			if ((result = SmoothRandom.s_Noise) == null)
			{
				result = (SmoothRandom.s_Noise = new FractalNoise(1.27f, 2.04f, 8.36f));
			}
			return result;
		}

		private static FractalNoise s_Noise;
	}
}
