using System;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class StealAVehicleTask : BaseTask
	{
		public override void TaskStart()
		{
			this.searchGo = new GameObject
			{
				name = "SearchProcessing_" + base.GetType().Name
			};
			SearchProcess<DrivableVehicle> process = new SearchProcess<DrivableVehicle>(new Func<DrivableVehicle, bool>(this.CheckCondition))
			{
				countMarks = this.countVisualMarks,
				markType = this.markVisualType
			};
			SearchProcessing searchProcessing = this.searchGo.AddComponent<SearchProcessing>();
			searchProcessing.process = process;
			searchProcessing.Init();
			base.TaskStart();
		}

		public override void Finished()
		{
			if (this.searchGo)
			{
				UnityEngine.Object.Destroy(this.searchGo);
			}
			base.Finished();
		}

		public override void GetIntoVehicleEvent(DrivableVehicle vehicle)
		{
			if (this.CheckCondition(vehicle))
			{
				this.CurrentQwest.MoveToTask(this.NextTask);
			}
		}

		private bool CheckCondition(DrivableVehicle vehicle)
		{
			return (this.SpecificVehicleName.Equals(VehicleList.None) || this.SpecificVehicleName.Equals(vehicle.VehicleSpecificPrefab.Name)) && (this.VehicleType == VehicleType.Any || vehicle.GetVehicleType().Equals(this.VehicleType));
		}

		public VehicleList SpecificVehicleName;

		public VehicleType VehicleType;

		public int countVisualMarks;

		public string markVisualType;

		private GameObject searchGo;
	}
}
