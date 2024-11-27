using System;
using Game.Character.CharacterController;
using Game.Character.Config;
using Game.Character.Input;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(ThirdPersonConfig))]
	public class ThirdPersonCameraMode : CameraMode
	{
		public override Type Type
		{
			get
			{
				return Type.ThirdPerson;
			}
		}

		public override void Init()
		{
			base.Init();
			if (this.LerpFromLastPos)
			{
				this.targetPos = this.Target.position;
			}
			this.UnityCamera.transform.LookAt(this.cameraTarget);
			this.config = base.GetComponent<ThirdPersonConfig>();
			this.lastTargetPos = this.Target.position;
			this.targetVelocity = 0f;
			if (this.dbgRing)
			{
				this.debugRing = RingPrimitive.Create(3f, 3f, 0.1f, 50, Color.red);
				Game.Character.Utils.Debug.SetActive(this.debugRing, this.dbgRing);
			}
			this.targetFilter = new PositionFilter(10, 1f);
			this.targetFilter.Reset(this.Target.position);
			Game.Character.Utils.DebugDraw.Enabled = true;
			this.resetTimeout = 0f;
		}

		public override void OnActivate()
		{
			base.OnActivate();
			this.config.SetCameraMode(this.DefaultConfiguration);
			this.targetVelocity = 0f;
			this.activateTimeout = 1f;
		}

		private void RotateCamera(Vector2 mousePosition)
		{
			this.rotationInput = (mousePosition.sqrMagnitude > Mathf.Epsilon);
			this.rotationInputTimeout += Time.unscaledDeltaTime;
			if (this.rotationInput)
			{
				this.rotationInputTimeout = 0f;
				this.hasRotated = true;
			}
			Game.Character.Utils.Math.ToSpherical(this.UnityCamera.transform.forward, out this.rotX, out this.rotY);
			this.rotY += this.config.GetFloat("RotationSpeedY") * mousePosition.y;
			this.rotX += this.config.GetFloat("RotationSpeedX") * mousePosition.x;
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

		private void UpdateFollow()
		{
			Vector3 vector = this.targetPos - this.lastTargetPos;
			vector.y = 0f;
			float num = Mathf.Clamp(vector.magnitude, 0f, 5f);
			if (Time.unscaledDeltaTime > Mathf.Epsilon)
			{
				this.targetVelocity = num / Time.unscaledDeltaTime;
			}
			else
			{
				this.targetVelocity = 0f;
			}
			if (this.InputManager.GetInput(InputType.Move).Valid)
			{
				Vector2 vector2 = (Vector2)this.InputManager.GetInput(InputType.Move).Value;
				vector2.Normalize();
				float @float = this.config.GetFloat("FollowCoef");
				float f = Mathf.Atan2(vector2.x, vector2.y);
				float num2 = Mathf.Sin(f);
				float num3 = Mathf.Clamp01(this.rotationInputTimeout);
				float num4 = num2 * Time.unscaledDeltaTime * @float * this.targetVelocity * 0.2f * num3;
				this.rotX += num4;
			}
		}

		private void UpdateDistance()
		{
			Vector3 a = this.targetPos + this.GetOffsetPos();
			this.cameraTarget = Vector3.Lerp(a, base.GetTargetHeadPos(), 1f - this.currCollisionTargetDist);
		}

		private void UpdateFOV()
		{
			this.UnityCamera.fieldOfView = this.config.GetFloat("FOV");
		}

		private void UpdateDir()
		{
			this.activateTimeout -= Time.unscaledDeltaTime;
			if (this.activateTimeout > 0f)
			{
				float @float = this.config.GetFloat("DefaultYRotation");
				this.rotY = -@float * 0.0174532924f;
				this.rotX = Mathf.Atan2(this.Target.forward.x, this.Target.forward.z);
			}
			Vector3 forward;
			Game.Character.Utils.Math.ToCartesian(this.rotX, this.rotY, out forward);
			this.UpdateAutoReset(ref forward);
			this.UnityCamera.transform.forward = forward;
			this.UnityCamera.transform.position = this.cameraTarget - this.UnityCamera.transform.forward * this.targetDistance;
			this.lastTargetPos = this.targetPos;
		}

		private void UpdateAutoReset(ref Vector3 dir)
		{
			if (this.AutoReset)
			{
				this.resetTimeout -= Time.unscaledDeltaTime;
				if (this.rotationInputTimeout < 0.1f)
				{
					this.resetTimeout = this.AutoResetTimeout;
				}
				if (this.resetTimeout < 0f && this.hasRotated)
				{
					float @float = this.config.GetFloat("DefaultYRotation");
					float num = -@float * 0.0174532924f;
					float num2 = Mathf.Atan2(this.Target.forward.x, this.Target.forward.z);
					if (Mathf.Abs(num2 - this.rotX) < 0.1f && Mathf.Abs(num - this.rotY) < 0.1f)
					{
						this.hasRotated = false;
					}
					Vector3 toDirection;
					Game.Character.Utils.Math.ToCartesian(num2, num, out toDirection);
					Quaternion b = Quaternion.FromToRotation(dir, toDirection);
					Quaternion rotation = Quaternion.Slerp(Quaternion.identity, b, Time.unscaledDeltaTime * this.AutoResetSpeed * 10f);
					dir = rotation * dir;
				}
			}
		}

		private Vector3 GetOffsetPos()
		{
			Vector3 vector = Vector3.zero;
			if (this.config.IsVector3("TargetOffset"))
			{
				vector = this.config.GetVector3("TargetOffset");
			}
			Vector3 a = Game.Character.Utils.Math.VectorXZ(this.UnityCamera.transform.forward);
			Vector3 a2 = Game.Character.Utils.Math.VectorXZ(this.UnityCamera.transform.right);
			Vector3 up = Vector3.up;
			return a * vector.z + a2 * vector.x + up * vector.y;
		}

		private void UpdateYRotation()
		{
			if (!this.rotationInput && this.targetVelocity > 0.1f)
			{
				float num = -this.rotY * 57.29578f;
				float @float = this.config.GetFloat("DefaultYRotation");
				float num2 = Mathf.Clamp01(this.rotationInputTimeout);
				float t = Mathf.Clamp01(this.targetVelocity * this.config.GetFloat("AutoYRotation") * Time.unscaledDeltaTime) * num2;
				num = Mathf.Lerp(num, @float, t);
				this.rotY = -num * 0.0174532924f;
			}
		}

		public override void Reset()
		{
			this.activateTimeout = 1f;
		}

		public void UpdateAutoAim()
		{
			if (!TargetManager.Instance.UseAutoAim)
			{
				return;
			}
			Transform autoAimTarget = TargetManager.Instance.AutoAimTarget;
			Vector3 targetLocalOffset = TargetManager.Instance.TargetLocalOffset;
			if (autoAimTarget && !autoAimTarget.gameObject.activeSelf)
			{
				return;
			}
			if (this.currentAutoAimPerformed)
			{
				if (!this.InputManager.GetInput(InputType.Aim).Valid || !(bool)this.InputManager.GetInput(InputType.Aim).Value || autoAimTarget == null)
				{
					this.currentAutoAimPerformed = false;
					this.currentAutoAimDuration = 0f;
				}
			}
			else if (this.InputManager.GetInput(InputType.Aim).Valid && (bool)this.InputManager.GetInput(InputType.Aim).Value && autoAimTarget != null)
			{
				this.UnityCamera.transform.forward = Vector3.Lerp(this.UnityCamera.transform.forward, autoAimTarget.position + targetLocalOffset - this.UnityCamera.transform.position, this.AutoAimSensitivity * Time.unscaledDeltaTime);
				this.currentAutoAimDuration += Time.unscaledDeltaTime;
				if (this.currentAutoAimDuration >= this.AutoAimDuration)
				{
					this.currentAutoAimPerformed = true;
				}
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
				if (this.InputManager.GetInput<bool>(InputType.Reset, false))
				{
					this.activateTimeout = 0.1f;
				}
				this.UpdateFOV();
				if (this.InputManager.GetInput(InputType.Rotate).Valid)
				{
					this.RotateCamera((Vector2)this.InputManager.GetInput(InputType.Rotate).Value);
				}
				this.UpdateFollow();
				this.UpdateDistance();
				this.UpdateYRotation();
				this.UpdateDir();
				this.UpdateAutoAim();
			}
		}

		private void UpdateCollision()
		{
			Vector3 cameraTarget = this.targetPos + this.GetOffsetPos();
			float @float = this.config.GetFloat("Distance");
			this.collision.ProcessCollision(cameraTarget, base.GetTargetHeadPos(), this.UnityCamera.transform.forward, @float, out this.collisionTargetDist, out this.collisionDistance);
			float num = this.collisionDistance / @float;
			if (this.collisionTargetDist > num)
			{
				this.collisionTargetDist = num;
			}
			this.targetDistance = Interpolation.Lerp(this.targetDistance, this.collisionDistance, (this.targetDistance <= this.collisionDistance) ? this.collision.GetReturnSpeed() : this.collision.GetClipSpeed());
			this.currCollisionTargetDist = Mathf.SmoothDamp(this.currCollisionTargetDist, this.collisionTargetDist, ref this.collisionTargetVelocity, (this.currCollisionTargetDist <= this.collisionTargetDist) ? this.collision.GetReturnTargetSpeed() : this.collision.GetTargetClipSpeed());
		}

		public override void GameUpdate()
		{
			base.GameUpdate();
			float @float = this.config.GetFloat("Spring");
			Vector2 vector = this.config.GetVector2("DeadZone");
			if (@float <= 0f && vector.sqrMagnitude <= Mathf.Epsilon)
			{
				this.targetPos = this.targetFilter.GetValue();
			}
			base.UpdateTargetDummy();
		}

		public override void FixedStepUpdate()
		{
			this.targetFilter.AddSample(this.Target.position);
			this.UpdateCollision();
			Vector2 vector = this.config.GetVector2("DeadZone");
			if (vector.sqrMagnitude > Mathf.Epsilon)
			{
				if (this.dbgRing)
				{
					RingPrimitive.Generate(this.debugRing, vector.x, vector.y, 0.1f, 50);
					this.debugRing.transform.position = this.targetPos + Vector3.up * 2f;
					Vector3 forward = Game.Character.Utils.Math.VectorXZ(this.UnityCamera.transform.forward);
					if (forward.sqrMagnitude < Mathf.Epsilon)
					{
						forward = Vector3.forward;
					}
					this.debugRing.transform.forward = forward;
					Game.Character.Utils.Debug.SetActive(this.debugRing, this.dbgRing);
				}
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
			}
		}

		public float AutoResetTimeout = 1f;

		public float AutoResetSpeed = 1f;

		public bool AutoReset = true;

		public bool LerpFromLastPos = true;

		public bool dbgRing;

		private bool rotationInput;

		private float rotationInputTimeout;

		private bool hasRotated;

		private float rotX;

		private float rotY;

		private float targetVelocity;

		private float collisionDistance;

		private float collisionZoomVelocity;

		private float currCollisionTargetDist;

		private float collisionTargetDist;

		private float collisionTargetVelocity;

		private Vector3 targetPos;

		private Vector3 lastTargetPos;

		private Vector3 springVelocity;

		private GameObject debugRing;

		private float activateTimeout;

		private float resetTimeout;

		private PositionFilter targetFilter;

		public float AutoAimSensitivity = 1f;

		public float AutoAimDuration = 0.2f;

		private float currentAutoAimDuration;

		private bool currentAutoAimPerformed;
	}
}
