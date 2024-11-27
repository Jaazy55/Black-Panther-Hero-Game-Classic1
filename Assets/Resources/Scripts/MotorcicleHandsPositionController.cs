using System;
using UnityEngine;

public class MotorcicleHandsPositionController : MonoBehaviour
{
	private void Update()
	{
		float num = Mathf.Clamp((this.SteeringWheel.localRotation.eulerAngles.z - this.ZeroSteeringAngle) / this.MaxSteeringAngle, -1f, 1f);
		this.LeftArm.Lerp(num);
		this.RightArm.Lerp(num);
		if (this.spine.zeroSpinePosition != Vector3.zero)
		{
			this.spine.spineTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(this.spine.zeroSpinePosition), Quaternion.Euler(this.spine.maxSpinePosition), Mathf.Abs(num));
		}
		if (this.spine.zeroChestPosition != Vector3.zero)
		{
			this.spine.chestTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(this.spine.zeroChestPosition), Quaternion.Euler(this.spine.maxChestPosition), Mathf.Abs(num));
		}
	}

	public void SetArms(Transform lUpperArm, Transform lForeArm, Transform rUpperArm, Transform rForeArm)
	{
		this.LeftArm.Forearm.bone = lForeArm;
		this.LeftArm.Upperarm.bone = lUpperArm;
		this.RightArm.Forearm.bone = rForeArm;
		this.RightArm.Upperarm.bone = rUpperArm;
	}

	public void SetMiddlePosition()
	{
		this.LeftArm.Forearm.MiddleState.rotation = this.LeftArm.Forearm.bone.localRotation.eulerAngles;
		this.LeftArm.Upperarm.MiddleState.rotation = this.LeftArm.Upperarm.bone.localRotation.eulerAngles;
		this.RightArm.Forearm.MiddleState.rotation = this.RightArm.Forearm.bone.localRotation.eulerAngles;
		this.RightArm.Upperarm.MiddleState.rotation = this.RightArm.Upperarm.bone.localRotation.eulerAngles;
		if (this.spine.spineTransform)
		{
			this.spine.zeroSpinePosition = this.spine.spineTransform.localEulerAngles;
		}
		if (this.spine.chestTransform)
		{
			this.spine.zeroChestPosition = this.spine.chestTransform.localEulerAngles;
		}
	}

	public void SetRightPosition()
	{
		this.LeftArm.Forearm.TurnRightState.rotation = this.LeftArm.Forearm.bone.localRotation.eulerAngles;
		this.LeftArm.Upperarm.TurnRightState.rotation = this.LeftArm.Upperarm.bone.localRotation.eulerAngles;
		this.RightArm.Forearm.TurnRightState.rotation = this.RightArm.Forearm.bone.localRotation.eulerAngles;
		this.RightArm.Upperarm.TurnRightState.rotation = this.RightArm.Upperarm.bone.localRotation.eulerAngles;
		if (this.spine.spineTransform)
		{
			this.spine.maxSpinePosition = this.spine.spineTransform.localEulerAngles;
		}
		if (this.spine.chestTransform)
		{
			this.spine.maxChestPosition = this.spine.chestTransform.localEulerAngles;
		}
	}

	public void SetLeftPosition()
	{
		this.LeftArm.Forearm.TurnLeftState.rotation = this.LeftArm.Forearm.bone.localRotation.eulerAngles;
		this.LeftArm.Upperarm.TurnLeftState.rotation = this.LeftArm.Upperarm.bone.localRotation.eulerAngles;
		this.RightArm.Forearm.TurnLeftState.rotation = this.RightArm.Forearm.bone.localRotation.eulerAngles;
		this.RightArm.Upperarm.TurnLeftState.rotation = this.RightArm.Upperarm.bone.localRotation.eulerAngles;
		if (this.spine.spineTransform)
		{
			this.spine.maxSpinePosition = this.spine.spineTransform.localEulerAngles;
		}
		if (this.spine.chestTransform)
		{
			this.spine.maxChestPosition = this.spine.chestTransform.localEulerAngles;
		}
	}

	public void SetCharacterMiddlePosition()
	{
		this.LeftArm.Forearm.bone.localRotation = Quaternion.Euler(this.LeftArm.Forearm.MiddleState.rotation.x, this.LeftArm.Forearm.MiddleState.rotation.y, this.LeftArm.Forearm.MiddleState.rotation.z);
		this.LeftArm.Upperarm.bone.localRotation = Quaternion.Euler(this.LeftArm.Upperarm.MiddleState.rotation.x, this.LeftArm.Upperarm.MiddleState.rotation.y, this.LeftArm.Upperarm.MiddleState.rotation.z);
		this.RightArm.Forearm.bone.localRotation = Quaternion.Euler(this.RightArm.Forearm.MiddleState.rotation.x, this.RightArm.Forearm.MiddleState.rotation.y, this.RightArm.Forearm.MiddleState.rotation.z);
		this.RightArm.Upperarm.bone.localRotation = Quaternion.Euler(this.RightArm.Upperarm.MiddleState.rotation.x, this.RightArm.Upperarm.MiddleState.rotation.y, this.RightArm.Upperarm.MiddleState.rotation.z);
		this.SteeringWheel.localRotation = Quaternion.Euler(0f, 0f, this.ZeroSteeringAngle);
		if (this.spine.spineTransform)
		{
			this.spine.spineTransform.localRotation = Quaternion.Euler(this.spine.zeroSpinePosition.x, this.spine.zeroSpinePosition.y, this.spine.zeroSpinePosition.z);
		}
		if (this.spine.chestTransform)
		{
			this.spine.chestTransform.localRotation = Quaternion.Euler(this.spine.zeroChestPosition.x, this.spine.zeroChestPosition.y, this.spine.zeroChestPosition.z);
		}
	}

	public void SetCharacterRightPosition()
	{
		this.LeftArm.Forearm.bone.localRotation = Quaternion.Euler(this.LeftArm.Forearm.TurnRightState.rotation.x, this.LeftArm.Forearm.TurnRightState.rotation.y, this.LeftArm.Forearm.TurnRightState.rotation.z);
		this.LeftArm.Upperarm.bone.localRotation = Quaternion.Euler(this.LeftArm.Upperarm.TurnRightState.rotation.x, this.LeftArm.Upperarm.TurnRightState.rotation.y, this.LeftArm.Upperarm.TurnRightState.rotation.z);
		this.RightArm.Forearm.bone.localRotation = Quaternion.Euler(this.RightArm.Forearm.TurnRightState.rotation.x, this.RightArm.Forearm.TurnRightState.rotation.y, this.RightArm.Forearm.TurnRightState.rotation.z);
		this.RightArm.Upperarm.bone.localRotation = Quaternion.Euler(this.RightArm.Upperarm.TurnRightState.rotation.x, this.RightArm.Upperarm.TurnRightState.rotation.y, this.RightArm.Upperarm.TurnRightState.rotation.z);
		this.SteeringWheel.localRotation = Quaternion.Euler(0f, 0f, this.ZeroSteeringAngle + this.MaxSteeringAngle);
		if (this.spine.spineTransform)
		{
			this.spine.spineTransform.localRotation = Quaternion.Euler(this.spine.maxSpinePosition.x, this.spine.maxSpinePosition.y, this.spine.maxSpinePosition.z);
		}
		if (this.spine.chestTransform)
		{
			this.spine.chestTransform.localRotation = Quaternion.Euler(this.spine.maxChestPosition.x, this.spine.maxChestPosition.y, this.spine.maxChestPosition.z);
		}
	}

	public void SetCharacterLeftPosition()
	{
		this.LeftArm.Forearm.bone.localRotation = Quaternion.Euler(this.LeftArm.Forearm.TurnLeftState.rotation.x, this.LeftArm.Forearm.TurnLeftState.rotation.y, this.LeftArm.Forearm.TurnLeftState.rotation.z);
		this.LeftArm.Upperarm.bone.localRotation = Quaternion.Euler(this.LeftArm.Upperarm.TurnLeftState.rotation.x, this.LeftArm.Upperarm.TurnLeftState.rotation.y, this.LeftArm.Upperarm.TurnLeftState.rotation.z);
		this.RightArm.Forearm.bone.localRotation = Quaternion.Euler(this.RightArm.Forearm.TurnLeftState.rotation.x, this.RightArm.Forearm.TurnLeftState.rotation.y, this.RightArm.Forearm.TurnLeftState.rotation.z);
		this.RightArm.Upperarm.bone.localRotation = Quaternion.Euler(this.RightArm.Upperarm.TurnLeftState.rotation.x, this.RightArm.Upperarm.TurnLeftState.rotation.y, this.RightArm.Upperarm.TurnLeftState.rotation.z);
		this.SteeringWheel.localRotation = Quaternion.Euler(0f, 0f, this.ZeroSteeringAngle - this.MaxSteeringAngle);
		if (this.spine.spineTransform)
		{
			this.spine.spineTransform.localRotation = Quaternion.Euler(this.spine.maxSpinePosition.x, this.spine.maxSpinePosition.y, this.spine.maxSpinePosition.z);
		}
		if (this.spine.chestTransform)
		{
			this.spine.chestTransform.localRotation = Quaternion.Euler(this.spine.maxChestPosition.x, this.spine.maxChestPosition.y, this.spine.maxChestPosition.z);
		}
	}

	public MotorcicleHandsPositionController.Spine spine;

	public MotorcicleHandsPositionController.Arm LeftArm;

	public MotorcicleHandsPositionController.Arm RightArm;

	public Transform SteeringWheel;

	public float MaxSteeringAngle;

	public float ZeroSteeringAngle;

	[Serializable]
	public class BoneHandler
	{
		public void Lerp(float t)
		{
			this.curState = ((t >= 0f) ? this.TurnRightState : this.TurnLeftState);
			this.bone.localRotation = Quaternion.Lerp(this.MiddleState.QRotation, this.curState.QRotation, Mathf.Abs(t));
		}

		private MotorcicleHandsPositionController.BoneState curState;

		public Transform bone;

		public MotorcicleHandsPositionController.BoneState TurnLeftState;

		public MotorcicleHandsPositionController.BoneState MiddleState;

		public MotorcicleHandsPositionController.BoneState TurnRightState;
	}

	[Serializable]
	public class BoneState
	{
		public Quaternion QRotation
		{
			get
			{
				if (!this.rotInit)
				{
					this._QRotation = Quaternion.Euler(this.rotation);
					this.rotInit = true;
				}
				return this._QRotation;
			}
		}

		public Vector3 rotation;

		private Quaternion _QRotation;

		private bool rotInit;
	}

	[Serializable]
	public class Arm
	{
		public void Lerp(float t)
		{
			this.Upperarm.Lerp(t);
			this.Forearm.Lerp(t);
		}

		public MotorcicleHandsPositionController.BoneHandler Upperarm;

		public MotorcicleHandsPositionController.BoneHandler Forearm;
	}

	[Serializable]
	public class Spine
	{
		public Transform spineTransform;

		public Vector3 maxSpinePosition;

		public Vector3 zeroSpinePosition;

		public Transform chestTransform;

		public Vector3 maxChestPosition;

		public Vector3 zeroChestPosition;
	}
}
