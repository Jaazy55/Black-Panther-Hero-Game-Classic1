using System;
using Game.Character.Config;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(DeadConfig))]
	public class DeadCameraMode : CameraMode
	{
		public override Type Type
		{
			get
			{
				return Type.Dead;
			}
		}

		public override void Init()
		{
			base.Init();
			this.UnityCamera.transform.LookAt(this.cameraTarget);
			this.config = base.GetComponent<DeadConfig>();
		}

		public override void OnActivate()
		{
			base.OnActivate();
			this.targetDistance = (this.cameraTarget - this.UnityCamera.transform.position).magnitude;
		}

		private void RotateCamera()
		{
			Game.Character.Utils.Math.ToSpherical(this.UnityCamera.transform.forward, out this.rotX, out this.rotY);
			this.angle = this.config.GetFloat("RotationSpeed") * Time.deltaTime;
			this.rotY = -this.config.GetFloat("Angle") * 0.0174532924f;
			this.rotX += this.angle;
		}

		private void UpdateFOV()
		{
			this.UnityCamera.fieldOfView = this.config.GetFloat("FOV");
		}

		private Vector3 GetOffsetPos()
		{
			Vector3 vector = this.config.GetVector3("TargetOffset");
			Vector3 a = Game.Character.Utils.Math.VectorXZ(this.UnityCamera.transform.forward);
			Vector3 a2 = Game.Character.Utils.Math.VectorXZ(this.UnityCamera.transform.right);
			Vector3 up = Vector3.up;
			return a * vector.z + a2 * vector.x + up * vector.y;
		}

		public override void PostUpdate()
		{
			this.UpdateFOV();
			this.RotateCamera();
			if (this.config.GetBool("Collision"))
			{
				this.UpdateCollision();
			}
			this.UpdateDir();
		}

		private void UpdateDir()
		{
			Vector3 forward;
			Game.Character.Utils.Math.ToCartesian(this.rotX, this.rotY, out forward);
			this.UnityCamera.transform.forward = forward;
			this.cameraTarget = this.Target.position + this.GetOffsetPos();
			this.UnityCamera.transform.position = this.cameraTarget - this.UnityCamera.transform.forward * this.targetDistance;
		}

		private void UpdateCollision()
		{
			float @float = this.config.GetFloat("Distance");
			float num;
			float num2;
			this.collision.ProcessCollision(this.cameraTarget, base.GetTargetHeadPos(), this.UnityCamera.transform.forward, @float, out num, out num2);
			this.targetDistance = Interpolation.Lerp(this.targetDistance, num2, (this.targetDistance <= num2) ? this.collision.GetReturnSpeed() : this.collision.GetClipSpeed());
		}

		private float rotX;

		private float rotY;

		private float angle;
	}
}
