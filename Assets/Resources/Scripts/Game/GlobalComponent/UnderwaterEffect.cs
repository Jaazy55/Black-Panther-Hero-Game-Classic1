using System;
using Game.Character;
using Game.GlobalComponent.Quality;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class UnderwaterEffect : MonoBehaviour
	{
		public static UnderwaterEffect Instance
		{
			get
			{
				if (!UnderwaterEffect.instance)
				{
					UnderwaterEffect.instance = UnityEngine.Object.FindObjectOfType<UnderwaterEffect>();
				}
				return UnderwaterEffect.instance;
			}
		}

		private void Awake()
		{
			UnderwaterEffect.instance = this;
			this.noramalColor = RenderSettings.fogColor;
		}

		private void Start()
		{
			this.gameCamera = CameraManager.Instance.UnityCamera;
		}

		public void SetDepth(float d)
		{
			this.depth = d;
		}

		private void UnderwaterCameraEffect()
		{
			if (this.gameCamera.transform.position.y + 0.1f <= this.depth)
			{
				this.EnableEffect();
			}
			else
			{
				this.DisableEffect();
				this.depth = -1000f;
			}
		}

		public void EnableEffect()
		{
			if (this.effectEnabled)
			{
				return;
			}
			if (this.gameCamera == null)
			{
				this.gameCamera = CameraManager.Instance.UnityCamera;
			}
			RenderSettings.fogEndDistance = this.FogStartDistance;
			RenderSettings.fogStartDistance = this.FogStartDistance / 4f;
			RenderSettings.fogColor = this.FogColor;
			this.gameCamera.clearFlags = CameraClearFlags.Color;
			this.gameCamera.backgroundColor = this.FogColor;
			this.effectEnabled = true;
		}

		public void DisableEffect()
		{
			if (!this.effectEnabled)
			{
				return;
			}
			RenderSettings.fogColor = this.noramalColor;
			QualityManager.ChangeFog(false);
			if (this.gameCamera)
			{
				this.gameCamera.clearFlags = CameraClearFlags.Skybox;
			}
			this.effectEnabled = false;
		}

		private void Update()
		{
			this.UnderwaterCameraEffect();
		}

		private static UnderwaterEffect instance;

		public Color FogColor;

		public float FogDensity;

		public float FogStartDistance;

		private Color noramalColor;

		private Camera gameCamera;

		private float depth = -1000f;

		private bool effectEnabled;
	}
}
