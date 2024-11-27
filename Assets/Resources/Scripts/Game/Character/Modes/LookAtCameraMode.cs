using System;
using Game.Character.Config;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(LookAtConfig))]
	public class LookAtCameraMode : CameraMode
	{
		public override Type Type
		{
			get
			{
				return Type.LookAt;
			}
		}

		public override void Init()
		{
			base.Init();
			this.UnityCamera.transform.LookAt(this.cameraTarget);
			this.config = base.GetComponent<LookAtConfig>();
			this.targetTimeout = -1f;
			this.targetTimeoutMax = 1f;
		}

		public override void OnActivate()
		{
			this.ApplyCurrentCamera();
		}

		public void RegisterFinishCallback(LookAtCameraMode.OnLookAtFinished callback)
		{
			this.finishedCallback = (LookAtCameraMode.OnLookAtFinished)Delegate.Combine(this.finishedCallback, callback);
		}

		public void UnregisterFinishCallback(LookAtCameraMode.OnLookAtFinished callback)
		{
			this.finishedCallback = (LookAtCameraMode.OnLookAtFinished)Delegate.Remove(this.finishedCallback, callback);
		}

		public void ApplyCurrentCamera()
		{
			Ray ray = new Ray(this.UnityCamera.transform.position, this.UnityCamera.transform.forward);
			Vector3 cameraTarget = ray.origin + ray.direction * 100f;
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 3.40282347E+38f))
			{
				cameraTarget = raycastHit.point;
			}
			this.cameraTarget = cameraTarget;
			this.targetDistance = (this.UnityCamera.transform.position - this.cameraTarget).magnitude;
		}

		public void LookAt(Vector3 point, float timeout)
		{
			this.LookAt(this.UnityCamera.transform.position, point, timeout);
		}

		public void LookAt(Vector3 from, Vector3 point, float timeout)
		{
			this.oldTarget = this.cameraTarget;
			this.oldRot = this.UnityCamera.transform.rotation;
			this.newTarget = point;
			if (timeout < 0f)
			{
				timeout = 0f;
			}
			this.newRot = Quaternion.LookRotation(point - from);
			this.targetTimeoutMax = timeout;
			this.targetTimeout = timeout;
		}

		public void LookFrom(Vector3 from, float timeout)
		{
			this.LookAt(from, this.cameraTarget, timeout);
		}

		public override void FixedStepUpdate()
		{
			this.LookAt(this.Target.position + Vector3.up, 0.1f);
		}

		private void UpdateLookAt()
		{
			if (this.targetTimeout >= 0f)
			{
				this.targetTimeout -= Time.deltaTime;
				float t;
				if (this.targetTimeoutMax < Mathf.Epsilon)
				{
					t = 1f;
				}
				else
				{
					t = 1f - this.targetTimeout / this.targetTimeoutMax;
				}
				if (this.config.GetBool("InterpolateTarget"))
				{
					this.cameraTarget = Vector3.Lerp(this.oldTarget, this.newTarget, Time.deltaTime * 2f);
					this.UnityCamera.transform.LookAt(this.cameraTarget);
				}
				else
				{
					Quaternion b = Quaternion.Slerp(this.oldRot, this.newRot, Interpolation.LerpS(0f, 1f, t));
					this.UnityCamera.transform.rotation = Quaternion.Slerp(this.UnityCamera.transform.rotation, b, Time.deltaTime);
				}
				if (this.targetTimeout < 0f && this.finishedCallback != null)
				{
					this.finishedCallback();
				}
			}
		}

		private void UpdateFOV()
		{
			this.UnityCamera.fieldOfView = this.config.GetFloat("FOV");
		}

		public override void PostUpdate()
		{
			this.UpdateFOV();
			this.UpdateLookAt();
		}

		private Vector3 newTarget;

		private Vector3 oldTarget;

		private Quaternion oldRot;

		private Quaternion newRot;

		private float targetTimeoutMax;

		private float targetTimeout;

		private LookAtCameraMode.OnLookAtFinished finishedCallback;

		public delegate void OnLookAtFinished();
	}
}
