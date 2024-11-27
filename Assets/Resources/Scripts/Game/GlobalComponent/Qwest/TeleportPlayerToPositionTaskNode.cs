using System;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class TeleportPlayerToPositionTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			if (this.m_LoadPositionFromTargetTransform)
			{
				this.m_TargetPositon = this.m_TargetPoint.position;
				this.m_TargetRotation = this.m_TargetPoint.rotation;
			}
			TeleportPlayerToPositionTask teleportPlayerToPositionTask = new TeleportPlayerToPositionTask
			{
				targetPosition = this.m_TargetPositon,
				targetRotarion = this.m_TargetRotation
			};
			base.ToPoBase(teleportPlayerToPositionTask);
			return teleportPlayerToPositionTask;
		}

		private void UpdatePostions()
		{
			if (this.m_LoadPositionFromTargetTransform)
			{
				this.m_TargetPositon = this.m_TargetPoint.position;
				this.m_TargetRotation = this.m_TargetPoint.rotation;
			}
		}

		[Separator("Specific")]
		public bool m_LoadPositionFromTargetTransform;

		public Transform m_TargetPoint;

		[SerializeField]
		private Vector3 m_TargetPositon;

		[SerializeField]
		private Quaternion m_TargetRotation;

		[InspectorButton("UpdatePostions")]
		public bool UpdatePosition;
	}
}
