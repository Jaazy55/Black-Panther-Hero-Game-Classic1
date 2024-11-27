using System;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class Stomp : CameraEffect
	{
		public override void Init()
		{
			base.Init();
			this.spring = new Spring();
		}

		public override void OnPlay()
		{
			this.spring.Setup(this.Mass, this.Distance, this.Strength, this.Damping);
		}

		public override void OnUpdate()
		{
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
			this.unityCamera.transform.position += Vector3.up * num * d;
		}

		public float Mass;

		public float Distance;

		public float Strength;

		public float Damping;

		private Spring spring;
	}
}
