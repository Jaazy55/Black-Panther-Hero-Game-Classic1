using System;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class Yes : CameraEffect
	{
		public override void OnPlay()
		{
			this.diff = 0f;
			this.origPos = this.unityCamera.transform.position;
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
						this.size = this.Angle;
						this.currPos = this.origPos;
					}
				}
				else
				{
					this.size = Interpolation.LerpS2(this.Angle, 0f, this.fadeOutNormalized);
					this.currPos = Interpolation.LerpS2(this.origPos, this.unityCamera.transform.position, this.fadeOutNormalized);
				}
			}
			else
			{
				this.size = Interpolation.LerpS3(0f, this.Angle, 1f - this.fadeInNormalized);
				this.currPos = this.origPos;
			}
			float num = Mathf.Sin(this.timeout * this.Speed) * this.size;
			float x = eulerAngles.x - this.diff + num;
			this.diff = num;
			Vector3 euler = eulerAngles;
			euler.x = x;
			this.unityCamera.transform.position = this.currPos;
			this.unityCamera.transform.rotation = Quaternion.Euler(euler);
		}

		public float Angle = 1f;

		public float Speed = 10f;

		private float diff;

		private float size;

		private Vector3 origPos;

		private Vector3 currPos;
	}
}
