using System;
using Game.Character.Config;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(DebugConfig))]
	public class DebugCameraMode : CameraMode
	{
		public override Type Type
		{
			get
			{
				return Type.Debug;
			}
		}

		public override void OnActivate()
		{
			base.OnActivate();
			if (this.Target)
			{
				this.cameraTarget = this.Target.position;
				this.RotateCamera(Vector2.zero);
			}
		}

		public override void Init()
		{
			base.Init();
			this.UnityCamera.transform.LookAt(this.cameraTarget);
			this.config = base.GetComponent<DebugConfig>();
		}

		private void UpdateFOV()
		{
			this.UnityCamera.fieldOfView = this.config.GetFloat("FOV");

		}

		public void RotateCamera(Vector2 mousePosition)
		{
			Game.Character.Utils.Math.ToSpherical(this.UnityCamera.transform.forward, out this.rotX, out this.rotY);
			this.rotY += this.config.GetFloat("RotationSpeedY") * mousePosition.y * 0.01f;
			this.rotX += this.config.GetFloat("RotationSpeedX") * mousePosition.x * 0.01f;
		}

		public void MoveCamera()
		{
			Vector3 a = Vector3.zero;
			if (UnityEngine.Input.GetKey(KeyCode.W))
			{
				a += this.UnityCamera.transform.forward;
			}
			if (UnityEngine.Input.GetKey(KeyCode.S))
			{
				a += -this.UnityCamera.transform.forward;
			}
			if (UnityEngine.Input.GetKey(KeyCode.A))
			{
				a += -this.UnityCamera.transform.right;
			}
			if (UnityEngine.Input.GetKey(KeyCode.D))
			{
				a += this.UnityCamera.transform.right;
			}
			a.Normalize();
			this.UnityCamera.transform.position += a * this.config.GetFloat("MoveSpeed") * Time.deltaTime * 10f;
		}

		private void UpdateDir()
		{
			Vector3 forward;
			Game.Character.Utils.Math.ToCartesian(this.rotX, this.rotY, out forward);
			this.UnityCamera.transform.forward = forward;
		}

		public override void PostUpdate()
		{
			this.UpdateFOV();
			if (CursorLocking.IsLocked)
			{
				this.RotateCamera(new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y")));
			}
			this.MoveCamera();
			this.UpdateDir();
		}

		private float rotX;

		private float rotY;
	}
}
