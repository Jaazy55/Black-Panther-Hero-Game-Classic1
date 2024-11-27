using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class RealFixedJoint : MonoBehaviour
	{
		private void Start()
		{
			GameObject gameObject = new GameObject("JointStart");
			this.armIK = gameObject.transform;
			GameObject gameObject2 = new GameObject("JointEnd");
			this.armRotation = gameObject2.transform;
			this.armRotation.parent = this.armIK;
			this.upperArmLength = Vector3.Distance(base.transform.position, this.Forearm.position);
			this.forearmLength = Vector3.Distance(this.Forearm.position, this.Hand.position);
			this.armLength = this.upperArmLength + this.forearmLength;
		}

		private void LateUpdate()
		{
			Quaternion rotation = base.transform.rotation;
			Quaternion rotation2 = this.Forearm.rotation;
			Quaternion rotation3 = this.RooTransform.rotation;
			this.RooTransform.rotation = Quaternion.identity;
			this.RooTransform.eulerAngles = this.OfsetRotation;
			this.armIK.position = base.transform.position;
			this.armIK.LookAt(this.Forearm);
			this.armRotation.position = base.transform.position;
			this.armRotation.rotation = base.transform.rotation;
			this.armIK.LookAt(this.Target);
			base.transform.rotation = this.armRotation.rotation;
			float num = Vector3.Distance(base.transform.position, this.Target.position);
			num = Mathf.Min(num, (float)((double)this.armLength - 0.1));
			float num2 = (this.upperArmLength * this.upperArmLength - this.forearmLength * this.forearmLength + num * num) / (2f * num);
			float num3 = Mathf.Acos(num2 / this.upperArmLength) * 57.29578f;
			if (float.IsNaN(num3))
			{
				return;
			}
			base.transform.RotateAround(base.transform.position, base.transform.forward, -num3);
			this.armIK.position = this.Forearm.position;
			this.armIK.LookAt(this.Hand);
			this.armRotation.position = this.Forearm.position;
			this.armRotation.rotation = this.Forearm.rotation;
			this.armIK.LookAt(this.Target);
			this.Forearm.rotation = this.armRotation.rotation;
			base.transform.RotateAround(base.transform.position, this.Target.position - base.transform.position, this.elbowAngle);
			this.Transition = Mathf.Clamp01(this.Transition);
			base.transform.rotation = Quaternion.Slerp(rotation, base.transform.rotation, this.Transition);
			this.Forearm.rotation = Quaternion.Slerp(rotation2, this.Forearm.rotation, this.Transition);
			this.Forearm.eulerAngles += this.ForearmOfsetRotation;
			base.transform.eulerAngles += this.HandOfsetRotation;
			this.RooTransform.rotation = rotation3;
		}

		public bool Debug;

		public Transform Forearm;

		public Transform Hand;

		public Transform Target;

		public Transform RooTransform;

		public Vector3 OfsetRotation = new Vector3(90f, 0f, 0f);

		public Vector3 ForearmOfsetRotation = new Vector3(90f, 0f, 0f);

		public Vector3 HandOfsetRotation = new Vector3(90f, 0f, 0f);

		public float Transition = 0.1f;

		public float elbowAngle;

		private Transform armIK;

		private Transform armRotation;

		private float upperArmLength;

		private float forearmLength;

		private float armLength;
	}
}
