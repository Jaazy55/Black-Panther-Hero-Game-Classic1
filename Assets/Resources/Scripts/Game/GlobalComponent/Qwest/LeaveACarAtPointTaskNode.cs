using System;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class LeaveACarAtPointTaskNode : DriveToPointTaskNode
	{
		public override BaseTask ToPo()
		{
			LeaveACarAtPointTask leaveACarAtPointTask = new LeaveACarAtPointTask
			{
				SpecificVehicleName = this.VehicleName,
				VehicleType = this.VehicleType,
				PointRadius = this.PointRadius,
				Range = this.LeaveRange,
				AtPointDialog = this.AtPointDialog,
				PointPosition = ((!(this.PointPosition == null)) ? this.PointPosition.position : base.transform.position)
			};
			base.ToPoBase(leaveACarAtPointTask);
			return leaveACarAtPointTask;
		}

		[TextArea]
		public string AtPointDialog;

		public float LeaveRange = 10f;
	}
}
