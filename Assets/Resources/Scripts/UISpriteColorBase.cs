using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class UISpriteColorBase : MonoBehaviour
{
	private void OnEnable()
	{
		this.uispriteRenderer = base.gameObject.GetComponent<MaskableGraphic>();
		if (this.uispriteRenderer != null)
		{
			this.CreateMaterial();
			UISpriteColorBase component = base.gameObject.GetComponent<UISpriteColorBase>();
			this.Initialize();
		}
		else
		{
			UnityEngine.Debug.LogWarning(string.Format("'{0}' without UISpriteRenderer, disabled.", base.GetType().ToString()));
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		if (this.uispriteRenderer != null && this.uispriteRenderer.material != null && string.CompareOrdinal(this.uispriteRenderer.material.name, "UI/Default") != 0)
		{
			this.uispriteRenderer.material = null;
		}
	}

	private void Update()
	{
		if (this.uispriteRenderer == null)
		{
			this.uispriteRenderer = base.gameObject.GetComponent<MaskableGraphic>();
		}
		if (this.uispriteRenderer != null && this.uispriteRenderer.material != null)
		{
			this.UpdateShader();
		}
	}

	protected void CreateMaterial()
	{
		string text = base.GetType().ToString().Replace("UISpriteColorBase.", string.Empty);
		if (this.m_Shader == null)
		{
			UnityEngine.Debug.LogWarning(string.Format("Failed to load '{0}', {1} disabled.", "Shader", text));
			base.enabled = false;
		}
		else if (!this.m_Shader.isSupported)
		{
			UnityEngine.Debug.LogWarning(string.Format("Shader '{0}' not supported, {1} disabled.", "Shader", text));
			base.enabled = false;
		}
		else
		{
			if (this.uispriteRenderer == null)
			{
				this.uispriteRenderer = base.gameObject.GetComponent<MaskableGraphic>();
			}
			bool flag = false;
			Color value = Color.white;
			Vector2 value2 = Vector2.zero;
			Vector2 value3 = Vector2.one;
			Vector2 value4 = Vector2.zero;
			Vector2 value5 = Vector2.one;
			bool flag2 = false;
			if (this.uispriteRenderer.material != null)
			{
				flag = this.uispriteRenderer.material.IsKeywordEnabled("PIXELSNAP_ON");
				value = this.uispriteRenderer.material.color;
				value2 = this.uispriteRenderer.material.GetTextureOffset("_MainTex");
				value3 = this.uispriteRenderer.material.GetTextureScale("_MainTex");
				value4 = Vector2.zero;
				value5 = Vector2.one;
				flag2 = this.uispriteRenderer.material.IsKeywordEnabled("_BumpMap");
				if (flag2)
				{
					value4 = this.uispriteRenderer.material.GetTextureOffset("_BumpMap");
					value5 = this.uispriteRenderer.material.GetTextureScale("_BumpMap");
				}
			}
			this.uispriteRenderer.material = new Material(this.m_Shader);
			this.uispriteRenderer.material.name = string.Format("UISprite/{0}", text);
			if (flag)
			{
				this.uispriteRenderer.material.SetFloat("PixelSnap", 1f);
				this.uispriteRenderer.material.EnableKeyword("PIXELSNAP_ON");
			}
			this.uispriteRenderer.material.SetColor("_Color", value);
			this.uispriteRenderer.material.SetTextureOffset("_MainTex", value2);
			this.uispriteRenderer.material.SetTextureScale("_MainTex", value3);
			if (flag2)
			{
				this.uispriteRenderer.material.SetTextureOffset("_BumpMap", value4);
				this.uispriteRenderer.material.SetTextureScale("_BumpMap", value5);
			}
		}
	}

	protected virtual void Initialize()
	{
	}

	protected abstract void UpdateShader();

	protected MaskableGraphic uispriteRenderer;

	public Shader m_Shader;
}
