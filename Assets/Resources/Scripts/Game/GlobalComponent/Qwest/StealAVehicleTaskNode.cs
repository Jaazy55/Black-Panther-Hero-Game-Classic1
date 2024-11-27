using System;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class StealAVehicleTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			StealAVehicleTask stealAVehicleTask = new StealAVehicleTask
			{
				VehicleType = this.VehicleType,
				SpecificVehicleName = this.VehicleName,
				countVisualMarks = this.MarksCount,
				markVisualType = this.MarksType
			};
			base.ToPoBase(stealAVehicleTask);
			return stealAVehicleTask;
		}

		[Separator("Specific")]
		[Space]
		[Header("Use null for any specific")]
		public VehicleList VehicleName;

		public VehicleType VehicleType = VehicleType.Any;

		public int MarksCount;

		[SelectiveString("MarkType")]
		public string MarksType;
	}
}
