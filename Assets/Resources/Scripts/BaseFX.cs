using System;
using UnityEngine;

public class BaseFX : MonoBehaviour
{
	public bool IsActive
	{
		get
		{
			return this.effectIsActive;
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
		this.effectIsActive = true;
		if (this.timeToStop == 0f)
		{
			this.timeToStop = Time.time;
		}
		this.timeToStop = ((!this.StackableDuration) ? (Time.time + this.EffectDuration) : (this.timeToStop + this.EffectDuration));
	}

	public virtual void StopEffect()
	{
		this.effectIsActive = false;
		this.timeToStop = 0f;
	}

	public virtual void ActivateFX()
	{
	}

	public virtual void DeactivateFX()
	{
	}

	public bool DebugLog;

	[Space(10f)]
	[Tooltip("Set 0 for manual control")]
	public float EffectDuration = 5f;

	[Tooltip("Is duration stack or just renew?")]
	public bool StackableDuration;

	protected float activationTime;

	protected bool effectIsActive;

	protected float timeToStop;

	private int currStacks;
}
