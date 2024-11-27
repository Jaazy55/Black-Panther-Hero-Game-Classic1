using System;
using System.Collections.Generic;
using System.Linq;
using Game.Character.CharacterController;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EffectArea : MonoBehaviour
{
	public bool IsActive
	{
		get
		{
			return this.effectIsActive;
		}
	}

	protected virtual void Awake()
	{
		if (this.TriggerCollider == null)
		{
			this.TriggerCollider = base.GetComponent<Collider>();
		}
		this.TriggerCollider.isTrigger = true;
		this.TriggerCollider.enabled = false;
		if (this.VisualEffects == null)
		{
			return;
		}
		this.VisualEffects.EffectDuration = this.EffectDuration;
		this.VisualEffects.StartEffect();
	}

	protected virtual void FixedUpdate()
	{
		if (!this.effectIsActive || this.EffectDuration == 0f)
		{
			return;
		}
		if (Time.time - this.activationTime >= this.EffectDuration)
		{
			this.Deactivate();
		}
	}

	public void SetIgnorable(Collider[] ignorableObjects)
	{
		this.ignoreColliders = ignorableObjects;
	}

	public virtual void Activate()
	{
		if (this.effectIsActive)
		{
			return;
		}
		this.effectIsActive = true;
		this.activationTime = Time.time;
		this.TriggerCollider.enabled = true;
		this.UpdateAffectedColliders(this.CheckTrigger(this.TriggerCollider), false);
		this.StartEffect();
		if (this.VisualEffects != null)
		{
			this.VisualEffects.StartEffect();
		}
	}

	public virtual void Activate(HitEntity initiator)
	{
		this.areaInitiator = initiator;
		this.Activate();
	}

	public virtual void Deactivate()
	{
		if (!this.effectIsActive)
		{
			return;
		}
		this.TriggerCollider.enabled = false;
		this.affectedColliders.Clear();
		this.effectIsActive = false;
		this.StopEffect();
		if (this.VisualEffects != null)
		{
			this.VisualEffects.StopEffect();
		}
	}

	protected virtual void StartEffect()
	{
		if (!this.DebugLog)
		{
			return;
		}
		foreach (Collider collider in this.affectedColliders)
		{
			UnityEngine.Debug.Log(collider.gameObject.name);
		}
	}

	protected virtual void StopEffect()
	{
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if ((1 << other.gameObject.layer & this.AffectedLayers) == 0)
		{
			return;
		}
		this.UpdateAffectedColliders(other, false);
	}

	protected virtual void OnTriggerExit(Collider other)
	{
		if ((1 << other.gameObject.layer & this.AffectedLayers) == 0)
		{
			return;
		}
		this.UpdateAffectedColliders(other, true);
	}

	protected virtual Collider[] CheckTrigger(Collider sensorCollider)
	{
		Collider[] result;
		if (sensorCollider is SphereCollider)
		{
			SphereCollider sphereCollider = sensorCollider as SphereCollider;
			result = Physics.OverlapSphere(sphereCollider.center, sphereCollider.radius, this.AffectedLayers);
		}
		else if (sensorCollider is BoxCollider)
		{
			BoxCollider boxCollider = sensorCollider as BoxCollider;
			result = Physics.OverlapBox(boxCollider.center, boxCollider.bounds.extents, boxCollider.transform.rotation, this.AffectedLayers);
		}
		else if (sensorCollider is CapsuleCollider)
		{
			CapsuleCollider capsuleCollider = sensorCollider as CapsuleCollider;
			Vector3 one = Vector3.one;
			if (capsuleCollider.direction == 0)
			{
				one.y = (one.z = 0f);
			}
			else if (capsuleCollider.direction == 1)
			{
				one.x = (one.z = 0f);
			}
			else
			{
				one.x = (one.y = 0f);
			}
			Vector3 b = one * capsuleCollider.height / 2f - one * capsuleCollider.radius;
			Vector3 a = base.transform.position + capsuleCollider.center;
			if (this.DebugLog)
			{
				UnityEngine.Debug.DrawLine(a + b, a - b, Color.red);
			}
			result = Physics.OverlapCapsule(a + b, a - b, capsuleCollider.radius, this.AffectedLayers);
		}
		else
		{
			result = Physics.OverlapSphere(base.transform.position, 1f, this.AffectedLayers);
		}
		return result;
	}

	protected void UpdateAffectedColliders(Collider incCollider, bool delete = false)
	{
		if (delete)
		{
			this.affectedColliders.Remove(incCollider);
		}
		else if ((this.ignoreColliders == null || !this.ignoreColliders.Contains(incCollider)) && !this.affectedColliders.Contains(incCollider))
		{
			this.affectedColliders.Add(incCollider);
		}
	}

	protected void UpdateAffectedColliders(Collider[] incColliders, bool delete = false)
	{
		foreach (Collider incCollider in incColliders)
		{
			this.UpdateAffectedColliders(incCollider, delete);
		}
	}

	private void OnEnable()
	{
		if ((double)this.EffectDuration < 0.001)
		{
			this.Activate();
		}
	}

	private void OnDisable()
	{
		if ((double)this.EffectDuration < 0.001)
		{
			this.Deactivate();
		}
	}

	[Separator("EffectArea parameters")]
	public bool DebugLog;

	[Space(10f)]
	public Collider TriggerCollider;

	[Space(5f)]
	[Tooltip("0 for permanent activity")]
	public float EffectDuration;

	[Space(5f)]
	public LayerMask AffectedLayers;

	[Space(5f)]
	public BaseFX VisualEffects;

	protected List<Collider> affectedColliders = new List<Collider>();

	protected bool effectIsActive;

	private Collider[] ignoreColliders;

	protected float activationTime;

	protected HitEntity areaInitiator;
}
