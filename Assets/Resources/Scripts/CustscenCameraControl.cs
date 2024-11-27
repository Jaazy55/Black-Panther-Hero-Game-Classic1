using System;
using Game.Character;
using Game.Character.Modes;
using Game.GlobalComponent;
using UnityEngine;

public class CustscenCameraControl : Cutscene
{
	public override void StartScene()
	{
		this.IsPlaying = true;
		this.Target = PlayerInteractionsManager.Instance.Player.transform;
		this.cameraMode = PoolManager.Instance.GetFromPool<CameraMode>(this.CameraMode);
		CameraManager.Instance.SetMode(this.cameraMode, false);
		CameraManager.Instance.SetCameraTarget(this.Target);
		this.cameraTransform = CameraManager.Instance.UnityCamera.transform;
		this.cameraTransform.position = this.CameraPosition.position;
		this.isChangeMode = true;
		if (this.mainManager.Scenes.Length > this.mainManager.CurrentIndex() + 1)
		{
			foreach (Cutscene cutscene in this.mainManager.Scenes[this.mainManager.CurrentIndex() + 1].Scenes)
			{
				if (cutscene as CustscenCameraControl)
				{
					this.isChangeMode = false;
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (!this.IsPlaying)
		{
			return;
		}
		this.cameraTransform.position = this.CameraPosition.position;
	}

	public override void EndScene(bool isCheck)
	{
		base.EndScene(isCheck);
		PoolManager.Instance.ReturnToPool(this.CameraMode);
		if (this.isChangeMode || !this.mainManager.Inited)
		{
			CameraManager.Instance.SetMode(CameraManager.Instance.ActivateModeOnStart, false);
		}
	}

	public CameraMode CameraMode;

	public Transform Target;

	public Transform CameraPosition;

	private Transform cameraTransform;

	private CameraMode cameraMode;

	private bool isChangeMode;
}
