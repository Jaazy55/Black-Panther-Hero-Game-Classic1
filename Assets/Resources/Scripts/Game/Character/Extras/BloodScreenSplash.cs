using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.Extras
{
	public class BloodScreenSplash : MonoBehaviour
	{
		private void Awake()
		{
			BloodScreenSplash.Instance = this;
			this.bloodTexture = base.GetComponent<Texture>();
		}

		public void Hit()
		{
			this.timeout = this.FadeoutTimer;
		}

		private void Update()
		{
			//Color color = this.bloodTexture.color;
			//color.a = Mathf.Clamp01(this.timeout / this.FadeoutTimer) * this.MaxAlpha;
			//this.bloodTexture.color = color;
			this.timeout -= Time.deltaTime;
		}

		public static BloodScreenSplash Instance;

		public float FadeoutTimer = 1f;

		public float MaxAlpha = 0.5f;

		private Texture bloodTexture;

		private float timeout;
	}
}
