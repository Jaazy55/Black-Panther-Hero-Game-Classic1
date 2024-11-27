using System;
using System.Linq;
using Game.Character.Stats;
using UnityEngine;

public class WorldStatBar : CharacterStatDisplay
{
	protected override void Start()
	{
		base.Start();
		if (this.CamTrf == null)
		{
			this.CamTrf = Camera.main.transform;
		}
		if (this.TextMesh == null)
		{
			this.TextMesh = base.GetComponentInChildren<TextMesh>();
		}
		if (this.Children == null || this.Children.Length <= 0)
		{
			this.Children = (from t in base.GetComponentsInChildren<Transform>()
			where t.parent.Equals(base.transform)
			select t.gameObject).ToArray<GameObject>();
		}
	}

	protected override void UpdateDisplayValue()
	{
		if (this.current < this.max && this.current > 0f)
		{
			this.SetStatBarActive(true);
			this.percentage = this.current / this.max;
			if (this.SpriteFrontTrf != null)
			{
				this.SpriteFrontTrf.localScale = new Vector3(this.percentage, this.SpriteFrontTrf.localScale.y, this.SpriteFrontTrf.localScale.z);
			}
			if (this.TextMesh != null)
			{
				this.TextMesh.text = (this.percentage * 100f).ToString("F0") + "/100";
			}
			base.transform.rotation = this.CamTrf.rotation;
		}
		else
		{
			this.SetStatBarActive(false);
		}
	}

	private void SetStatBarActive(bool on)
	{
		if (this.isSetActive != on)
		{
			foreach (GameObject gameObject in this.Children)
			{
				gameObject.gameObject.SetActive(on);
			}
			this.isSetActive = on;
		}
	}

	public override void OnChanged(float amount)
	{
	}

	public Transform CamTrf;

	public Transform SpriteFrontTrf;

	public TextMesh TextMesh;

	private float percentage;

	private const string ofHundredString = "/100";

	public GameObject[] Children;

	private bool isSetActive = true;
}
