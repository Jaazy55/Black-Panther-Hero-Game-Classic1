using System;
using Game.Character;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class DriveToPointTask : BaseTask
	{
		public override void TaskStart()
		{
			if (!PlayerInteractionsManager.Instance.IsDrivingAVehicle())
			{
				this.CurrentQwest.MoveToTask(this.PrevTask);
				return;
			}
			base.TaskStart();
			this.point = PoolManager.Instance.GetFromPool<QwestVehiclePoint>(GameEventManager.Instance.QwestVehiclePointPrefab);
			PoolManager.Instance.AddBeforeReturnEvent(this.point, delegate(GameObject poolingObject)
			{
				this.Target = null;
				if (this.point != null)
				{
					this.point.Task = null;
					this.point = null;
				}
			});
			this.point.SetRadius(this.PointRadius);
			this.point.Task = this;
			this.point.transform.parent = GameEventManager.Instance.transform;
			this.point.transform.position = this.PointPosition;
			this.Target = this.point.transform;
		}

		public override void Finished()
		{
			if (this.point != null)
			{
				PoolManager.Instance.ReturnToPool(this.point);
			}
			base.Finished();
		}

		public override void PointReachedByVehicleEvent(Vector3 position, BaseTask task, DrivableVehicle vehicle)
		{
			if (this.Equals(task) && (this.SpecificVehicleName.Equals(VehicleList.None) || vehicle.VehicleSpecificPrefab.Name.Equals(this.SpecificVehicleName)) && (this.VehicleType == VehicleType.Any || vehicle.GetVehicleType().Equals(this.VehicleType)))
			{
				PoolManager.Instance.ReturnToPool(this.point);
				this.CurrentQwest.MoveToTask(this.NextTask);
			}
		}

		public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
		{
			this.CurrentQwest.MoveToTask(this.PrevTask);
		}

		public override void GetOutVehicleEvent(DrivableVehicle vehicle)
		{
			this.CurrentQwest.MoveToTask(this.PrevTask);
		}

		public Vector3 PointPosition;

		public VehicleList SpecificVehicleName;

		public float PointRadius;

		public VehicleType VehicleType;

		private QwestVehiclePoint point;
	}
}
