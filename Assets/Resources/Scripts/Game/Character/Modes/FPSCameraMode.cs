using System;
using Game.Character.Config;
using Game.Character.Input;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(FPSConfig))]
	public class FPSCameraMode : CameraMode
	{
		public override Type Type
		{
			get
			{
				return Type.FPS;
			}
		}

		public override void OnActivate()
		{
			base.OnActivate();
			if (this.Target)
			{
				this.cameraTarget = this.Target.position;
				this.UnityCamera.transform.position = this.GetEyePos();
				this.UnityCamera.transform.LookAt(this.GetEyePos() + this.Target.forward);
				this.RotateCamera(Vector2.zero);
				this.targetHide = false;
				this.activateTimeout = 1f;
			}
		}

		public override void OnDeactivate()
		{
			this.ShowTarget(true);
		}

		private Vector3 GetEyePos()
		{
			if (this.config.IsVector3("TargetOffset"))
			{
				return this.Target.transform.position + this.config.GetVector3("TargetOffset");
			}
			return this.Target.transform.position;
		}

		private void UpdateFOV()
		{
			this.UnityCamera.fieldOfView = this.config.GetFloat("FOV");
		}

		public override void SetCameraTarget(Transform target)
		{
			base.SetCameraTarget(target);
			if (target)
			{
				this.cameraTarget = this.Target.position;
				this.UnityCamera.transform.position = this.GetEyePos();
				this.UnityCamera.transform.LookAt(this.GetEyePos() + this.Target.forward);
				this.RotateCamera(Vector2.zero);
			}
		}

		public override void Init()
		{
			base.Init();
			this.config = base.GetComponent<FPSConfig>();
			this.cameraTarget = this.Target.position;
			this.UnityCamera.transform.position = this.GetEyePos();
			if (this.config.IsFloat("RotationSpeedY"))
			{
				this.RotateCamera(Vector2.zero);
			}
		}

		public void RotateCamera(Vector2 mousePosition)
		{
			Game.Character.Utils.Math.ToSpherical(this.UnityCamera.transform.forward, out this.rotX, out this.rotY);
			this.rotY += this.config.GetFloat("RotationSpeedY") * mousePosition.y * 0.01f;
			this.rotX += this.config.GetFloat("RotationSpeedX") * mousePosition.x * 0.01f;
			float num = -this.rotY * 57.29578f;
			float @float = this.config.GetFloat("RotationYMax");
			float float2 = this.config.GetFloat("RotationYMin");
			if (num > @float)
			{
				this.rotY = -@float * 0.0174532924f;
			}
			if (num < float2)
			{
				this.rotY = -float2 * 0.0174532924f;
			}
		}

		private void UpdateDir()
		{
			Vector3 forward;
			Game.Character.Utils.Math.ToCartesian(this.rotX, this.rotY, out forward);
			this.UnityCamera.transform.forward = forward;
			this.UnityCamera.transform.position = this.GetEyePos();
			this.activateTimeout -= Time.deltaTime;
			if (this.activateTimeout > 0f)
			{
				this.UnityCamera.transform.LookAt(this.GetEyePos() + this.Target.forward);
			}
		}

		private void UpdateTargetVisibility()
		{
			bool @bool = this.config.GetBool("HideTarget");
			if (@bool != this.targetHide)
			{
				this.targetHide = @bool;
				this.ShowTarget(!this.targetHide);
			}
		}

		private void ShowTarget(bool show)
		{
			Game.Character.Utils.Debug.SetVisible(this.Target.gameObject, show, true);
		}

		public override void Reset()
		{
			this.activateTimeout = 0.1f;
		}

		public override void PostUpdate()
		{
			if (this.disableInput)
			{
				return;
			}
			if (this.InputManager)
			{
				if (this.activateTimeout < 0f)
				{
					this.UpdateTargetVisibility();
				}
				this.UpdateFOV();
				if (this.InputManager.GetInput(InputType.Rotate).Valid)
				{
					this.RotateCamera((Vector2)this.InputManager.GetInput(InputType.Rotate).Value);
				}
				this.UpdateDir();
			}
		}

		private float rotX;

		private float rotY;

		private bool targetHide;

		private float activateTimeout;
	}
}
