using System;
using UnityEngine;

public class MyCustomIK : MonoBehaviour
{
	private void Start()
	{
		for (int i = 0; i < this.Limbs.Length; i++)
		{
			MyCustomIK.Limb limb = this.Limbs[i];
			limb.Initialialization(this);
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < this.Limbs.Length; i++)
		{
			MyCustomIK.Limb limb = this.Limbs[i];
			if (limb.IsEnabled && limb.NeedToRecalculate())
			{
				limb.CalculateIK();
			}
		}
	}

	public MyCustomIK.Limb[] Limbs;

	[Serializable]
	public class Limb
	{
		public void Initialialization(MyCustomIK IKObject)
		{
			this.upperArmStartRotation = this.upperArm.rotation;
			this.forearmStartRotation = this.forearm.rotation;
			this.handStartRotation = this.hand.rotation;
			this.elbowTargetRelativeStartPosition = this.elbowTarget.position - this.upperArm.position;
			this.parentForAxis = new GameObject("IK_" + this.LimbName);
			this.upperArmAxisCorrection = new GameObject("upperArmAxisCorrection_" + this.LimbName);
			this.forearmAxisCorrection = new GameObject("forearmAxisCorrection_" + this.LimbName);
			this.handAxisCorrection = new GameObject("handAxisCorrection_" + this.LimbName);
			this.parentForAxis.transform.parent = IKObject.transform;
			this.upperArmAxisCorrection.transform.parent = this.parentForAxis.transform;
			this.forearmAxisCorrection.transform.parent = this.upperArmAxisCorrection.transform;
			this.handAxisCorrection.transform.parent = this.forearmAxisCorrection.transform;
			this.LastFrameTargetPosition = default(Vector3);
			this.LastFrameElbowTargetPosition = default(Vector3);
		}

		public void CalculateIK()
		{
			if (this.target == null)
			{
				this.targetRelativeStartPosition = Vector3.zero;
				return;
			}
			if (this.targetRelativeStartPosition == Vector3.zero && this.target != null)
			{
				this.targetRelativeStartPosition = this.target.position - this.upperArm.position;
			}
			if (this.upperArm != null && this.forearm != null && this.hand != null)
			{
				float num = Vector3.Distance(this.upperArm.position, this.forearm.position);
				float num2 = Vector3.Distance(this.forearm.position, this.hand.position);
				float num3 = num + num2;
				float num4 = num;
				float num5 = Vector3.Distance(this.upperArm.position, this.target.position);
				num5 = Mathf.Min(num5, num3 - 0.0001f);
				float num6 = (num4 * num4 - num2 * num2 + num5 * num5) / (2f * num5);
				float x = Mathf.Acos(num6 / num4) * 57.29578f;
				Vector3 position = this.target.position;
				Vector3 position2 = this.elbowTarget.position;
				Transform parent = this.upperArm.parent;
				Transform parent2 = this.forearm.parent;
				Transform parent3 = this.hand.parent;
				Vector3 localScale = this.upperArm.localScale;
				Vector3 localScale2 = this.forearm.localScale;
				Vector3 localScale3 = this.hand.localScale;
				Vector3 localPosition = this.upperArm.localPosition;
				Vector3 localPosition2 = this.forearm.localPosition;
				Vector3 localPosition3 = this.hand.localPosition;
				Quaternion rotation = this.upperArm.rotation;
				Quaternion rotation2 = this.forearm.rotation;
				Quaternion rotation3 = this.hand.rotation;
				Quaternion localRotation = this.hand.localRotation;
				this.target.position = this.targetRelativeStartPosition + this.upperArm.position;
				this.elbowTarget.position = this.elbowTargetRelativeStartPosition + this.upperArm.position;
				this.upperArm.rotation = this.upperArmStartRotation;
				this.forearm.rotation = this.forearmStartRotation;
				this.hand.rotation = this.handStartRotation;
				this.parentForAxis.transform.position = this.upperArm.position;
				this.parentForAxis.transform.LookAt(position, position2 - this.parentForAxis.transform.position);
				this.upperArmAxisCorrection.transform.position = this.upperArm.position;
				this.upperArmAxisCorrection.transform.LookAt(this.forearm.position, Vector3.up);
				this.upperArm.parent = this.upperArmAxisCorrection.transform;
				this.forearmAxisCorrection.transform.position = this.forearm.position;
				this.forearmAxisCorrection.transform.LookAt(this.hand.position, Vector3.up);
				this.forearm.parent = this.forearmAxisCorrection.transform;
				this.handAxisCorrection.transform.position = this.hand.position;
				this.hand.parent = this.handAxisCorrection.transform;
				this.target.position = position;
				this.elbowTarget.position = position2;
				this.upperArmAxisCorrection.transform.LookAt(this.target, Vector3.up);
				this.upperArmAxisCorrection.transform.localRotation = Quaternion.Euler(this.upperArmAxisCorrection.transform.localRotation.eulerAngles - new Vector3(x, 0f, 0f));
				this.upperArmAxisCorrection.transform.LookAt(this.forearmAxisCorrection.transform, Vector3.up);
				this.forearmAxisCorrection.transform.LookAt(this.target, Vector3.up);
				this.handAxisCorrection.transform.rotation = this.target.rotation;
				this.upperArmAxisCorrection.transform.LookAt(this.forearmAxisCorrection.transform, Vector3.up);
				this.Weight = Mathf.Clamp01(this.Weight);
				this.upperArm.parent = parent;
				this.upperArm.localScale = localScale;
				this.upperArm.localPosition = localPosition;
				this.forearm.parent = parent2;
				this.forearm.localScale = localScale2;
				this.forearm.localPosition = localPosition2;
				this.hand.parent = parent3;
				this.hand.localScale = localScale3;
				this.hand.localPosition = localPosition3;
				this.hand.rotation = this.target.rotation;
				this.LastFrameTargetPosition = this.target.position;
				this.LastFrameElbowTargetPosition = this.elbowTarget.position;
				this.LastFrameTargetRotation = this.target.rotation;
				this.LastFrameElbowTargetRotation = this.elbowTarget.rotation;
				return;
			}
		}

		public bool NeedToRecalculate()
		{
			return this.LastFrameTargetPosition != this.target.position || this.LastFrameElbowTargetPosition != this.elbowTarget.position || this.LastFrameTargetRotation != this.target.rotation || this.LastFrameElbowTargetRotation != this.elbowTarget.rotation;
		}

		public string LimbName;

		public Transform upperArm;

		public Transform forearm;

		public Transform hand;

		public Transform target;

		public Transform elbowTarget;

		private Vector3 LastFrameTargetPosition;

		private Vector3 LastFrameElbowTargetPosition;

		private Quaternion LastFrameTargetRotation;

		private Quaternion LastFrameElbowTargetRotation;

		public bool IsEnabled;

		public float Weight = 1f;

		private Quaternion upperArmStartRotation;

		private Quaternion forearmStartRotation;

		private Quaternion handStartRotation;

		private Vector3 targetRelativeStartPosition;

		private Vector3 elbowTargetRelativeStartPosition;

		private GameObject upperArmAxisCorrection;

		private GameObject forearmAxisCorrection;

		private GameObject handAxisCorrection;

		private GameObject parentForAxis;
	}
}
