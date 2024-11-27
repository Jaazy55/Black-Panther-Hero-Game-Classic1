using System;
using System.Collections.Generic;
using Game.Managers;
using UnityEngine;

public class AreaVFX : BaseFX
{
	private void Start()
	{
		if (this.SampleCollider == null)
		{
			this.defaultRadius = ((!this.StartFromMin) ? this.MaxRadius : this.MinRadius);
		}
		else
		{
			this.defaultRadius = this.SampleCollider.radius;
		}
		this.currRadius = this.defaultRadius;
		this.particleSystems.Capacity = 10;
		foreach (GameObject gameObject in this.GameObjects)
		{
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			if (component != null)
			{
				this.particleSystems.Add(component);
			}
		}
		if (this.DebugLog_AreaVFX && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log("AreaVFX " + base.gameObject.name + " started");
		}
		this.SetAreaRadius(this.currRadius);
		this.effectIsActive = true;
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
		if (this.SampleCollider == null)
		{
			if (this.GrowPerSecond == 0f || this.currRadius < this.MinRadius || this.currRadius > this.MaxRadius)
			{
				return;
			}
			this.currRadius += this.GrowPerSecond * Time.deltaTime;
		}
		else
		{
			this.currRadius = this.SampleCollider.radius;
		}
		this.SetAreaRadius(this.currRadius);
	}

	private void StopAndClearParticleSystems()
	{
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			particleSystem.Stop();
			particleSystem.Clear();
		}
	}

	private void StopParticleSystems()
	{
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			particleSystem.Stop();
		}
	}

	private void SetAreaRadius(float radius)
	{
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			particleSystem.transform.localScale = new Vector3((this.ScaleMultiplier.x != 0f) ? (radius * this.ScaleMultiplier.x) : 1f, (this.ScaleMultiplier.y != 0f) ? (radius * this.ScaleMultiplier.y) : 1f, (this.ScaleMultiplier.z != 0f) ? (radius * this.ScaleMultiplier.z) : 1f);
		}
	}

	public override void ActivateFX()
	{
		this.currRadius = this.defaultRadius;
		this.SetAreaRadius(this.currRadius);
		base.ActivateFX();
	}

	public override void DeactivateFX()
	{
		this.StopParticleSystems();
		base.DeactivateFX();
	}

	[Separator("AreaVFX parameters")]
	public bool DebugLog_AreaVFX;

	[Space(10f)]
	public GameObject[] GameObjects;

	[Space(5f)]
	[Tooltip("Множитель позволяет настроить масштаб эффекта для соответствия реальным размерам пространства. 0 означает, что по этой ветке изменений не нужно.")]
	public Vector3 ScaleMultiplier = Vector3.one;

	[Space(10f)]
	[Tooltip("Если есть образец - игнорирует остальные параметры.")]
	public SphereCollider SampleCollider;

	[Space(5f)]
	public float MinRadius = 1f;

	public float MaxRadius = 1f;

	public float GrowPerSecond;

	[Space(5f)]
	public bool StartFromMin = true;

	private ParticleSystem ps;

	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	private float defaultRadius;

	private float currRadius;
}
