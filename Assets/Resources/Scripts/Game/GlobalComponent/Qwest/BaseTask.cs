using System;
using Game.Character.CharacterController;
using Game.DialogSystem;
using Game.Items;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class BaseTask : IEvent
	{
		public virtual void TaskStart()
		{
			this.StartDialog(this.DialogData);
		}

		public virtual void Init(Qwest qwest)
		{
			this.CurrentQwest = qwest;
		}

		public virtual void Cancel()
		{
			this.Target = null;
		}

		public virtual void Finished()
		{
			this.StartDialog(this.EndDialogData);
			this.Target = null;
		}

		public virtual void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
		{
		}

		public virtual void NpcKilledEvent(Vector3 position, Faction npcFaction, HitEntity victim, HitEntity killer)
		{
		}

		public virtual void PickedQwestItemEvent(Vector3 position, QwestPickupType pickupType, BaseTask relatedTask)
		{
		}

		public virtual void PointReachedEvent(Vector3 position, BaseTask task)
		{
		}

		public virtual void PointReachedByVehicleEvent(Vector3 position, BaseTask task, DrivableVehicle vehicle)
		{
		}

		public virtual void GetIntoVehicleEvent(DrivableVehicle vehicle)
		{
		}

		public virtual void GetOutVehicleEvent(DrivableVehicle vehicle)
		{
		}

		public virtual void PickUpCollectionEvent(string collectionName)
		{
		}

		public virtual void GetLevelEvent(int level)
		{
		}

		public virtual void GetShopEvent()
		{
		}

		public virtual void VehicleDrawingEvent(DrivableVehicle vehicle)
		{
		}

		public virtual void BuyItemEvent(GameItem item)
		{
		}

		public virtual string TaskStatus()
		{
			return this.TaskText;
		}

		public virtual Transform TaskTarget()
		{
			return this.Target;
		}

		protected void StartDialog(string dialog)
		{
			if (!string.IsNullOrEmpty(dialog) && dialog.Length > Qwest.MinCharDialogLength)
			{
				DialogManager.Instance.StartDialog(dialog);
			}
		}

		public string TaskText;

		public BaseTask NextTask;

		public BaseTask PrevTask;

		public float AdditionalTimer;

		public string DialogData;

		public string EndDialogData;

		protected Transform Target;

		protected Qwest CurrentQwest;
	}
}
