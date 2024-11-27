using System;
using UnityEngine;

public class BaseParticlesVFX : BaseFX
{
	public override void StartEffect()
	{
		base.StartEffect();
		this.ActivateFX();
	}

	public override void StopEffect()
	{
		base.StopEffect();
		this.DeactivateFX();
	}

	public override void ActivateFX()
	{
		foreach (ParticleSystem particleSystem in this.ParticleEffects)
		{
			particleSystem.Play();
		}
	}

	public override void DeactivateFX()
	{
		foreach (ParticleSystem particleSystem in this.ParticleEffects)
		{
			particleSystem.Stop();
		}
	}

	[Separator("BaseParticlesVFX")]
	public bool DebugLog_BaseParticlesVFX;

	[Space(10f)]
	public ParticleSystem[] ParticleEffects;
}
