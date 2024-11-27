using System;
using Game.Character;
using Game.Vehicle;
using UnityEngine;

namespace MiamiGames.Mech
{
	public class MechSpiderController : MechController
	{
		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			this.cameraTransform = CameraManager.Instance.UnityCamera.transform;
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.IsInitialized)
			{
				this.DetermineTargetRotation();
			}
		}

		private void Update()
		{
			if (this.IsInitialized)
			{
				this.StabilizationOnSurface();
				this.HorizontalRotateControl();
			}
		}

		private void HorizontalRotateControl()
		{
			Vector3 worldPosition = this.cameraTransform.position + this.cameraTransform.transform.forward * -100f;
			this.drivableMech.rotatedPart.transform.LookAt(worldPosition);
		}

		private void StabilizationOnSurface()
		{
			this.MainRigidbody.transform.rotation = Quaternion.Lerp(this.MainRigidbody.transform.rotation, this.targetRotation, Time.deltaTime * 5f);
		}

		private void DetermineTargetRotation()
		{
			Ray ray = new Ray(this.MainRigidbody.transform.position, -this.MainRigidbody.transform.up);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 2f, this.GroundMask))
			{
				this.targetRotation = Quaternion.FromToRotation(this.MainRigidbody.transform.up, raycastHit.normal) * this.MainRigidbody.transform.rotation;
			}
			else
			{
				this.targetRotation = Quaternion.identity;
			}
		}

		private const float RayDistance = 2f;

		private const int CameraOffset = -100;

		private Transform cameraTransform;

		private Quaternion targetRotation;
	}
}
