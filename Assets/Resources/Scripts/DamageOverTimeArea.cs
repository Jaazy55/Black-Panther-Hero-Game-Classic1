using System;
using System.Collections.Generic;
using System.Linq;
using Game.Character.CharacterController;
using Game.Enemy;
using UnityEngine;

public class DamageOverTimeArea : EffectGrowingArea
{
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (Time.time - this.lastTicTime >= this.TicPeriod)
		{
			this.HitThemAll();
			this.lastTicTime = Time.time;
		}
	}

	public void SetIgnorable(HitEntity[] ignorableObjects)
	{
		this.ignoreHitEntities = ignorableObjects;
	}

	public override void Activate()
	{
		base.Activate();
		this.UpdateAffectedHitEntities();
	}

	public override void Deactivate()
	{
		base.Deactivate();
		this.affectedHitEntities.Clear();
	}

	protected override void StartEffect()
	{
	}

	private void HitThemAll()
	{
		List<HitEntity> list = new List<HitEntity>();
		foreach (HitEntity hitEntity in this.affectedHitEntities)
		{
			if (!(hitEntity == null) && hitEntity.isActiveAndEnabled)
			{
				hitEntity.OnHit(this.DamageType, this.areaInitiator, this.DamagePerTic, Vector3.zero, Vector3.zero, 0f);
				if (hitEntity.IsDead)
				{
					list.Add(hitEntity);
				}
			}
		}
		this.UpdateAffectedHitEntities(list, true);
	}

	protected void UpdateAffectedHitEntities()
	{
		foreach (Collider collider in this.affectedColliders)
		{
			HitEntity component = collider.GetComponent<HitEntity>();
			if (component != null)
			{
				this.UpdateAffectedHitEntities(component, false);
			}
		}
	}

	protected void UpdateAffectedHitEntities(HitEntity incHitEntity, bool delete = false)
	{
		if (incHitEntity is BodyPartDamageReciever)
		{
			incHitEntity = (incHitEntity as BodyPartDamageReciever).RerouteEntity;
		}
		if (delete)
		{
			this.affectedHitEntities.Remove(incHitEntity);
		}
		else if ((this.ignoreHitEntities == null || !this.ignoreHitEntities.Contains(incHitEntity)) && !this.affectedHitEntities.Contains(incHitEntity))
		{
			this.affectedHitEntities.Add(incHitEntity);
		}
	}

	protected void UpdateAffectedHitEntities(List<HitEntity> incHitEntities, bool delete = false)
	{
		foreach (HitEntity incHitEntity in incHitEntities)
		{
			this.UpdateAffectedHitEntities(incHitEntity, delete);
		}
	}

	protected new virtual void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		HitEntity component = other.GetComponent<HitEntity>();
		if (component != null)
		{
			this.UpdateAffectedHitEntities(component, false);
		}
	}

	protected new virtual void OnTriggerExit(Collider other)
	{
		base.OnTriggerExit(other);
		HitEntity component = other.GetComponent<HitEntity>();
		if (component != null)
		{
			this.UpdateAffectedHitEntities(component, true);
		}
	}

	[Separator("DamageOverTimeArea parameters")]
	public bool DebugLog_DoTArea;

	[Space(10f)]
	public float TicPeriod = 1f;

	public float DamagePerTic = 100f;

	public DamageType DamageType;

	private List<HitEntity> affectedHitEntities = new List<HitEntity>();

	private HitEntity[] ignoreHitEntities;

	private float lastTicTime;
}
