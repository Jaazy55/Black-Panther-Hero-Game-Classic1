using System;
using UnityEngine;

namespace Game.Vehicle
{
	public class VehicleSpecific : MonoBehaviour
	{
		public Transform PlayerSkeleton
		{
			get
			{
				return (!(this.PlayerOnlySkeleton != null)) ? this.Skeleton : this.PlayerOnlySkeleton;
			}
		}

		public Transform PlayerDeadSkeleton
		{
			get
			{
				return (!(this.PlayerOnlyDeadSkeleton != null)) ? this.SkeletonDead : this.PlayerOnlyDeadSkeleton;
			}
		}

		private void OnDrawGizmos()
		{
			if (!this.ShowDriverStatusesPoint)
			{
				return;
			}
			Gizmos.color = Color.green;
			Vector3 center = base.transform.TransformPoint(this.PlayerDriverStatusPosition);
			Gizmos.DrawSphere(center, 0.5f);
			Gizmos.color = Color.red;
			center = base.transform.TransformPoint(this.DriverStatusPosition);
			Gizmos.DrawSphere(center, 0.5f);
		}

		public VehicleList Name;

		public Transform Skeleton;

		public Transform SkeletonDead;

		public Vector3 DriverStatusPosition = Vector3.zero;

		[Tooltip("In Euler angles")]
		public Vector3 DriverStatusRotation = Vector3.zero;

		[Space(10f)]
		public Transform PlayerOnlySkeleton;

		public Transform PlayerOnlyDeadSkeleton;

		public Vector3 PlayerDriverStatusPosition = Vector3.zero;

		[Tooltip("In Euler angles")]
		public Vector3 PlayerDriverStatusRotation = Vector3.zero;

		[Tooltip("Green - for player, red - for npc")]
		public bool ShowDriverStatusesPoint;

		private Transform playerSkeletonInHierarchy;

		private Transform skeletonInHierarchy;

		private bool skeletonsFound;

		[Space(10f)]
		public float MaxLength = 3f;

		public float MaxHeight = 2f;

		public float MaxWidth = 2.5f;

		public bool HasRadio = true;

		public float forceGetOutAnimationLength = 3f;

		[Separator("Setup for Traffic Driver ")]
		public int PullForce = 300000;

		public int EngineTorque = 10000;

		public int RotateForce = 10000;

		public float BrakeTorque = 50f;

		public int SteerMaxAngle = 60;

		[Separator("Quest Direction Arrow Options")]
		public Vector3 ArrowPosOffset;
	}
}
