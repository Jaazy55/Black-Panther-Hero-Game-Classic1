using System;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class FireKick : CameraEffect
	{
		public override void OnPlay()
		{
			this.diff = 0f;
			this.KickTime = Mathf.Clamp(this.KickTime, 0f, this.Length);
		}

		public override void OnUpdate()
		{
			Vector3 eulerAngles = this.unityCamera.transform.rotation.eulerAngles;
			float num;
			if (this.timeout < this.KickTime)
			{
				float t = this.timeout / this.KickTime;
				num = Interpolation.LerpS2(0f, this.KickAngle, t);
			}
			else
			{
				float t2 = (this.timeout - this.KickTime) / (this.Length - this.KickTime);
				num = Interpolation.LerpS(this.KickAngle, 0f, t2);
			}
			num = -num;
			float x = eulerAngles.x - this.diff + num;
			this.diff = num;
			Vector3 euler = eulerAngles;
			euler.x = x;
			this.unityCamera.transform.rotation = Quaternion.Euler(euler);
		}

		public float KickTime;

		public float KickAngle;

		private float diff;

		private float kickTimeout;
	}
}
