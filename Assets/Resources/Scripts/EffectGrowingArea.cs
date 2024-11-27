using System;
using UnityEngine;

public class EffectGrowingArea : EffectArea
{
	protected override void Awake()
	{
		base.Awake();
		this.defaultRadius = ((!this.StartFromMin) ? this.MaxRadius : this.MinRadius);
		this.sphereTriggerCollider = (this.TriggerCollider as SphereCollider);
		this.currRadius = this.defaultRadius;
		this.SetAreaRadius(this.currRadius);
	}

	public override void Activate()
	{
		base.Activate();
		this.currRadius = this.defaultRadius;
		this.SetAreaRadius(this.currRadius);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!this.effectIsActive)
		{
			return;
		}
		this.AutoChangeSize();
	}

	private void AutoChangeSize()
	{
		if (this.GrowPerSecond == 0f || this.currRadius < this.MinRadius || this.currRadius > this.MaxRadius)
		{
			return;
		}
		this.currRadius += this.GrowPerSecond * Time.deltaTime;
		this.SetAreaRadius(this.currRadius);
	}

	private void SetAreaRadius(float radius)
	{
		if (!this.sphereTriggerCollider)
		{
			return;
		}
		this.sphereTriggerCollider.radius = radius;
	}

	[Separator("EffectGrowingArea parameters")]
	[Space(10f)]
	public bool DebugLog_EGArea;

	[Space(10f)]
	[Tooltip("Works only with sphere colliders")]
	public float MinRadius = 1f;

	[Tooltip("Works only with sphere colliders")]
	public float MaxRadius = 1f;

	[Tooltip("Works only with sphere colliders")]
	public float GrowPerSecond;

	[Space(5f)]
	public bool StartFromMin = true;

	private float defaultRadius;

	private float currRadius;

	private SphereCollider sphereTriggerCollider;
}
