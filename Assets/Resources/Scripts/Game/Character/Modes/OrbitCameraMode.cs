using System;
using Game.Character.Config;
using Game.Character.Input;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(OrbitConfig))]
	public class OrbitCameraMode : CameraMode
	{
		public override Type Type
		{
			get
			{
				return Type.Orbit;
			}
		}

		public override void OnActivate()
		{
			base.OnActivate();
			if (this.Target)
			{
				this.cameraTarget = this.Target.position;
				this.newTarget = this.Target.position;
				this.interpolateTime = -0.1f;
				this.UnityCamera.transform.LookAt(this.cameraTarget);
				this.targetDistance = (this.UnityCamera.transform.position - this.cameraTarget).magnitude;
				this.panValid = false;
				this.RotateCamera(Vector2.one * 1f);
				this.lastPanPosition = Vector2.zero;
			}
			else
			{
				Ray ray = new Ray(this.UnityCamera.transform.position, this.UnityCamera.transform.forward);
				Vector3 vector = ray.origin + ray.direction * 100f;
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, 3.40282347E+38f))
				{
					vector = raycastHit.point;
				}
				this.newTarget = vector;
				this.cameraTarget = this.newTarget;
				this.targetDistance = (this.UnityCamera.transform.position - this.cameraTarget).magnitude;
				this.RotateCamera(Vector2.zero * 0.01f);
				this.lastPanPosition = Vector2.zero;
			}
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
				this.newTarget = this.Target.position;
				this.interpolateTime = 0.1f;
				this.UnityCamera.transform.LookAt(this.cameraTarget);
				this.targetDistance = (this.UnityCamera.transform.position - this.cameraTarget).magnitude;
				this.panValid = false;
				this.RotateCamera(Vector2.zero * 0.01f);
				this.lastPanPosition = Vector2.zero;
			}
		}

		public override void Init()
		{
			base.Init();
			this.UnityCamera.transform.LookAt(this.cameraTarget);
			this.config = base.GetComponent<OrbitConfig>();
		}

		public void ZoomCamera(float amount)
		{
			if (this.config.GetBool("DisableZoom"))
			{
				return;
			}
			float num = amount * this.config.GetFloat("ZoomSpeed");
			if (System.Math.Abs(num) > Mathf.Epsilon)
			{
				if (this.UnityCamera.orthographic)
				{
					float zoomFactor = base.GetZoomFactor();
					num *= zoomFactor;
					this.UnityCamera.orthographicSize -= num;
					if (this.UnityCamera.orthographicSize < 0.01f)
					{
						this.UnityCamera.orthographicSize = 0.01f;
					}
				}
				else
				{
					float num2 = base.GetZoomFactor();
					if (num2 < 0.01f)
					{
						num2 = 0.01f;
					}
					num *= num2;
					Vector3 b = this.UnityCamera.transform.forward * num;
					Vector3 vector = this.UnityCamera.transform.position + b;
					Plane plane = new Plane(this.UnityCamera.transform.forward, this.cameraTarget);
					if (!plane.GetSide(vector))
					{
						this.UnityCamera.transform.position = vector;
					}
				}
				this.targetDistance = (this.UnityCamera.transform.position - this.cameraTarget).magnitude;
			}
		}

		public void PanCamera(Vector2 mousePosition)
		{
			if (this.config.GetBool("DisablePan"))
			{
				return;
			}
			if (this.panValid)
			{
				Vector2 a = mousePosition - this.lastPanPosition;
				this.lastPanPosition = mousePosition;
				a *= 0.01f * this.config.GetFloat("PanSpeed") * base.GetZoomFactor();
				Vector3 delta = -this.UnityCamera.transform.up * a.y + -this.UnityCamera.transform.right * a.x;
				this.PanCameraDelta(delta);
			}
			else
			{
				this.lastPanPosition = mousePosition;
				this.panValid = true;
			}
		}

		public void PanCameraWithMove(Vector2 move)
		{
			if (move.sqrMagnitude <= Mathf.Epsilon || this.config.GetBool("DisablePan"))
			{
				return;
			}
			move *= 0.1f * this.config.GetFloat("PanSpeed") * base.GetZoomFactor();
			Vector3 delta = this.UnityCamera.transform.up * move.y + this.UnityCamera.transform.right * move.x;
			this.PanCameraDelta(delta);
		}

		private void PanCameraDelta(Vector3 delta)
		{
			if (this.config.GetBool("DragLimits"))
			{
				Vector2 vector = this.config.GetVector2("DragLimitX");
				Vector2 vector2 = this.config.GetVector2("DragLimitY");
				Vector2 vector3 = this.config.GetVector2("DragLimitZ");
				Vector3 position = this.UnityCamera.transform.position;
				position.x = Mathf.Clamp(position.x + delta.x, vector.x, vector.y);
				position.y = Mathf.Clamp(position.y + delta.y, vector2.x, vector2.y);
				position.z = Mathf.Clamp(position.z + delta.z, vector3.x, vector3.y);
				this.cameraTarget += this.UnityCamera.transform.position - position;
				this.UnityCamera.transform.position = position;
			}
			else
			{
				this.UnityCamera.transform.position += delta;
				this.cameraTarget += delta;
			}
		}

		public void RotateCamera(Vector2 mousePosition)
		{
			if (this.config.GetBool("DisableRotation"))
			{
				return;
			}
			if (!this.panValid)
			{
				Vector3 right = this.UnityCamera.transform.right;
				this.UnityCamera.transform.RotateAround(this.cameraTarget, right, this.config.GetFloat("RotationSpeedY") * -mousePosition.y);
				float @float = this.config.GetFloat("RotationYMax");
				float float2 = this.config.GetFloat("RotationYMin");
				float floatMax = this.config.GetFloatMax("RotationYMax");
				float floatMin = this.config.GetFloatMin("RotationYMin");
				if (@float < floatMax || float2 > floatMin)
				{
					float rotX;
					float num;
					Game.Character.Utils.Math.ToSpherical(this.UnityCamera.transform.forward, out rotX, out num);
					float num2 = -num * 57.29578f;
					bool flag = false;
					float rotZ = 0f;
					if (@float < floatMax && num2 > @float)
					{
						rotZ = -@float * 0.0174532924f;
						flag = true;
					}
					if (float2 > floatMin && num2 < float2)
					{
						rotZ = -float2 * 0.0174532924f;
						flag = true;
					}
					if (flag)
					{
						Vector3 forward;
						Game.Character.Utils.Math.ToCartesian(rotX, rotZ, out forward);
						this.UnityCamera.transform.forward = forward;
						this.UnityCamera.transform.position = this.cameraTarget - this.UnityCamera.transform.forward * this.targetDistance;
					}
				}
				Vector3 up = Vector3.up;
				this.UnityCamera.transform.RotateAround(this.cameraTarget, up, this.config.GetFloat("RotationSpeedX") * mousePosition.x);
				Vector3 eulerAngles = this.UnityCamera.transform.eulerAngles;
				this.UnityCamera.transform.rotation = Quaternion.Euler(eulerAngles);
				this.UnityCamera.transform.position = this.cameraTarget - this.UnityCamera.transform.forward * this.targetDistance;
			}
		}

		public void ResetCamera(Vector2 position)
		{
			Ray ray = this.UnityCamera.ScreenPointToRay(position);
			Vector3 vector = ray.origin + ray.direction * 100f;
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 3.40282347E+38f))
			{
				vector = raycastHit.point;
			}
			this.newTarget = vector;
			this.interpolateTime = this.config.GetFloat("TargetInterpolation");
		}

		private void InterpolateTarget()
		{
			this.interpolateTime -= Time.deltaTime;
			this.cameraTarget = Vector3.Lerp(this.cameraTarget, this.newTarget, Time.deltaTime * 10f);
			this.UnityCamera.transform.position = this.cameraTarget - this.UnityCamera.transform.forward * this.targetDistance;
			this.targetDistance = (this.UnityCamera.transform.position - this.cameraTarget).magnitude;
		}

		public override void PostUpdate()
		{
			if (this.disableInput)
			{
				return;
			}
			if (this.interpolateTime >= 0f)
			{
				this.InterpolateTarget();
				return;
			}
			if (this.InputManager)
			{
				this.UpdateFOV();
				if (this.InputManager.GetInput(InputType.Pan).Valid)
				{
					this.PanCamera((Vector2)this.InputManager.GetInput(InputType.Pan).Value);
				}
				else
				{
					if (this.InputManager.GetInput(InputType.Move).Valid)
					{
						this.PanCameraWithMove((Vector2)this.InputManager.GetInput(InputType.Move).Value);
					}
					this.panValid = false;
				}
				if (this.InputManager.GetInput(InputType.Zoom).Valid)
				{
					this.ZoomCamera((float)this.InputManager.GetInput(InputType.Zoom).Value);
				}
				if (this.InputManager.GetInput(InputType.Rotate).Valid)
				{
					this.RotateCamera((Vector2)this.InputManager.GetInput(InputType.Rotate).Value);
				}
				Game.Character.Input.Input input = this.InputManager.GetInput(InputType.Reset);
				if (input.Valid && (bool)input.Value)
				{
					this.ResetCamera(UnityEngine.Input.mousePosition);
				}
			}
		}

		private Vector2 lastPanPosition;

		private bool panValid;

		private Vector3 newTarget;

		private float interpolateTime;
	}
}
