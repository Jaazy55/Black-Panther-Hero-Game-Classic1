using System;
using UnityEngine;

namespace Game.PostEffect
{
	[ExecuteInEditMode]
	public class PostEffectDrawingCrosshatch : MonoBehaviour
	{
		private Material Material
		{
			get
			{
				if (this.scMaterial == null)
				{
					this.scMaterial = new Material(this.ScShader)
					{
						hideFlags = HideFlags.HideAndDontSave
					};
				}
				return this.scMaterial;
			}
		}

		private void Start()
		{
			PostEffectDrawingCrosshatch.ChangeWidth = this.Width;
			this.ScShader = Shader.Find("PostEffect/Drawing_Crosshatch");
			if (!SystemInfo.supportsImageEffects)
			{
				base.enabled = false;
			}
		}

		private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
		{
			if (this.ScShader != null)
			{
				this.timeX += Time.deltaTime;
				if (this.timeX > 100f)
				{
					this.timeX = 0f;
				}
				this.Material.SetFloat("_TimeX", this.timeX);
				this.Material.SetFloat("_Distortion", this.Width);
				this.Material.SetVector("_ScreenResolution", new Vector4((float)sourceTexture.width, (float)sourceTexture.height, 0f, 0f));
				Graphics.Blit(sourceTexture, destTexture, this.Material);
			}
			else
			{
				Graphics.Blit(sourceTexture, destTexture);
			}
		}

		private void Update()
		{
			if (Application.isPlaying)
			{
				this.Width = PostEffectDrawingCrosshatch.ChangeWidth;
			}
		}

		private void OnDisable()
		{
			if (this.scMaterial)
			{
				UnityEngine.Object.DestroyImmediate(this.scMaterial);
			}
		}

		public static float ChangeWidth;

		public Shader ScShader;

		private float timeX = 1f;

		private Vector4 screenResolution;

		private Material scMaterial;

		[Range(1f, 10f)]
		public float Width = 2f;
	}
}
