using System;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class Fall : CameraEffect
	{
		public override void Init()
		{
			base.Init();
			this.spring = new Spring();
			this.spring.Setup(this.Mass, this.Distance, this.Strength, this.Damping);
		}

		public override void OnPlay()
		{
			this.frameCounter = (float)this.ForceFrames;
			this.spring.Setup(this.Mass, this.Distance, this.Strength, this.Damping);
		}

		public override void OnUpdate()
		{
			if (this.frameCounter > 0f)
			{
				this.spring.AddForce(this.Force);
				this.frameCounter -= 1f;
			}
			float num = this.spring.Calculate(Time.deltaTime);
			float d = 1f;
			CameraEffect.FadeState fadeState = this.fadeState;
			if (fadeState != CameraEffect.FadeState.FadeIn)
			{
				if (fadeState == CameraEffect.FadeState.FadeOut)
				{
					d = Interpolation.LerpS2(num, 0f, this.fadeOutNormalized);
				}
			}
			else
			{
				d = Interpolation.LerpS3(0f, num, 1f - this.fadeInNormalized);
			}
			float num2 = Mathf.Clamp01(this.ImpactVelocity / 10f);
			this.DistanceMax = num2 * 2f;
			if (num > this.DistanceMax)
			{
				num = this.DistanceMax;
			}
			this.unityCamera.transform.position += Vector3.up * num * d * -1f;
		}

		public float Mass;

		public float Distance;

		public float Strength;

		public float Damping;

		public float Force;

		public int ForceFrames;

		public float ImpactVelocity;

		private Spring spring;

		private float frameCounter;

		private float DistanceMax;
	}
}
