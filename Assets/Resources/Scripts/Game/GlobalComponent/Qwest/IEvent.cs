using System;
using Game.Character.CharacterController;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public interface IEvent
	{
		void PlayerDeadEvent(SuicideAchievment.DethType type = SuicideAchievment.DethType.None);

		void NpcKilledEvent(Vector3 position, Faction npcFaction, HitEntity victim, HitEntity killer);

		void PickedQwestItemEvent(Vector3 position, QwestPickupType pickupType, BaseTask relatedTask);

		void PointReachedEvent(Vector3 position, BaseTask task);

		void PointReachedByVehicleEvent(Vector3 position, BaseTask task, DrivableVehicle vehicle);

		void GetIntoVehicleEvent(DrivableVehicle vehicle);

		void GetOutVehicleEvent(DrivableVehicle vehicle);

		void PickUpCollectionEvent(string collectionName);

		void GetLevelEvent(int level);

		void GetShopEvent();

		void VehicleDrawingEvent(DrivableVehicle vehicle);
	}
}
