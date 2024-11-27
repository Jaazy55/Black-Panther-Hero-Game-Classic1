using System;
using System.Collections;
using Game.Character;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class HelicopterController : VehicleController
	{
		public bool IsGrounded
		{
			get
			{
				return this.isGrounded;
			}
		}

		public bool InitializedGetter
		{
			get
			{
				return this.IsInitialized;
			}
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Copter;
		}

		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			this.IsInitialized = false;
			DrivableHelicopter x = drivableVehicle as DrivableHelicopter;
			if (x != null)
			{
				this.currentHelicopter = x;
			}
			this.cameraTransform = CameraManager.Instance.UnityCamera.transform;
			this.OpenCabine(true);
			base.StartCoroutine(this.DeferredInit());
		}

		public override void DeInit(Action callbackAfterDeInit)
		{
			if (!this.IsGrounded)
			{
				base.Invoke("EvacuationTilt", 2.7f);
			}
			else
			{
				this.MainRigidbody.useGravity = true;
				this.MainRigidbody.angularDrag = 0.05f;
				this.currentHelicopter.CurrentBladesRotateSpeed = Mathf.Min(this.currentHelicopter.CurrentBladesRotateSpeed, this.AvaibleToFlyBladesRotateSpeed);
			}
			this.IsInitialized = false;
			this.xTiltAngle = 0f;
			this.zTiltAngle = 0f;
			this.xForce = 0f;
			this.zForce = 0f;
			this.OpenCabine(false);
			this.deInitAction = callbackAfterDeInit;
			HelicopterSpecific helicopterSpecific = this.VehicleSpecific as HelicopterSpecific;
			if (helicopterSpecific != null)
			{
				base.Invoke("DeferredDeInit", helicopterSpecific.GetOutAnimationTime);
			}
		}

		public override void StopVehicle(bool inMoment = false)
		{
			base.StopVehicle(inMoment);
			this.IsInitialized = false;
			this.currentHelicopter.CurrentBladesRotateSpeed = Mathf.Min(this.currentHelicopter.CurrentBladesRotateSpeed, this.AvaibleToFlyBladesRotateSpeed);
		}

		protected override void Drowning()
		{
			base.Drowning();
			this.IsInitialized = false;
			if (this.currentHelicopter.CurrentBladesRotateSpeed > this.AvaibleToFlyBladesRotateSpeed)
			{
				this.currentHelicopter.CurrentBladesRotateSpeed = this.AvaibleToFlyBladesRotateSpeed;
			}
			if (this.currentHelicopter.CurrentBladesRotateSpeed > 0f)
			{
				this.currentHelicopter.CurrentBladesRotateSpeed -= Time.deltaTime;
			}
			this.xAccelerationInput = 0f;
			this.zAccelerationInput = 0f;
			this.TiltControl();
			this.verticalMovementInput = -1f;
			this.VerticalControl();
		}

		protected override void SetCameraFollowTarget(DrivableVehicle drivableVehicle)
		{
			base.StartCoroutine(this.DefferedSetCameraTerget(drivableVehicle));
		}

		private IEnumerator DeferredInit()
		{
			yield return new WaitForSeconds(5f);
			this.IsInitialized = true;
			this.MainRigidbody.angularDrag = this.RigidbodyAngularDrag;
			yield break;
		}

		private IEnumerator DefferedSetCameraTerget(DrivableVehicle drivableVehicle)
		{
			yield return new WaitForSeconds(5f);
			base.SetCameraFollowTarget(drivableVehicle);
			yield break;
		}

		private void DeferredDeInit()
		{
			base.DeInit(this.deInitAction);
		}

		protected override void FixedUpdate()
		{
			if (this.DrivableVehicle.WaterSensor.InWater)
			{
				this.Drowning();
			}
			if (!this.IsInitialized)
			{
				return;
			}
			base.FixedUpdate();
			this.Inputs();
			this.BladesControl();
			this.GroundCheck();
			this.MainRigidbody.useGravity = !this.avaibleToFly;
			if (this.avaibleToFly)
			{
				this.VerticalControl();
				if (!this.isGrounded)
				{
					this.TiltControl();
					this.HorizontralRotateControl();
				}
				else
				{
					this.xTiltAngle = 0f;
					this.zTiltAngle = 0f;
					this.xForce = 0f;
					this.zForce = 0f;
				}
			}
		}

		private void OpenCabine(bool getIn)
		{
			HelicopterSpecific helicopterSpecific = this.VehicleSpecific as HelicopterSpecific;
			if (helicopterSpecific != null && helicopterSpecific.CabinAnimator != null && this.currentHelicopter.AnimateGetInOut)
			{
				helicopterSpecific.CabinAnimator.SetBool("EnterIn", getIn);
				helicopterSpecific.CabinAnimator.SetTrigger("Open");
			}
		}

		private void Inputs()
		{
			this.xAccelerationInput = Controls.GetAxis("Vertical");
			this.zAccelerationInput = -Controls.GetAxis("Horizontal");
			this.verticalMovementInput = Controls.GetAxis("Vertical_Heli");
		}

		private void GroundCheck()
		{
			this.isGrounded = Physics.Raycast(this.MainRigidbody.transform.position + this.MainRigidbody.transform.up * 0.2f, -this.MainRigidbody.transform.up, 1f, this.GroundLayerMask);
		}

		private void VerticalControl()
		{
			if (this.verticalMovementInput != 0f && (Math.Sign(this.verticalMovementInput) == Math.Sign(this.verticalForce) || Math.Round((double)this.verticalForce, 1) == 0.0))
			{
				this.verticalForce += this.VerticalMovementVelocity * this.verticalMovementInput;
			}
			else if (this.verticalMovementInput != 0f && Math.Sign(this.verticalMovementInput) != Math.Sign(this.verticalForce))
			{
				this.verticalForce = Mathf.Lerp(this.verticalForce, 0f, Time.deltaTime * (float)this.VerticalMovementBrakeForce);
			}
			else
			{
				this.verticalForce = Mathf.Lerp(this.verticalForce, 0f, Time.deltaTime);
			}
			this.verticalForce = Mathf.Clamp(this.verticalForce, -this.MaxVerticalVelocity, this.MaxVerticalVelocity);
			if (this.MainRigidbody.transform.position.y >= this.MaximumHelicopterY)
			{
				this.verticalForce = -this.MaxVerticalVelocity;
			}
			this.MainRigidbody.AddForce(this.MainRigidbody.transform.up * this.verticalForce, ForceMode.Acceleration);
		}

		private void TiltControl()
		{
			this.xTiltAngle += this.TiltAngleSpeed * this.xAccelerationInput;
			this.zTiltAngle += this.TiltAngleSpeed * this.zAccelerationInput;
			this.xTiltAngle = Mathf.Clamp(this.xTiltAngle, -this.MaxTiltAngle, this.MaxTiltAngle);
			this.zTiltAngle = Mathf.Clamp(this.zTiltAngle, -this.MaxTiltAngle, this.MaxTiltAngle);
			this.xForce += -(this.SideMovementForce * this.zAccelerationInput);
			this.zForce += this.SideMovementForce * this.xAccelerationInput;
			this.xForce = Mathf.Clamp(this.xForce, -this.MaximumSideForce, this.MaximumSideForce);
			this.zForce = Mathf.Clamp(this.zForce, -this.MaximumSideForce, this.MaximumSideForce);
			if (this.xAccelerationInput == 0f)
			{
				this.xTiltAngle = Mathf.LerpAngle(this.xTiltAngle, 0f, Time.deltaTime);
				this.zForce = Mathf.Lerp(this.zForce, 0f, Time.deltaTime);
			}
			if (this.zAccelerationInput == 0f)
			{
				this.zTiltAngle = Mathf.LerpAngle(this.zTiltAngle, 0f, Time.deltaTime);
				this.xForce = Mathf.Lerp(this.xForce, 0f, Time.deltaTime);
			}
			this.MainRigidbody.transform.localEulerAngles = new Vector3(this.xTiltAngle, this.MainRigidbody.transform.localEulerAngles.y, this.zTiltAngle);
			Vector3 force = this.MainRigidbody.transform.TransformDirection(this.xForce, 0f, this.zForce);
			force.y = 0f;
			this.MainRigidbody.AddForce(force, ForceMode.Acceleration);
		}

		private void HorizontralRotateControl()
		{
			Vector3 vector = this.MainRigidbody.transform.InverseTransformPoint(this.cameraTransform.position + this.cameraTransform.transform.forward * 100f);
			float num = vector.x / vector.magnitude;
			float num2 = (num <= 0f) ? (-this.TorqueForce) : this.TorqueForce;
			this.MainRigidbody.AddRelativeTorque(0f, num2 * Mathf.Abs(num), 0f, ForceMode.Force);
		}

		private void BladesControl()
		{
			this.currentHelicopter.CurrentBladesRotateSpeed += Time.deltaTime;
			this.currentHelicopter.CurrentBladesRotateSpeed = Mathf.Clamp(this.currentHelicopter.CurrentBladesRotateSpeed, 0f, this.MaxBladesRotateSpeed);
			this.avaibleToFly = (this.currentHelicopter.CurrentBladesRotateSpeed >= this.AvaibleToFlyBladesRotateSpeed);
		}

		private void EvacuationTilt()
		{
			this.MainRigidbody.useGravity = true;
			this.MainRigidbody.angularDrag = 0.05f;
			Vector3 a = this.MainRigidbody.transform.TransformDirection(Vector3.right);
			Vector3 a2 = this.MainRigidbody.transform.TransformDirection(-Vector3.forward);
			this.MainRigidbody.AddForce(a * 2f, ForceMode.VelocityChange);
			this.MainRigidbody.AddTorque(a2 * 2f, ForceMode.Acceleration);
		}

		private const string XAccelerationAxisName = "Vertical";

		private const string ZAccelerationAxisName = "Horizontal";

		private const string VerticalMovementAxisName = "Vertical_Heli";

		private const int GroundCheckRayDistance = 1;

		private const float JumpOutFromHelicopterAnimationLenght = 2.7f;

		private const float EnterInAnimationLength = 5f;

		private const int EvacuationForce = 2;

		[Separator("HelicopterSpecificParametrs")]
		public LayerMask GroundLayerMask;

		public float TiltAngleSpeed = 1f;

		public float MaxTiltAngle = 30f;

		public float VerticalMovementVelocity = 0.2f;

		public float MaxVerticalVelocity = 20f;

		public int VerticalMovementBrakeForce = 6;

		public float SideMovementForce = 0.5f;

		public float MaximumSideForce = 25f;

		public float TorqueForce = 200000f;

		public float MaxBladesRotateSpeed = 20f;

		public float AvaibleToFlyBladesRotateSpeed = 10f;

		public float MaximumHelicopterY = 200f;

		public float RigidbodyAngularDrag = 2f;

		public AudioClip OpenCabineSound;

		private bool isGrounded;

		private bool avaibleToFly;

		private DrivableHelicopter currentHelicopter;

		private float verticalMovementInput;

		private float xAccelerationInput;

		private float zAccelerationInput;

		private Transform cameraTransform;

		private float verticalForce;

		private float xTiltAngle;

		private float zTiltAngle;

		private float zForce;

		private float xForce;

		private Action deInitAction;
	}
}
