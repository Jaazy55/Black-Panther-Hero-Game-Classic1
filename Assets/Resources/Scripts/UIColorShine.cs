using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(MaskableGraphic))]
public sealed class UIColorShine : UISpriteColorBase
{
	protected override void Initialize()
	{
		this.shineLocationParameterID = Shader.PropertyToID(this.m_LocationPropertyName);
		this.shineWidthParameterID = Shader.PropertyToID(this.m_ShineWidthPropertyName);
		this.shineEmissionParameterID = Shader.PropertyToID(this.m_EmissionPropertyName);
		base.Initialize();
	}

	protected override void UpdateShader()
	{
		this.uispriteRenderer.material.SetFloat(this.shineLocationParameterID, this.shinePositon);
		this.uispriteRenderer.material.SetFloat(this.shineWidthParameterID, this.shineWidth);
		this.uispriteRenderer.material.SetFloat(this.shineEmissionParameterID, this.shineEmission);
	}

	[SerializeField]
	private string m_LocationPropertyName = "_ShineLocation";

	private int shineLocationParameterID;

	[SerializeField]
	private string m_ShineWidthPropertyName = "_ShineWidth";

	private int shineWidthParameterID;

	[SerializeField]
	private string m_EmissionPropertyName = "_EmissionGain";

	private int shineEmissionParameterID;

	public float shinePositon;

	public float shineWidth = 0.01f;

	public float shineEmission = 0.3f;
}
