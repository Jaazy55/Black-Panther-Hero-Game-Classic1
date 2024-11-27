using System;
using UnityEngine;

public class TrailedComboVFX : BaseComboVFX
{
	public override void ActivateFX()
	{
		base.ActivateFX();
		foreach (TrailRenderer trailRenderer in this.Trails)
		{
			trailRenderer.gameObject.SetActive(true);
		}
	}

	public override void DeactivateFX()
	{
		base.DeactivateFX();
		foreach (TrailRenderer trailRenderer in this.Trails)
		{
			trailRenderer.gameObject.SetActive(false);
		}
	}

	[Separator("TrailedComboVFX")]
	public bool DebugLog_TrailedComboVFX;

	[Space(10f)]
	public TrailRenderer[] Trails;
}
