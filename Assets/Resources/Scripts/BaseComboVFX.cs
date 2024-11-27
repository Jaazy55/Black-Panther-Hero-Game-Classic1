using System;
using UnityEngine;

public class BaseComboVFX : BaseParticlesVFX
{
	public override void ActivateFX()
	{
		base.ActivateFX();
		foreach (ParticleSystem particleSystem in this.OneshotParticles)
		{
			particleSystem.Emit(1);
		}
	}

	[Separator("BaseComboVFX")]
	public bool DebugLog_BaseComboVFX;

	[Space(10f)]
	public ParticleSystem[] OneshotParticles;
}
