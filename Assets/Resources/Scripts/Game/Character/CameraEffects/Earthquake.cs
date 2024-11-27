using System;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class Earthquake : CameraEffect
	{
		public override void OnPlay()
		{
			this.diff = Vector2.zero;
		}

		public override void OnUpdate()
		{
			Vector3 eulerAngles = this.unityCamera.transform.rotation.eulerAngles;
			CameraEffect.FadeState fadeState = this.fadeState;
			if (fadeState != CameraEffect.FadeState.FadeIn)
			{
				if (fadeState != CameraEffect.FadeState.FadeOut)
				{
					if (fadeState == CameraEffect.FadeState.Full)
					{
						this.size = this.Size;
					}
				}
				else
				{
					this.size = Interpolation.LerpS2(this.Size, 0f, this.fadeOutNormalized);
				}
			}
			else
			{
				this.size = Interpolation.LerpS3(0f, this.Size, 1f - this.fadeInNormalized);
			}
			Vector3 b = SmoothRandom.GetVector3(this.Speed) * this.size;
			Vector3 euler = eulerAngles - this.diff + b;
			this.diff = b;
			this.unityCamera.transform.rotation = Quaternion.Euler(euler);
		}

		public float Size = 1f;

		public float Speed = 10f;

		private Vector3 diff;

		private float size;
	}
}
