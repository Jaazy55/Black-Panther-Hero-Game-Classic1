using System;
using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.Items;
using Game.Traffic;
using Game.Vehicle;
using Game.Weapons;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class MassacreTask : BaseTask
	{
		public override void TaskStart()
		{
			if (!PlayerManager.Instance.PlayerIsDefault)
			{
				return;
			}
			base.TaskStart();
			GameEventManager.Instance.MassacreTaskActive = true;
			GameItemWeapon gameItemWeapon = ItemsManager.Instance.GetItem(this.WeaponItemID) as GameItemWeapon;
			if (gameItemWeapon != null)
			{
				WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
				int slotIndex = defaultWeaponController.WeaponSet.FirstSlotOfType(defaultWeaponController.GetTargetSlot(gameItemWeapon.Weapon));
				this.defaultWeaponID = defaultWeaponController.EquipWeapon(gameItemWeapon, slotIndex, true);
				defaultWeaponController.ChooseSlot(slotIndex);
				defaultWeaponController.LockWeponSet();
			}
			EntityManager instance = EntityManager.Instance;
			instance.PlayerKillEvent = (EntityManager.PlayerKill)Delegate.Combine(instance.PlayerKillEvent, new EntityManager.PlayerKill(this.CheckForComplite));
			this.searchGo = new GameObject
			{
				name = "SearchProcessing_" + base.GetType().Name
			};
			SearchProcess<HitEntity> process = new SearchProcess<HitEntity>
			{
				countMarks = this.MarksCount,
				markType = this.MarksTypeNPC
			};
			SearchProcessing searchProcessing = this.searchGo.AddComponent<SearchProcessing>();
			searchProcessing.process = process;
			searchProcessing.Init();
		}

		public void CheckForComplite(HitEntity enemy)
		{
			if (!(enemy is VehicleStatus))
			{
				this.currVictimsCount++;
				if (this.currVictimsCount >= this.RequiredVictimsCount)
				{
					this.CurrentQwest.MoveToTask(this.NextTask);
					FactionsManager.Instance.ChangePlayerRelations(Faction.Police, -FactionsManager.Instance.GetPlayerRelationsValue(Faction.Police));
					TrafficManager.Instance.CalmDownCops();
					EntityManager.Instance.ReturnAllLivingRagdollsAroundPoint(PlayerManager.Instance.Player.transform.position, 100f);
				}
			}
		}

		public override void Finished()
		{
			base.Finished();
			GameEventManager.Instance.MassacreTaskActive = false;
			GameItemWeapon gameItemWeapon = ItemsManager.Instance.GetItem(this.WeaponItemID) as GameItemWeapon;
			if (gameItemWeapon != null)
			{
				WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
				defaultWeaponController.UnlockWeponSet();
				int slotIndex = defaultWeaponController.WeaponSet.FirstSlotOfType(defaultWeaponController.GetTargetSlot(gameItemWeapon.Weapon));
				defaultWeaponController.UnEquipWeapon(slotIndex);
				GameItemWeapon gameItemWeapon2 = ItemsManager.Instance.GetItem(this.defaultWeaponID) as GameItemWeapon;
				if (gameItemWeapon2 != null)
				{
					defaultWeaponController.EquipWeapon(gameItemWeapon2, slotIndex, false);
				}
			}
			this.currVictimsCount = 0;
			EntityManager instance = EntityManager.Instance;
			instance.PlayerKillEvent = (EntityManager.PlayerKill)Delegate.Remove(instance.PlayerKillEvent, new EntityManager.PlayerKill(this.CheckForComplite));
			if (this.searchGo)
			{
				UnityEngine.Object.Destroy(this.searchGo);
			}
		}

		public override string TaskStatus()
		{
			return string.Concat(new object[]
			{
				base.TaskStatus(),
				"\nKilled  ",
				this.currVictimsCount,
				"/",
				this.RequiredVictimsCount
			});
		}

		private bool CheckConditionNPC(HitEntity npc)
		{
			return !(npc is VehicleStatus);
		}

		public int WeaponItemID;

		public int RequiredVictimsCount = 50;

		private int currVictimsCount;

		private int defaultWeaponID;

		public string MarksTypeNPC;

		public int MarksCount;

		private GameObject searchGo;
	}
}
