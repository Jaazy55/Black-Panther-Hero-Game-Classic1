using System;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class DriveToPointTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			DriveToPointTask driveToPointTask = new DriveToPointTask
			{
				SpecificVehicleName = this.VehicleName,
				VehicleType = this.VehicleType,
				PointRadius = this.PointRadius,
				PointPosition = ((!(this.PointPosition == null)) ? this.PointPosition.position : base.transform.position)
			};
			base.ToPoBase(driveToPointTask);
			return driveToPointTask;
		}

		private void OnDrawGizmos()
		{
			if (this.IsDebug)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
				Gizmos.DrawSphere((!(this.PointPosition == null)) ? this.PointPosition.position : base.transform.position, this.PointRadius);
			}
		}

		[Separator("Specific")]
		[Space]
		[Header("Will use its own position if null")]
		public Transform PointPosition;

		[Header("Use null for any specific")]
		public VehicleList VehicleName;

		public float PointRadius = 2f;

		public VehicleType VehicleType = VehicleType.Any;
	}
}
