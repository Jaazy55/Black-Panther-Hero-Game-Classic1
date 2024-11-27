using System;
using UnityEngine;

public class BaseGameObjectsVFX : BaseFX
{
	public override void StartEffect()
	{
		this.ActivateFX();
		base.StartEffect();
	}

	public override void StopEffect()
	{
		this.DeactivateFX();
		base.StopEffect();
	}

	public override void ActivateFX()
	{
		foreach (GameObject gameObject in this.GameObjects)
		{
			gameObject.SetActive(true);
		}
	}

	public override void DeactivateFX()
	{
		foreach (GameObject gameObject in this.GameObjects)
		{
			gameObject.SetActive(false);
		}
	}

	[Separator("BaseGameObjectsVFX")]
	public bool DebugLog_BaseGameObjectsVFX;

	[Space(10f)]
	public GameObject[] GameObjects;
}
