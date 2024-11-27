using System;
using Game.Character.Config;
using Game.Character.Input;
using Game.Character.Modes;
using Game.Character.Utils;
using UnityEngine;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(CarConfig))]
	public class VehicleCameraMode : CameraMode
	{
		public override Game.Character.Modes.Type Type
		{
			get
			{
				return Game.Character.Modes.Type.ThirdPersonVehicle;
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
			this.config = base.GetComponent<CarConfig>();
			this.lastTargetPos = this.Target.position;
			this.targetVelocity = Vector3.zero;
			if (this.dbgRing)
			{
				this.debugRing = RingPrimitive.Create(3f, 3f, 0.1f, 50, Color.red);
			}
			this.targetFilter = new PositionFilter(10, 1f);
			this.targetFilter.Reset(this.Target.position);
			this.velocityFilter = new PositionFilter(10, 1f);
			this.velocityFilter.Reset(Vector3.zero);
			Game.Character.Utils.DebugDraw.Enabled = true;
			this.resetTimeout = 0f;
		}

		public override void OnActivate()
		{
			base.OnActivate();
			this.config.SetCameraMode(this.DefaultConfiguration);
			this.targetVelocity = Vector3.zero;
			this.activateTimeout = 1f;
		}

		private void RotateCamera()
		{
			Vector2 vector = new Vector2(Controls.GetAxis("Horizontal_R"), Controls.GetAxis("Vertical_R"));
			this.rotationInput = (vector.sqrMagnitude > Mathf.Epsilon);
			this.rotationInputTimeout += Time.deltaTime;
			if (this.rotationInput)
			{
				this.rotationInputTimeout = 0f;
				this.autoRotSpeed = this.config.GetFloat("AutoTurnMinSpeed");
			}
			Game.Character.Utils.Math.ToSpherical(this.UnityCamera.transform.forward, out this.rotX, out this.rotY);
			this.rotY += this.config.GetFloat("RotationSpeedY") * vector.y;
			this.rotX += this.config.GetFloat("RotationSpeedX") * vector.x;
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
			this.activateTimeout -= Time.deltaTime;
			if (this.activateTimeout > 0f)
			{
				float @float = this.config.GetFloat("DefaultYRotation");
				this.rotY = -@float * 0.0174532924f;
				this.rotX = Mathf.Atan2(this.Target.forward.x, this.Target.forward.z);
			}
			this.UpdateAutoReset();
			Vector3 forward;
			Game.Character.Utils.Math.ToCartesian(this.rotX, this.rotY, out forward);
			this.UnityCamera.transform.forward = forward;
			float float2 = this.config.GetFloat("Distance");
			float float3 = this.config.GetFloat("SpeedRollback");
			this.rollbackDistance = float2 * Mathf.Clamp01(this.velocityFilter.GetValue().magnitude * float3);
			this.UnityCamera.transform.position = this.cameraTarget - this.UnityCamera.transform.forward * this.targetDistance;
		}

		private void UpdateAutoReset()
		{
			if (this.config.GetBool("AutoReset"))
			{
				this.resetTimeout -= Time.deltaTime;
				if (this.rotationInputTimeout < 0.1f)
				{
					this.resetTimeout = this.config.GetFloat("AutoResetTimeout");
				}
				if (this.resetTimeout <= 0f)
				{
					this.resetTimeout = 0f;
					float num = this.rotX;
					if (this.targetVelocity.magnitude * 3.6f > this.config.GetFloat("WhenRotateSpeed") || (this.autoCameraVelocityTurn && this.targetVelocity.magnitude * 3.6f > this.config.GetFloat("WhenRotateSpeed") * 0.1f))
					{
						num = Mathf.Atan2(this.targetVelocity.x, this.targetVelocity.z);
						this.autoCameraVelocityTurn = true;
					}
					else
					{
						this.autoRotSpeed = this.config.GetFloat("AutoTurnMinSpeed");
					}
					if (this.autoCameraVelocityTurn && this.targetVelocity.magnitude * 3.6f < this.config.GetFloat("WhenRotateSpeed") * 0.1f)
					{
						this.autoCameraVelocityTurn = false;
					}
					float rotZ = -this.config.GetFloat("DefaultYRotation") * 0.0174532924f;
					Vector3 toDirection;
					Game.Character.Utils.Math.ToCartesian(num, rotZ, out toDirection);
					Vector3 vector;
					Game.Character.Utils.Math.ToCartesian(this.rotX, this.rotY, out vector);
					Quaternion quaternion = Quaternion.FromToRotation(vector, toDirection);
					float value = Quaternion.Angle(Quaternion.identity, quaternion);
					float max = Mathf.Min(this.autoRotSpeed * this.config.GetFloat("AutoTurnAcceleration"), this.config.GetFloat("AutoTurnMaxSpeed"));
					this.autoRotSpeed = Mathf.Clamp(value, this.autoRotSpeed, max);
					Quaternion quaternion2 = Quaternion.RotateTowards(Quaternion.identity, quaternion, this.autoRotSpeed * Time.deltaTime);
					if (Quaternion.Angle(Quaternion.identity, quaternion2) < Mathf.Epsilon)
					{
						this.autoRotSpeed = this.config.GetFloat("AutoTurnMinSpeed");
					}
					vector = quaternion2 * vector;
					Game.Character.Utils.Math.ToSpherical(vector, out this.rotX, out this.rotY);
				}
				else
				{
					this.autoCameraVelocityTurn = false;
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

		public override void Reset()
		{
			this.activateTimeout = 1f;
		}

		public override void PostUpdate()
		{
			if (this.InputManager.GetInput<bool>(InputType.Reset, false))
			{
				this.activateTimeout = 0.1f;
			}
			this.UpdateFOV();
			this.RotateCamera();
			this.UpdateDistance();
			this.UpdateDir();
		}

		private void UpdateCollision()
		{
			Vector3 cameraTarget = this.targetPos + this.GetOffsetPos();
			float num = this.config.GetFloat("Distance");
			num += this.rollbackDistance;
			this.collision.ProcessCollision(cameraTarget, base.GetTargetHeadPos(), this.UnityCamera.transform.forward, num, out this.collisionTargetDist, out this.collisionDistance);
			float num2 = this.collisionDistance / num;
			if (this.collisionTargetDist > num2)
			{
				this.collisionTargetDist = num2;
			}
			this.targetDistance = Interpolation.Lerp(this.targetDistance, this.collisionDistance, (this.targetDistance <= this.collisionDistance) ? this.collision.GetReturnSpeed() : this.collision.GetClipSpeed());
			this.currCollisionTargetDist = Mathf.SmoothDamp(this.currCollisionTargetDist, this.collisionTargetDist, ref this.collisionTargetVelocity, (this.currCollisionTargetDist <= this.collisionTargetDist) ? this.collision.GetReturnTargetSpeed() : this.collision.GetTargetClipSpeed());
		}

		public override void FixedStepUpdate()
		{
			this.targetFilter.AddSample(this.Target.position);
			this.UpdateCollision();
			base.UpdateTargetDummy();
			this.UpdateTargetVelocity();
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
				this.targetPos = Vector3.SmoothDamp(this.lastTargetPos, this.Target.position, ref this.springVelocity, this.config.GetFloat("Spring"));
				this.lastTargetPos = this.targetPos;
			}
		}

		private void UpdateTargetVelocity()
		{
			Vector3 a = this.Target.position - this.lastTargetPos;
			if (Time.deltaTime > 0f)
			{
				this.targetVelocity = a / Time.deltaTime;
				this.velocityFilter.AddSample(this.targetVelocity);
			}
		}

		public bool LerpFromLastPos = true;

		public bool dbgRing;

		private bool rotationInput;

		private float rotationInputTimeout;

		private float rotX;

		private float rotY;

		private Vector3 targetVelocity;

		private float collisionDistance;

		private float collisionZoomVelocity;

		private float rollbackDistance;

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

		private PositionFilter velocityFilter;

		private bool autoCameraVelocityTurn;

		private float autoRotSpeed;
	}
}
