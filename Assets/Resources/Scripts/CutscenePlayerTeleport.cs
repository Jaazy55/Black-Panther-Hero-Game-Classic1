using System;
using System.Collections;
using Game.Character;
using Game.GlobalComponent;
using Naxeex;
using UnityEngine;

public class CutscenePlayerTeleport : Cutscene
{
	public override void StartScene()
	{
		this.IsPlaying = true;
		base.StartCoroutine(this.WaitTeleportFucntion());
	}

	private IEnumerator WaitTeleportFucntion()
	{
		if (this.DelayedLaunch > 0f)
		{
			yield return new WaitForSeconds(this.DelayedLaunch);
		}
		PlayerInteractionsManager.Instance.TeleportPlayerToPosition(this.teleportPosition.position, this.teleportPosition.rotation);
		yield return new WaitForSeconds(0.25f);
		this.EndScene(true);
		yield break;
	}

	public SpatialPoint teleportPosition;

	[SerializeField]
	private float DelayedLaunch;
}
