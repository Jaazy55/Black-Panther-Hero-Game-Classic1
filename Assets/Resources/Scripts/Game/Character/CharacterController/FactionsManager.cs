using System;
using System.Collections.Generic;
using Game.GlobalComponent;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.CharacterController
{
	public class FactionsManager : MonoBehaviour
	{
		public static FactionsManager Instance
		{
			get
			{
				if (FactionsManager.instance == null)
				{
					throw new Exception("FactionsManager is not initialized");
				}
				return FactionsManager.instance;
			}
		}

		public static void ClearPlayerRelations()
		{
			BaseProfile.ClearArray<string>("PlayerRelationsFactions");
			BaseProfile.ClearArray<string>("PlayerRelationsValues");
		}

		public void SavePlayerRelations()
		{
			List<int> list = new List<int>();
			List<float> list2 = new List<float>();
			foreach (PlayerRelations playerRelations in this.playerRelationsList)
			{
				if (!this.ignoredFactionsToSave.Contains(playerRelations.NpcFaction))
				{
					list.Add((int)playerRelations.NpcFaction);
					list2.Add(playerRelations.RelationValue);
				}
			}
			BaseProfile.StoreArray<int>(list.ToArray(), "PlayerRelationsFactions");
			BaseProfile.StoreArray<float>(list2.ToArray(), "PlayerRelationsValues");
		}

		public Relations GetRelations(Faction entityFaction, Faction targetFaction)
		{
			if (entityFaction == Faction.NoneFaction || targetFaction == Faction.NoneFaction)
			{
				return Relations.Neutral;
			}
			if (entityFaction == targetFaction)
			{
				return Relations.Friendly;
			}
			if (entityFaction == Faction.Player)
			{
				return this.GetPlayerRelations(targetFaction);
			}
			if (targetFaction == Faction.Player)
			{
				return this.GetPlayerRelations(entityFaction);
			}
			return this.GetNpcRelations(entityFaction, targetFaction);
		}

		public void ChangePlayerRelations(Faction faction, float value)
		{
			this.FindPlayerRelations(faction).ChangeRelationValue(value);
			if (faction == Faction.Police)
			{
				this.UpdateCopRelationSlider();
			}
		}

		public void ChangePlayerRelations(Faction faction, Relations newRelations)
		{
			this.FindPlayerRelations(faction).CurrentRelations = newRelations;
			if (faction == Faction.Police)
			{
				this.UpdateCopRelationSlider();
			}
		}

		public void ChangeFriendlyFactionsRelation(Faction rootFaction, float amount)
		{
			foreach (NpcRelations npcRelations in this.NpcRelationsList)
			{
				if (npcRelations.TheirRelations == Relations.Friendly && npcRelations.FirstFaction != npcRelations.SecondFaction)
				{
					Faction faction = Faction.NoneFaction;
					if (npcRelations.FirstFaction == rootFaction)
					{
						faction = npcRelations.SecondFaction;
					}
					else if (npcRelations.SecondFaction == rootFaction)
					{
						faction = npcRelations.FirstFaction;
					}
					if (faction != Faction.NoneFaction)
					{
						this.ChangePlayerRelations(faction, amount);
					}
				}
			}
		}

		public void ChangeFriendlyFactionsRelation(Faction rootFaction, Relations newRelations)
		{
			foreach (NpcRelations npcRelations in this.NpcRelationsList)
			{
				if (npcRelations.TheirRelations == Relations.Friendly && npcRelations.FirstFaction != npcRelations.SecondFaction)
				{
					Faction faction = Faction.NoneFaction;
					if (npcRelations.FirstFaction == rootFaction)
					{
						faction = npcRelations.SecondFaction;
					}
					else if (npcRelations.SecondFaction == rootFaction)
					{
						faction = npcRelations.FirstFaction;
					}
					if (faction != Faction.NoneFaction)
					{
						this.ChangePlayerRelations(faction, newRelations);
					}
				}
			}
		}

		public void CommitedACrime()
		{
			if (this.GetPlayerRelations(Faction.Police) != Relations.Hostile)
			{
				this.ChangePlayerRelations(Faction.Police, -1f);
				this.ChangeFriendlyFactionsRelation(Faction.Police, -1f);
			}
			this.lastCrimeTime = Time.time;
		}

		public void PlayerAttackHuman(HitEntity victim)
		{
			if (!this.victimsList.Contains(victim) && this.GetNpcRelations(victim.Faction, Faction.Police) == Relations.Friendly)
			{
				this.victimsList.Add(victim);
				this.CommitedACrime();
				PoolManager.Instance.AddBeforeReturnEvent(victim, delegate(GameObject poolingObject)
				{
					this.victimsList.Remove(victim);
				});
			}
		}

		public Relations GetPlayerRelations(Faction faction)
		{
			return this.FindPlayerRelations(faction).CurrentRelations;
		}

		public float GetPlayerRelationNormalized(Faction faction)
		{
			float playerRelationsValue = this.GetPlayerRelationsValue(faction);
			if (playerRelationsValue >= 0f)
			{
				return playerRelationsValue / 10f;
			}
			return -playerRelationsValue / -10f;
		}

		public float GetPlayerRelationsValue(Faction faction)
		{
			return this.FindPlayerRelations(faction).RelationValue;
		}

		private void Awake()
		{
			FactionsManager.instance = this;
			this.CopRelationSubSlider.maxValue = Math.Abs(-10f);
			this.CopRelationSlider.maxValue = Math.Abs(-10f) / 2f;
			Slider copRelationSubSlider = this.CopRelationSubSlider;
			float value = 0f;
			this.CopRelationSlider.value = value;
			copRelationSubSlider.value = value;
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 1f);
			this.LoadPlayerRelations();
			this.LoadDefaultFactionsValues();
		}

		private void SlowUpdate()
		{
			if (Time.time > this.lastCrimeTime + 20f && this.GetPlayerRelationsValue(Faction.Police) < 0f)
			{
				this.ChangePlayerRelations(Faction.Police, 0.5f);
				this.ChangeFriendlyFactionsRelation(Faction.Police, 0.5f);
			}
		}

		private void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void LoadPlayerRelations()
		{
			int[] array = BaseProfile.ResolveArray<int>("PlayerRelationsFactions");
			float[] array2 = BaseProfile.ResolveArray<float>("PlayerRelationsValues");
			if (array != null && array.Length != 0)
			{
				this.playerRelationsList.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					Faction item = (Faction)array[i];
					if (!this.ignoredFactionsToSave.Contains(item))
					{
						this.playerRelationsList.Add(new PlayerRelations
						{
							NpcFaction = (Faction)array[i],
							RelationValue = array2[i]
						});
					}
				}
			}
		}

		private void LoadDefaultFactionsValues()
		{
			foreach (PlayerRelations playerRelations in this.PlayerDefaultRelations)
			{
				PlayerRelations playerRelations2 = null;
				foreach (PlayerRelations playerRelations3 in this.playerRelationsList)
				{
					if (playerRelations3.NpcFaction == playerRelations.NpcFaction)
					{
						playerRelations2 = playerRelations3;
						break;
					}
				}
				if (playerRelations2 == null)
				{
					this.playerRelationsList.Add(playerRelations);
				}
			}
		}

		private void UpdateCopRelationSlider()
		{
			this.CopRelationSubSlider.value = Mathf.Abs(this.GetPlayerRelationsValue(Faction.Police));
			this.CopRelationSlider.value = (float)((int)Math.Truncate((double)(Mathf.Abs(this.GetPlayerRelationsValue(Faction.Police)) / 2f)));
		}

		private PlayerRelations FindPlayerRelations(Faction faction)
		{
			PlayerRelations playerRelations = null;
			for (int i = 0; i < this.playerRelationsList.Count; i++)
			{
				PlayerRelations playerRelations2 = this.playerRelationsList[i];
				if (playerRelations2.NpcFaction == faction)
				{
					playerRelations = playerRelations2;
					break;
				}
			}
			if (playerRelations == null)
			{
				playerRelations = new PlayerRelations
				{
					NpcFaction = faction
				};
				this.playerRelationsList.Add(playerRelations);
			}
			return playerRelations;
		}

		private Relations GetNpcRelations(Faction firstFac, Faction secondFac)
		{
			NpcRelations npcRelations = null;
			for (int i = 0; i < this.NpcRelationsList.Count; i++)
			{
				NpcRelations npcRelations2 = this.NpcRelationsList[i];
				if ((npcRelations2.FirstFaction == firstFac && npcRelations2.SecondFaction == secondFac) || (npcRelations2.FirstFaction == secondFac && npcRelations2.SecondFaction == firstFac))
				{
					npcRelations = npcRelations2;
					break;
				}
			}
			if (npcRelations == null)
			{
				npcRelations = new NpcRelations
				{
					FirstFaction = firstFac,
					SecondFaction = secondFac
				};
				this.NpcRelationsList.Add(npcRelations);
			}
			return npcRelations.TheirRelations;
		}

		private const string PlayerRelationsFactionsArrayName = "PlayerRelationsFactions";

		private const string PlayerRelationsValuesArrayName = "PlayerRelationsValues";

		private const float PolliceAttentionTime = 20f;

		private const float PoliceRelationWarmingValue = 0.5f;

		private static FactionsManager instance;

		public List<PlayerRelations> PlayerDefaultRelations = new List<PlayerRelations>();

		public List<NpcRelations> NpcRelationsList = new List<NpcRelations>();

		[Separator]
		public Slider CopRelationSlider;

		public Slider CopRelationSubSlider;

		private List<PlayerRelations> playerRelationsList = new List<PlayerRelations>();

		private readonly List<HitEntity> victimsList = new List<HitEntity>();

		private SlowUpdateProc slowUpdateProc;

		private float lastCrimeTime;

		private readonly List<Faction> ignoredFactionsToSave = new List<Faction>
		{
			Faction.Police,
			Faction.Civilian
		};
	}
}
