using System;
using System.Collections;
using Game.Character;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class LeaveACarAtPointTask : BaseTask
	{
		public override void TaskStart()
		{
			base.TaskStart();
			if (!PlayerInteractionsManager.Instance.IsDrivingAVehicle())
			{
				this.CurrentQwest.MoveToTask(this.PrevTask);
			}
			this.point = PoolManager.Instance.GetFromPool<QwestVehiclePoint>(GameEventManager.Instance.QwestVehiclePointPrefab.gameObject);
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
			if (this.Equals(task) && (this.SpecificVehicleName.Equals(VehicleList.None) || vehicle.VehicleSpecificPrefab.Name.Equals(this.SpecificVehicleName)))
			{
				base.StartDialog(this.AtPointDialog);
			}
		}

		public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
		{
			this.CurrentQwest.MoveToTask(this.PrevTask);
		}

		public override void GetOutVehicleEvent(DrivableVehicle vehicle)
		{
			if (Vector3.Distance(vehicle.MainRigidbody.position, this.PointPosition) < this.Range && (this.SpecificVehicleName.Equals(VehicleList.None) || vehicle.VehicleSpecificPrefab.Name.Equals(this.SpecificVehicleName)) && (this.VehicleType == VehicleType.Any || vehicle.GetVehicleType().Equals(this.VehicleType)))
			{
				if (PlayerInteractionsManager.Instance != null)
				{
					PlayerInteractionsManager.Instance.StartCoroutine(this.ReturnVehicleToPool(vehicle));
				}
				else if (!PlayerInteractionsManager.Instance.Player.IsTransformer && !PoolManager.Instance.ReturnToPool(vehicle.gameObject))
				{
					UnityEngine.Object.Destroy(vehicle.gameObject);
				}
				this.CurrentQwest.MoveToTask(this.NextTask);
			}
			else
			{
				this.CurrentQwest.MoveToTask(this.PrevTask);
			}
		}

		private IEnumerator ReturnVehicleToPool(DrivableVehicle vehicle)
		{
			while (PlayerInteractionsManager.Instance.Player.transform.parent == vehicle.transform)
			{
				yield return null;
			}
			if (!PlayerInteractionsManager.Instance.Player.IsTransformer && !PoolManager.Instance.ReturnToPool(vehicle.gameObject))
			{
				UnityEngine.Object.Destroy(vehicle.gameObject);
			}
			yield return null;
			yield break;
		}

		public Vector3 PointPosition;

		public VehicleList SpecificVehicleName;

		public VehicleType VehicleType;

		public float Range;

		public float PointRadius;

		public string AtPointDialog;

		private QwestVehiclePoint point;
	}
}
