using System;
using Game.Character.Config;
using Game.Character.Input;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(RPGConfig))]
	public class RPGCameraMode : CameraMode
	{
		public override Type Type
		{
			get
			{
				return Type.RPG;
			}
		}

		public override void Init()
		{
			base.Init();
			this.UnityCamera.transform.LookAt(this.cameraTarget);
			this.config = base.GetComponent<RPGConfig>();
			Game.Character.Utils.DebugDraw.Enabled = true;
			this.targetFilter = new PositionFilter(10, 1f);
			this.targetFilter.Reset(this.Target.position);
			this.debugRing = RingPrimitive.Create(3f, 3f, 0.1f, 50, Color.red);
			Game.Character.Utils.Debug.SetActive(this.debugRing, this.dbgRing);
			this.config.TransitCallback = new Config.Config.OnTransitMode(this.OnTransitMode);
			this.config.TransitionStartCallback = new Config.Config.OnTransitionStart(this.OnTransitStartMode);
		}

		public override void OnActivate()
		{
			base.OnActivate();
			this.config.SetCameraMode(this.DefaultConfiguration);
			this.targetDistance = this.config.GetFloat("Distance");
			this.cameraTarget = this.Target.position;
			this.targetFilter.Reset(this.Target.position);
			this.targetPos = this.Target.position;
			this.UpdateYAngle();
			this.UpdateXAngle(true);
			this.UpdateDir();
			this.activateTimeout = 2f;
		}

		private void OnTransitMode(string newMode, float t)
		{
			float @float = this.config.GetFloat("Distance");
			this.targetDistance = Mathf.Lerp(this.transitDistance, @float, t);
		}

		private void OnTransitStartMode(string oldMode, string newMode)
		{
			this.transitDistance = this.targetDistance;
		}

		public override void SetCameraTarget(Transform target)
		{
			base.SetCameraTarget(target);
			if (target)
			{
				this.cameraTarget = target.position;
			}
		}

		private void RotateCamera(Vector2 mousePosition)
		{
			if (this.config.GetBool("EnableRotation") && mousePosition.sqrMagnitude > Mathf.Epsilon)
			{
				this.rotX += this.config.GetFloat("RotationSpeed") * mousePosition.x * 0.01f;
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
		}

		private void UpdateConfig()
		{
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

		private Vector3 GetOffsetPos()
		{
			Vector3 vector = this.config.GetVector3("TargetOffset");
			Vector3 a = Game.Character.Utils.Math.VectorXZ(this.UnityCamera.transform.forward);
			Vector3 a2 = Game.Character.Utils.Math.VectorXZ(this.UnityCamera.transform.right);
			Vector3 up = Vector3.up;
			return a * vector.z + a2 * vector.x + up * vector.y;
		}

		private void UpdateDistance()
		{
			this.cameraTarget = this.targetPos + this.GetOffsetPos();
		}

		public override void Reset()
		{
			this.activateTimeout = 0.1f;
			this.targetDistance = this.config.GetFloat("Distance");
			this.cameraTarget = this.Target.position;
			this.targetFilter.Reset(this.Target.position);
			this.targetPos = this.Target.position;
			this.targetZoom = 0f;
			this.UpdateYAngle();
			this.UpdateXAngle(true);
			this.UpdateDir();
		}

		public override void PostUpdate()
		{
			if (this.disableInput)
			{
				return;
			}
			if (this.InputManager)
			{
				this.UpdateConfig();
				this.UpdateFOV();
				if (this.InputManager.GetInput(InputType.Zoom).Valid)
				{
					this.targetZoom = (float)this.InputManager.GetInput(InputType.Zoom).Value;
				}
				this.UpdateZoom();
				this.UpdateYAngle();
				this.UpdateXAngle(false);
				if (this.InputManager.GetInput(InputType.Rotate).Valid)
				{
					this.RotateCamera((Vector2)this.InputManager.GetInput(InputType.Rotate).Value);
				}
				this.UpdateDistance();
				this.UpdateDir();
			}
			this.activateTimeout -= Time.deltaTime;
		}

		public override void FixedStepUpdate()
		{
			this.targetFilter.AddSample(this.Target.position);
			Vector2 vector = this.config.GetVector2("DeadZone");
			if (vector.sqrMagnitude > Mathf.Epsilon)
			{
				RingPrimitive.Generate(this.debugRing, vector.x, vector.y, 0.1f, 50);
				this.debugRing.transform.position = this.targetPos + Vector3.up * 2f;
				this.debugRing.transform.forward = Game.Character.Utils.Math.VectorXZ(this.UnityCamera.transform.forward);
				Game.Character.Utils.Debug.SetActive(this.debugRing, this.dbgRing);
				Vector3 vector2 = this.targetFilter.GetValue() - this.targetPos;
				float magnitude = vector2.magnitude;
				vector2 /= magnitude;
				if (magnitude > vector.x || magnitude > vector.y)
				{
					Vector3 vector3 = this.UnityCamera.transform.InverseTransformDirection(vector2);
					float f = Mathf.Atan2(vector3.x, vector3.z);
					Vector3 vector4 = new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
					Vector3 vector5 = new Vector3(vector4.x * vector.x, 0f, vector4.z * vector.y);
					float magnitude2 = vector5.magnitude;
					if (magnitude > magnitude2)
					{
						Vector3 target = this.targetPos + vector2 * (magnitude - magnitude2);
						this.targetPos = Vector3.SmoothDamp(this.targetPos, target, ref this.springVelocity, this.config.GetFloat("Spring"));
					}
				}
			}
			else
			{
				this.targetPos = Vector3.SmoothDamp(this.targetPos, this.targetFilter.GetValue(), ref this.springVelocity, this.config.GetFloat("Spring"));
				this.targetPos.y = this.targetFilter.GetValue().y;
			}
			this.UpdateCollision();
			base.UpdateTargetDummy();
		}

		public bool dbgRing;

		private float rotX;

		private float rotY;

		private float targetZoom;

		private Vector3 targetPos;

		private PositionFilter targetFilter;

		private Vector3 springVelocity;

		private GameObject debugRing;

		private float transitDistance;

		private float activateTimeout;
	}
}
