using System;
using Game.Character.Config;
using Game.Character.Input;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(RTSConfig))]
	public class RTSCameraMode : CameraMode
	{
		public override Type Type
		{
			get
			{
				return Type.RTS;
			}
		}

		public override void Init()
		{
			base.Init();
			this.UnityCamera.transform.LookAt(this.cameraTarget);
			this.config = base.GetComponent<RTSConfig>();
			Game.Character.Utils.DebugDraw.Enabled = true;
		}

		public override void OnActivate()
		{
			base.OnActivate();
			this.config.SetCameraMode(this.DefaultConfiguration);
			this.targetDistance = this.config.GetFloat("Distance");
			this.groundPlane = new Plane(Vector3.up, this.config.GetFloat("GroundOffset"));
			if (this.Target)
			{
				this.cameraTarget = this.Target.position;
			}
			this.UpdateYAngle();
			this.UpdateXAngle(true);
			this.activateTimeout = 2f;
		}

		public override void SetCameraTarget(Transform target)
		{
			base.SetCameraTarget(target);
			if (target)
			{
				this.cameraTarget = target.position;
			}
		}

		private bool RotateCamera(Vector2 mousePosition)
		{
			if (this.config.GetBool("EnableRotation") && mousePosition.sqrMagnitude > Mathf.Epsilon)
			{
				this.rotX += this.config.GetFloat("RotationSpeed") * mousePosition.x * 0.01f;
				this.updatePanTarget = true;
				return true;
			}
			return false;
		}

		private void DragCamera(Vector2 mousePosition)
		{
			if (this.panning)
			{
				this.UnityCamera.transform.position = this.panCameraPos;
				Ray ray = this.UnityCamera.ScreenPointToRay(mousePosition);
				float d = 0f;
				Vector3 a;
				if (this.groundPlane.Raycast(ray, out d))
				{
					a = ray.origin + ray.direction * d;
				}
				else
				{
					a = ray.origin + ray.direction * this.targetDistance;
				}
				Vector3 b = a - this.panMousePosition;
				b.y = 0f;
				Vector3 vector = this.panCameraTarget - b;
				this.ClampWithinMapBounds(this.cameraTarget, ref vector, true);
				this.dragVelocity = vector - this.cameraTarget;
				this.dragSlowdown = 1f;
				this.cameraTarget = vector;
			}
			else
			{
				this.panCameraTarget = this.cameraTarget;
				this.panCameraPos = this.UnityCamera.transform.position;
				Ray ray2 = this.UnityCamera.ScreenPointToRay(mousePosition);
				Vector3 vector2;
				if (GameInput.FindWaypointPosition(mousePosition, out vector2))
				{
					this.groundPlane.distance = vector2.y;
				}
				float d2 = 0f;
				if (this.groundPlane.Raycast(ray2, out d2))
				{
					this.panMousePosition = ray2.origin + ray2.direction * d2;
				}
				else
				{
					this.panMousePosition = ray2.origin + ray2.direction * this.targetDistance;
				}
				this.panning = true;
			}
		}

		private void UpdatePanTarget()
		{
			if (this.updatePanTarget)
			{
			}
		}

		private void UpdateDragMomentum()
		{
			if (this.dragVelocity.sqrMagnitude > Mathf.Epsilon)
			{
				this.dragSlowdown -= Time.deltaTime;
				if (this.dragSlowdown < 0f)
				{
					this.dragSlowdown = 0f;
				}
				this.dragVelocity *= this.dragSlowdown;
				this.cameraTarget += this.dragVelocity * Time.deltaTime * this.config.GetFloat("DragMomentum") * 100f;
				this.ClampWithinMapBounds(this.cameraTarget, ref this.cameraTarget, true);
			}
			Vector2 vector = this.config.GetVector2("MapCenter");
			Vector2 vector2 = this.config.GetVector2("MapSize");
			float @float = this.config.GetFloat("SoftBorder");
			if (this.cameraTarget.x > vector.x + vector2.x / 2f)
			{
				float num = (this.cameraTarget.x - (vector.x + vector2.x / 2f)) / @float;
				this.cameraTarget.x = this.cameraTarget.x - Time.deltaTime * 40f * num;
			}
			else if (this.cameraTarget.x < vector.x - vector2.x / 2f)
			{
				float num = (-this.cameraTarget.x + vector.x - vector2.x / 2f) / @float;
				this.cameraTarget.x = this.cameraTarget.x + Time.deltaTime * 40f * num;
			}
			if (this.cameraTarget.z > vector.y + vector2.y / 2f)
			{
				float num = (this.cameraTarget.z - (vector.y + vector2.y / 2f)) / @float;
				this.cameraTarget.z = this.cameraTarget.z - Time.deltaTime * 40f * num;
			}
			else if (this.cameraTarget.z < vector.y - vector2.y / 2f)
			{
				float num = (-this.cameraTarget.z + vector.y - vector2.y / 2f) / @float;
				this.cameraTarget.z = this.cameraTarget.z + Time.deltaTime * 40f * num;
			}
		}

		private void MoveCamera(Vector2 move)
		{
			if (move.sqrMagnitude <= Mathf.Epsilon)
			{
				return;
			}
			move *= 0.1f * this.config.GetFloat("MoveSpeed") * base.GetZoomFactor();
			Vector3 forward = this.UnityCamera.transform.forward;
			forward.y = 0f;
			forward.Normalize();
			Vector3 right = this.UnityCamera.transform.right;
			right.y = 0f;
			right.Normalize();
			Vector3 b = forward * move.y + right * move.x;
			Vector3 cameraTarget = this.cameraTarget + b;
			this.ClampWithinMapBounds(this.cameraTarget, ref cameraTarget, false);
			this.cameraTarget = cameraTarget;
		}

		private void MoveCameraByScreenBorder(Vector2 mousePosition)
		{
			Vector2 vector = mousePosition;
			vector.y = (float)Screen.height - vector.y;
			float @float = this.config.GetFloat("ScreenBorderOffset");
			Vector2 zero = Vector2.zero;
			float num = 0f;
			if (vector.x <= @float)
			{
				zero.x = -1f;
				num = 1f - vector.x / @float;
			}
			else if (vector.x >= (float)Screen.width - @float)
			{
				zero.x = 1f;
				num = 1f - ((float)Screen.width - vector.x) / @float;
			}
			if (vector.y >= (float)Screen.height - @float)
			{
				zero.y = -1f;
				num = 1f - ((float)Screen.height - vector.y) / @float;
			}
			else if (vector.y <= @float)
			{
				zero.y = 1f;
				num = 1f - vector.y / @float;
			}
			if (zero.sqrMagnitude > Mathf.Epsilon)
			{
				zero.Normalize();
				num = Mathf.Clamp01(num);
				Vector2 vector2 = zero * Time.deltaTime * num * this.config.GetFloat("ScreenBorderSpeed") * base.GetZoomFactor();
				Vector3 forward = this.UnityCamera.transform.forward;
				forward.y = 0f;
				forward.Normalize();
				Vector3 right = this.UnityCamera.transform.right;
				right.y = 0f;
				right.Normalize();
				Vector3 b = forward * vector2.y + right * vector2.x;
				Vector3 cameraTarget = Vector3.Lerp(this.cameraTarget, this.cameraTarget + b, Time.deltaTime * 50f);
				this.ClampWithinMapBounds(this.cameraTarget, ref cameraTarget, false);
				this.cameraTarget = cameraTarget;
			}
		}

		private void UpdateFOV()
		{
			this.UnityCamera.fieldOfView = this.config.GetFloat("FOV");
		}

		private void UpdateYAngle()
		{
			Game.Character.Utils.Math.ToSpherical(this.UnityCamera.transform.forward, out this.rotX, out this.rotY);
			float num;
			if (this.UnityCamera.orthographic)
			{
				num = (this.UnityCamera.orthographicSize - this.config.GetFloat("OrthoMin")) / (this.config.GetFloat("OrthoMax") - this.config.GetFloat("OrthoMin"));
			}
			else
			{
				num = (this.targetDistance - this.config.GetFloat("DistanceMin")) / (this.config.GetFloat("DistanceMax") - this.config.GetFloat("DistanceMin"));
			}
			float num2 = this.config.GetFloat("AngleZoomMin") * (1f - num) + this.config.GetFloat("AngleY") * num;
			this.rotY = Mathf.Lerp(this.rotY, num2 * -1f * 0.0174532924f, Time.deltaTime * 50f);
		}

		private void UpdateXAngle(bool force)
		{
			if (!this.config.GetBool("EnableRotation") || force || this.activateTimeout > 0f)
			{
				this.rotX = this.config.GetFloat("DefaultAngleX") * -0.0174532924f;
			}
		}

		private void UpdateDir()
		{
			Vector3 forward;
			Game.Character.Utils.Math.ToCartesian(this.rotX, this.rotY, out forward);
			this.UnityCamera.transform.forward = forward;
			this.UnityCamera.transform.position = this.cameraTarget - this.UnityCamera.transform.forward * this.targetDistance;
			this.UpdatePanTarget();
		}

		private void UpdateConfig()
		{
		}

		private void UpdateDistance()
		{
			if (this.Target && this.config.GetBool("FollowTargetY"))
			{
				this.cameraTarget.y = this.Target.position.y;
			}
		}

		private void UpdateZoom()
		{
			Game.Character.Utils.Math.ToSpherical(this.UnityCamera.transform.forward, out this.rotX, out this.rotY);
			if (Mathf.Abs(this.targetZoom) > Mathf.Epsilon)
			{
				float num = this.targetZoom * 20f * Time.deltaTime;
				if (Mathf.Abs(num) > Mathf.Abs(this.targetZoom))
				{
					num = this.targetZoom;
				}
				this.Zoom(num);
				this.targetZoom -= num;
				this.updatePanTarget = true;
			}
		}

		private bool IsInMapBounds(Vector3 point)
		{
			Game.Character.Utils.Math.Swap<float>(ref point.y, ref point.z);
			Vector2 vector = this.config.GetVector2("MapCenter");
			Vector2 vector2 = this.config.GetVector2("MapSize");
			Rect rect = new Rect(vector.x - vector2.x / 2f, vector.y - vector2.y / 2f, vector2.x, vector2.y);
			return rect.Contains(point);
		}

		private void ClampWithinMapBounds(Vector3 currTarget, ref Vector3 point, bool border)
		{
			Vector2 vector = this.config.GetVector2("MapCenter");
			Vector2 vector2 = this.config.GetVector2("MapSize");
			if (this.config.GetBool("DisableHorizontal"))
			{
				point.x = currTarget.x;
			}
			if (this.config.GetBool("DisableVertical"))
			{
				point.z = currTarget.z;
			}
			float num = this.config.GetFloat("SoftBorder");
			if (!border)
			{
				num = 0f;
			}
			if (point.x > vector.x + vector2.x / 2f + num)
			{
				point.x = vector.x + vector2.x / 2f + num;
			}
			else if (point.x < vector.x - vector2.x / 2f - num)
			{
				point.x = vector.x - vector2.x / 2f - num;
			}
			if (point.z > vector.y + vector2.y / 2f + num)
			{
				point.z = vector.y + vector2.y / 2f + num;
			}
			else if (point.z < vector.y - vector2.y / 2f - num)
			{
				point.z = vector.y - vector2.y / 2f - num;
			}
		}

		public void Zoom(float amount)
		{
			if (!this.config.GetBool("EnableZoom"))
			{
				return;
			}
			float num = amount * this.config.GetFloat("ZoomSpeed");
			if (Mathf.Abs(num) > Mathf.Epsilon)
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
					this.UnityCamera.orthographicSize = Mathf.Clamp(this.UnityCamera.orthographicSize, this.config.GetFloat("OrthoMin"), this.config.GetFloat("OrthoMax"));
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
				this.targetDistance = Mathf.Clamp(this.targetDistance, this.config.GetFloat("DistanceMin"), this.config.GetFloat("DistanceMax"));
			}
		}

		public override void Reset()
		{
			this.activateTimeout = 0.1f;
			this.targetZoom = 0f;
			this.targetDistance = this.config.GetFloat("Distance");
			if (this.Target)
			{
				this.cameraTarget = this.Target.position;
			}
		}

		public override void PostUpdate()
		{
			if (this.disableInput)
			{
				return;
			}
			if (this.InputManager)
			{
				this.updatePanTarget = false;
				this.UpdateConfig();
				this.UpdateDistance();
				this.UpdateFOV();
				if (this.InputManager.GetInput(InputType.Zoom).Valid)
				{
					this.targetZoom = (float)this.InputManager.GetInput(InputType.Zoom).Value;
				}
				this.UpdateZoom();
				this.UpdateYAngle();
				this.UpdateXAngle(false);
				bool flag = false;
				if (this.InputManager.GetInput(InputType.Rotate).Valid)
				{
					this.RotateCamera((Vector2)this.InputManager.GetInput(InputType.Rotate).Value);
					flag = true;
				}
				if (!flag)
				{
					if (this.config.GetBool("DraggingMove"))
					{
						if (this.InputManager.GetInput(InputType.Pan).Valid)
						{
							this.DragCamera((Vector2)this.InputManager.GetInput(InputType.Pan).Value);
						}
						else
						{
							this.UpdateDragMomentum();
							this.panning = false;
						}
					}
					if (!this.panning)
					{
						if (this.config.GetBool("KeyMove") && this.InputManager.GetInput(InputType.Move).Valid)
						{
							this.MoveCamera((Vector2)this.InputManager.GetInput(InputType.Move).Value);
						}
						if (this.config.GetBool("ScreenBorderMove"))
						{
							this.MoveCameraByScreenBorder(UnityEngine.Input.mousePosition);
						}
					}
				}
				this.UpdateDir();
			}
			this.activateTimeout -= Time.deltaTime;
		}

		private void UpdateCollision()
		{
			if (this.collision)
			{
				float num;
				float num2;
				this.collision.ProcessCollision(this.cameraTarget, this.cameraTarget, this.UnityCamera.transform.forward, this.targetDistance, out num, out num2);
			}
		}

		public override void FixedStepUpdate()
		{
			this.UpdateCollision();
		}

		private float rotX;

		private float rotY;

		private float targetZoom;

		private Plane groundPlane;

		private bool panning;

		private Vector3 panMousePosition;

		private Vector3 panCameraTarget;

		private Vector3 panCameraPos;

		private float activateTimeout;

		private Vector3 dragVelocity;

		private float dragSlowdown;

		private bool updatePanTarget;
	}
}
