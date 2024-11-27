using System;
using UnityEngine;

namespace Naxeex.AI
{
	public class NPCMovable : MonoBehaviour
	{
		public Transform MoveTarget
		{
			get
			{
				return this.m_NavigateAgent.Target;
			}
		}

		public Transform RotateTarget
		{
			get
			{
				return this.m_RotateTarget;
			}
		}

		public void ToRoam()
		{
			this.m_RotateTarget = null;
			this.m_NavigateAgent.SetWalkPath();
		}

		public void MoveTo(Vector3 position)
		{
		}

		public void FollowTo(Transform target)
		{
			this.m_RotateTarget = null;
			this.m_NavigateAgent.FollowTarget(target, 0f, null);
		}

		public void RotateTo(Transform target)
		{
			this.m_RotateTarget = target;
			this.m_NavigateAgent.Stop();
		}

		public void FlyTo(Vector3 position)
		{
		}

		public void FlyHeight(float height)
		{
		}

		public void Stop()
		{
			this.m_RotateTarget = null;
			this.m_NavigateAgent.Stop();
		}

		protected virtual void FixedUpdate()
		{
			if (this.m_Rigidbody.useGravity == this.m_GroundSensor.IsGrounded)
			{
				this.m_Rigidbody.useGravity = !this.m_GroundSensor.IsGrounded;
			}
			Vector3 vector = this.m_Rigidbody.position;
			if (this.m_GroundSensor.IsGrounded)
			{
				vector.y = Mathf.MoveTowards(vector.y, this.m_GroundSensor.Height, this.m_Rigidbody.velocity.y * Time.fixedDeltaTime);
			}
			else
			{
				vector += Time.fixedDeltaTime * Time.fixedDeltaTime / 2f * Physics.gravity;
			}
			this.m_Rigidbody.MovePosition(vector);
			if (!this.m_NavigateAgent.HasPath)
			{
				return;
			}
			Vector3 vector2 = default(Vector3);
			Vector3 vector3 = default(Vector3);
			if (this.m_RotateTarget != null)
			{
				vector3 = this.m_Rigidbody.transform.InverseTransformPoint(this.m_RotateTarget.position);
				vector3.y = 0f;
			}
			else
			{
				vector2 = this.m_Rigidbody.transform.InverseTransformPoint(this.m_NavigateAgent.SteeringTarget);
				vector3 = new Vector3(vector2.x, 0f, vector2.z);
			}
			float num = Vector3.Angle(vector3, Vector3.forward);
			if (num != 0f)
			{
				num = this.RotationCurve.Evaluate(num) * this.rotationSpeed * Time.fixedDeltaTime * Mathf.Sign(Vector3.Cross(vector3, Vector3.forward).y);
			}
			if (vector2.magnitude > 0f)
			{
				vector2.Normalize();
			}
			this.m_AnimatorController.Forward = this.ForwardCurve.Evaluate(vector2.z);
			this.m_Rigidbody.MoveRotation(this.m_Rigidbody.rotation * Quaternion.Euler(0f, -num, 0f));
		}

		[SerializeField]
		private NPCAnimationController m_AnimatorController;

		[SerializeField]
		private GroundSensor m_GroundSensor;

		[SerializeField]
		private NavigateAgent m_NavigateAgent;

		[SerializeField]
		private Rigidbody m_Rigidbody;

		[SerializeField]
		private float rotationSpeed = 1f;

		[SerializeField]
		private AnimationCurve RotationCurve;

		[SerializeField]
		private AnimationCurve ForwardCurve;

		private float fartherRotationThreshold = 45f;

		private float nearRotationThreshold = 20f;

		private float distanceThreshold = 2f;

		private Transform m_RotateTarget;
	}
}
