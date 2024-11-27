using System;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public abstract class CameraEffect : MonoBehaviour
	{
		public bool Playing { get; protected set; }

		private void Start()
		{
			if (!this.unityCamera)
			{
				EffectManager.Instance.Register(this);
				this.Init();
			}
		}

		public virtual void Init()
		{
			this.Playing = false;
			this.unityCamera = CameraManager.Instance.UnityCamera;
		}

		public void Play()
		{
			this.Playing = true;
			this.timeout = 0f;
			this.FadeIn = Mathf.Clamp(this.FadeIn, 0f, this.Length);
			this.FadeOut = Mathf.Clamp(this.FadeOut, 0f, this.Length);
			this.OnPlay();
		}

		public void Stop()
		{
			this.Playing = false;
			this.OnStop();
		}

		public virtual void OnPlay()
		{
		}

		public virtual void OnStop()
		{
		}

		public virtual void OnUpdate()
		{
		}

		public void PostUpdate()
		{
			this.timeout += Time.deltaTime;
			this.timeoutNormalized = Mathf.Clamp01(this.timeout / this.Length);
			this.fadeState = CameraEffect.FadeState.Full;
			if (this.FadeIn > 0f)
			{
				if (this.timeout < this.FadeIn)
				{
					this.fadeInNormalized = this.timeout / this.FadeIn;
					this.fadeState = CameraEffect.FadeState.FadeIn;
				}
				else
				{
					this.fadeInNormalized = 1f;
				}
			}
			if (this.FadeOut > 0f)
			{
				if (this.timeout > this.Length - this.FadeOut)
				{
					this.fadeOutNormalized = (this.timeout - (this.Length - this.FadeOut)) / this.FadeOut;
					this.fadeState = CameraEffect.FadeState.FadeOut;
				}
				else
				{
					this.fadeOutNormalized = 0f;
				}
			}
			if (this.timeout > this.Length)
			{
				if (this.Loop)
				{
					this.Play();
				}
				else
				{
					this.Stop();
				}
			}
			this.OnUpdate();
		}

		public void Delete()
		{
			EffectManager.Instance.Delete(this);
		}

		public bool Loop;

		public float Length = 1f;

		public float FadeIn = 0.5f;

		public float FadeOut = 0.5f;

		protected float timeout;

		protected float timeoutNormalized;

		protected float fadeInNormalized;

		protected float fadeOutNormalized;

		protected CameraEffect.FadeState fadeState;

		protected Camera unityCamera;

		protected enum FadeState
		{
			FadeIn,
			Full,
			FadeOut
		}
	}
}
