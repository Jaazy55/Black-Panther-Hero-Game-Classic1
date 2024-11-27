using System;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.PickUps;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class CollectItemsTask : BaseTask
	{
		public override void TaskStart()
		{
			this.searchGo = new GameObject
			{
				name = "SearchProcessing_" + base.GetType().Name
			};
			SearchProcess<QuestPickUp> process = new SearchProcess<QuestPickUp>(new Func<QuestPickUp, bool>(this.CheckConditionPickUp))
			{
				countMarks = this.MarksCount,
				markType = this.MarksTypePickUp
			};
			SearchProcessing searchProcessing = this.searchGo.AddComponent<SearchProcessing>();
			searchProcessing.process = process;
			searchProcessing.Init();
			SearchProcess<HumanoidStatusNPC> process2 = new SearchProcess<HumanoidStatusNPC>(new Func<HumanoidStatusNPC, bool>(this.CheckConditionNPC))
			{
				countMarks = this.MarksCount,
				markType = this.MarksTypeNPC
			};
			SearchProcessing searchProcessing2 = this.searchGo.AddComponent<SearchProcessing>();
			searchProcessing2.process = process2;
			searchProcessing2.Init();
			base.TaskStart();
			this.itemsCollected = 0;
		}

		public override void NpcKilledEvent(Vector3 position, Faction npcFaction, HitEntity victim, HitEntity killer)
		{
			if (npcFaction.Equals(this.TargetFaction))
			{
				for (int i = 0; i < GameEventManager.Instance.questPickUps.Length; i++)
				{
					QuestPickUp questPickUp = GameEventManager.Instance.questPickUps[i];
					if (questPickUp.type.Equals(this.PickupType))
					{
						QuestPickUp qwestItem = PoolManager.Instance.GetFromPool<QuestPickUp>(questPickUp);
						PoolManager.Instance.AddBeforeReturnEvent(qwestItem, delegate(GameObject poolingObject)
						{
							qwestItem.DeInit();
						});
						qwestItem.transform.parent = GameEventManager.Instance.transform;
						qwestItem.RelatedTask = this;
						Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
						qwestItem.transform.position = position + new Vector3(insideUnitCircle.x, 1f, insideUnitCircle.y);
						break;
					}
				}
			}
		}

		public override void PickedQwestItemEvent(Vector3 position, QwestPickupType pickupType, BaseTask relatedTask)
		{
			if (this.Equals(relatedTask) && pickupType.Equals(this.PickupType))
			{
				this.itemsCollected++;
				if (this.itemsCollected >= this.InitialCountToCollect)
				{
					this.CurrentQwest.MoveToTask(this.NextTask);
				}
			}
		}

		public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
		{
			this.itemsCollected = 0;
		}

		public override string TaskStatus()
		{
			return string.Concat(new object[]
			{
				base.TaskStatus(),
				"\nItem collected ",
				this.itemsCollected,
				"/",
				this.InitialCountToCollect
			});
		}

		public override void Finished()
		{
			if (this.searchGo)
			{
				UnityEngine.Object.Destroy(this.searchGo);
			}
			base.Finished();
		}

		private bool CheckConditionPickUp(QuestPickUp pickup)
		{
			return pickup.type.Equals(this.PickupType);
		}

		private bool CheckConditionNPC(HumanoidStatusNPC npc)
		{
			return npc.Faction == this.TargetFaction;
		}

		public int InitialCountToCollect;

		public QwestPickupType PickupType;

		public Faction TargetFaction;

		private int itemsCollected;

		public int MarksCount;

		public string MarksTypeNPC;

		public string MarksTypePickUp;

		private GameObject searchGo;
	}
}
