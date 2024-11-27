using System;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class ExplosionCFX : CameraEffect
	{
		public override void Init()
		{
			base.Init();
			this.posSpring = new Spring();
		}

		public override void OnPlay()
		{
			this.posSpring.Setup(this.Mass, this.Distance, this.Strength, this.Damping);
			this.v0 = (this.position - this.unityCamera.transform.position).normalized;
			this.diff = Vector3.zero;
		}

		public override void OnUpdate()
		{
			Vector3 eulerAngles = this.unityCamera.transform.rotation.eulerAngles;
			this.size = this.Size;
			float num = this.posSpring.Calculate(Time.deltaTime);
			float d = 1f;
			CameraEffect.FadeState fadeState = this.fadeState;
			if (fadeState != CameraEffect.FadeState.FadeIn)
			{
				if (fadeState == CameraEffect.FadeState.FadeOut)
				{
					d = Interpolation.LerpS2(num, 0f, this.fadeOutNormalized);
					this.size = Interpolation.LerpS2(this.Size, 0f, this.fadeOutNormalized);
				}
			}
			else
			{
				d = Interpolation.LerpS3(0f, num, 1f - this.fadeInNormalized);
				this.size = Interpolation.LerpS3(0f, this.Size, 1f - this.fadeInNormalized);
			}
			Vector3 b = SmoothRandom.GetVector3(this.Speed) * this.size;
			Vector3 euler = eulerAngles - this.diff + b;
			this.diff = b;
			this.unityCamera.transform.rotation = Quaternion.Euler(euler);
			this.unityCamera.transform.position += this.v0 * num * d;
		}

		public float Mass;

		public float Distance;

		public float Strength;

		public float Damping;

		public Vector3 position;

		public float Size = 1f;

		public float Speed = 10f;

		private float size;

		private Spring posSpring;

		private Vector3 v0;

		private Vector3 diff;
	}
}
