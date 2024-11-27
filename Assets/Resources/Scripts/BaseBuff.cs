using System;
using UnityEngine;

[Serializable]
public class BaseBuff : MonoBehaviour
{
	public bool IsActive
	{
		get
		{
			return this.effectIsActive;
		}
	}

	protected virtual void Start()
	{
		if (this.VFX != null)
		{
			this.VFX.EffectDuration = this.EffectDuration;
			this.VFX.StackableDuration = this.StackableDuration;
		}
	}

	protected virtual void FixedUpdate()
	{
		if (!this.effectIsActive)
		{
			return;
		}
		this.CheckAutoStop();
	}

	protected void CheckAutoStop()
	{
		if (this.EffectDuration == 0f)
		{
			return;
		}
		if (this.timeToStop != 0f && Time.time >= this.timeToStop)
		{
			this.StopEffect();
		}
	}

	public virtual void StartEffect()
	{
		if (this.currStacks < this.MaxStacks)
		{
			this.currStacks++;
			this.AddStackEffect();
		}
		this.effectIsActive = true;
		if (this.timeToStop == 0f)
		{
			this.timeToStop = Time.time;
		}
		this.timeToStop = ((!this.StackableDuration) ? (Time.time + this.EffectDuration) : (this.timeToStop + this.EffectDuration));
		if (this.VFX != null)
		{
			this.VFX.StartEffect();
		}
	}

	public virtual void StopEffect()
	{
		this.currStacks = 0;
		this.ClearStacksEffects();
		this.effectIsActive = false;
		this.timeToStop = 0f;
		if (this.VFX != null)
		{
			this.VFX.StopEffect();
		}
	}

	public virtual void AddStackEffect()
	{
	}

	public virtual void ClearStacksEffects()
	{
	}

	[Separator("BaseBuff")]
	public bool DebugLog;

	[Space(10f)]
	[Tooltip("If not oneshot. How many time it's effect can be stacked?")]
	public int MaxStacks = 1;

	[Space(10f)]
	[Tooltip("Set 0 for manual control")]
	public float EffectDuration = 5f;

	[Tooltip("Is duration stack or just renew?")]
	public bool StackableDuration;

	[Space(10f)]
	public BaseFX VFX;

	protected float activationTime;

	protected bool effectIsActive;

	protected float timeToStop;

	protected int currStacks;
}
