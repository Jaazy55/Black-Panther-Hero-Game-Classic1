using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.Character.Extras
{
	internal class EntityManager
	{
		private EntityManager()
		{
			this.enemies = new HashSet<HitEntity>();
			this.players = new HashSet<HitEntity>();
			this.killedEntities = new HashSet<HitEntity>();
			this.livingRagdolls = new HashSet<RagdollStatus>();
		}

		public static EntityManager Instance
		{
			get
			{
				EntityManager result;
				if ((result = EntityManager.instance) == null)
				{
					result = (EntityManager.instance = new EntityManager());
				}
				return result;
			}
		}

		public HitEntity Player
		{
			get
			{
				return this.players.FirstOrDefault((HitEntity player) => player is Human && !((Human)player).Remote && player);
			}
		}

		public void SingleAlarm(HitEntity disturber, HitEntity victim)
		{
			if (!victim || !disturber)
			{
				return;
			}
			BaseStatusNPC baseStatusNPC = victim as BaseStatusNPC;
			if (baseStatusNPC != null)
			{
				baseStatusNPC.OnStatusAlarm(disturber);
			}
		}

		public void OverallAlarm(HitEntity disturber, HitEntity victim, Vector3 position, float range)
		{
			if (disturber == null)
			{
				return;
			}
			if (!this.overallCallers.ContainsKey(disturber))
			{
				this.overallCallers.Add(disturber, Time.time - 2f);
				PoolManager.Instance.AddBeforeReturnEvent(disturber, delegate(GameObject poolingObject)
				{
					this.overallCallers.Remove(disturber);
				});
			}
			if (Time.time < this.overallCallers[disturber] + 2f)
			{
				return;
			}
			BaseStatusNPC[] array = this.FindAllClosestNPCs(position, range, true);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != disturber)
				{
					if (array[i].BaseNPC.SpecificNpcLinks.SmartActionType == ActionType.Coward || array[i].Faction == Faction.Police)
					{
						array[i].OnStatusAlarm(disturber);
					}
					if (!(victim == null) && !(array[i] == victim))
					{
						if (FactionsManager.Instance.GetRelations(array[i].Faction, victim.Faction) == Relations.Friendly)
						{
							array[i].OnStatusAlarm(disturber);
						}
					}
				}
			}
			this.overallCallers[disturber] = Time.time;
		}

		public void Register(HitEntity enemy)
		{
			this.enemies.Add(enemy);
			this.killedEntities.Remove(enemy);
		}

		public void RegisterLivingRagdoll(RagdollStatus ragdoll)
		{
			this.livingRagdolls.Add(ragdoll);
		}

		public void UnregisterRagdoll(RagdollStatus ragdoll)
		{
			this.livingRagdolls.Remove(ragdoll);
		}

		public void RegisterPlayer(HitEntity player)
		{
			this.players.Add(player);
		}

		public void OnDeath(HitEntity enemy)
		{
			Player player = PlayerInteractionsManager.Instance.Player;
			if (enemy.LastHitOwner == player && FactionsManager.Instance.GetRelations(player.Faction, enemy.Faction) != Relations.Friendly)
			{
				LevelManager.Instance.AddExperience((int)enemy.ExperienceForAKill, true);
			}
			if (this.PlayerKillEvent != null && enemy.LastHitOwner == player)
			{
				this.PlayerKillEvent(enemy);
			}
			this.killedEntities.Add(enemy);
			this.enemies.Remove(enemy);
		}

		public bool EntityWasKilled(HitEntity entity)
		{
			return this.killedEntities.Contains(entity);
		}

		public HitEntity Find(Vector3 pos, float radius, string ignoreTag)
		{
			float num = radius * radius;
			HitEntity result = null;
			float num2 = float.MaxValue;
			foreach (HitEntity hitEntity in this.enemies)
			{
				if (hitEntity)
				{
					if (!hitEntity.gameObject.CompareTag(ignoreTag))
					{
						float sqrMagnitude = (pos - hitEntity.transform.position).sqrMagnitude;
						if (sqrMagnitude < num && sqrMagnitude < num2)
						{
							num2 = sqrMagnitude;
							result = hitEntity;
						}
					}
				}
			}
			return result;
		}

		public BaseStatusNPC[] FindAllClosestNPCs(Vector3 aroundPosition, float radius, bool onPlayerSignlineOnly = false)
		{
			List<BaseStatusNPC> list = new List<BaseStatusNPC>();
			foreach (HitEntity hitEntity in this.enemies)
			{
				BaseStatusNPC baseStatusNPC = hitEntity as BaseStatusNPC;
				if (baseStatusNPC != null && baseStatusNPC.gameObject.activeSelf && baseStatusNPC.Faction != Faction.Player && Vector3.Distance(aroundPosition, baseStatusNPC.transform.position) <= radius)
				{
					if (!onPlayerSignlineOnly)
					{
						list.Add(baseStatusNPC);
					}
					else if (PlayerManager.Instance.OnPlayerSignline(baseStatusNPC, radius))
					{
						list.Add(baseStatusNPC);
					}
				}
			}
			return list.ToArray();
		}

		public void ReturnToPoolAllEnemiesAroundPoint(Vector3 point, float radius)
		{
			foreach (HitEntity hitEntity in this.enemies)
			{
				if (!(hitEntity == null))
				{
					if (hitEntity.gameObject.activeSelf && Vector3.Distance(hitEntity.transform.position, point) <= radius)
					{
						BaseStatusNPC baseStatusNPC = hitEntity as BaseStatusNPC;
						if (baseStatusNPC != null)
						{
							PoolManager.Instance.ReturnToPool(baseStatusNPC);
						}
						else
						{
							VehicleStatus vehicleStatus = hitEntity as VehicleStatus;
							if (vehicleStatus != null)
							{
								PoolManager.Instance.ReturnToPool(vehicleStatus.transform.parent.gameObject);
							}
						}
					}
				}
			}
			this.ReturnAllLivingRagdollsAroundPoint(point, radius);
		}

		public void ReturnAllLivingRagdollsAroundPoint(Vector3 point, float radius)
		{
			foreach (RagdollStatus ragdollStatus in this.livingRagdolls)
			{
				if (!(ragdollStatus == null))
				{
					if ((!ragdollStatus.gameObject.activeSelf || Vector3.Distance(ragdollStatus.transform.position, point) > radius) && ragdollStatus.gameObject.activeInHierarchy)
					{
						ragdollStatus.StartCoroutine(this.DeInitRagdoll(ragdollStatus));
					}
				}
			}
		}

		private IEnumerator DeInitRagdoll(RagdollStatus ragdoll)
		{
			yield return new WaitForSeconds(0.2f);
			ragdoll.wakeUper.DeInitRagdoll(true, true, false, 0);
			yield break;
		}

		private const float CallOverallReloadTime = 2f;

		private static EntityManager instance;

		public EntityManager.PlayerKill PlayerKillEvent;

		private readonly HashSet<HitEntity> enemies;

		private readonly HashSet<HitEntity> players;

		private readonly HashSet<HitEntity> killedEntities;

		private readonly HashSet<RagdollStatus> livingRagdolls;

		private Dictionary<HitEntity, float> overallCallers = new Dictionary<HitEntity, float>();

		public delegate void PlayerKill(HitEntity enemy);
	}
}
