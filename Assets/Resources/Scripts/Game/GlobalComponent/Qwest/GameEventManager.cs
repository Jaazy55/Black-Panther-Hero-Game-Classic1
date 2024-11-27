using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Items;
using Game.MiniMap;
using Game.PickUps;
using Game.UI;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	[AddComponentMenu("MiniMap/Map marker")]

	public class GameEventManager : MonoBehaviour
	{
		public static GameEventManager Instance
		{
			get
			{
				if (GameEventManager.instance == null)
				{
					throw new Exception("GameEventManager is not initialized");
				}
				return GameEventManager.instance;
			}
		}

		public bool TaskSelectionAvailable
		{
			get
			{
				return !this.MassacreTaskActive;
			}
		}

		public bool TimeQwestActive
		{
			get
			{
				return this.CurrentTimeQwest != null;
			}
		}
		public GameObject[] obj_maps;
		public void StartQwest(Qwest qwest)
		{
			if (this.TimeQwestActive)
			{
				return;
			}
			if (this.availableQwests.Contains(qwest))
			{
				this.ActiveQwests.Add(qwest);
				qwest.Init();
				this.MarkedQwest = qwest;
				this.RefreshQwestArrow();
				this.availableQwests.Remove(qwest);
				this.ToggleQwestMark(qwest, false);
				if (qwest.IsTimeQwest)
				{
					this.CurrentTimeQwest = qwest;
					this.MarkedQwest = qwest;
					BackgroundMusic.instance.PlayTimeQuestClip();
				}
			//	AdsManager.ShowFullscreenAd();
			}

		}

		public void QwestFailed(Qwest qwest)
		{
			if (this.ActiveQwests.Contains(qwest))
			{
				BaseTask currentTask = qwest.GetCurrentTask();
				if (currentTask != null)
				{
					currentTask.Finished();
				}
				this.ActiveQwests.Remove(qwest);
				if (qwest.Equals(this.MarkedQwest))
				{
					this.MarkedQwest = ((this.ActiveQwests.Count <= 0) ? null : this.ActiveQwests[0]);
					this.RefreshQwestArrow();
				}
				this.PlaceQuestOnMap(qwest);
				InGameLogManager.Instance.RegisterNewMessage(MessageType.QuestFailed, qwest.QwestTitle);
				PointSoundManager.Instance.PlaySoundAtPoint(Vector3.zero, "QwestFailed");
				if (qwest.Equals(this.CurrentTimeQwest))
				{
					this.CurrentTimeQwest = null;
					BackgroundMusic.instance.StopTimeQuestClip();
				}
			}
		}

		public void QwestCancel(Qwest qwest)
		{
			if (this.ActiveQwests.Contains(qwest))
			{
				BaseTask currentTask = qwest.GetCurrentTask();
				if (currentTask != null)
				{
					currentTask.Cancel();
				}
				this.ActiveQwests.Remove(qwest);
				if (qwest.Equals(this.MarkedQwest))
				{
					this.MarkedQwest = ((this.ActiveQwests.Count <= 0) ? null : this.ActiveQwests[0]);
					this.RefreshQwestArrow();
				}
				InGameLogManager.Instance.RegisterNewMessage(MessageType.QuestItem, qwest.QwestTitle + " canceled");
				if (qwest.Equals(this.CurrentTimeQwest))
				{
					this.CurrentTimeQwest = null;
					BackgroundMusic.instance.StopTimeQuestClip();
				}
			}
		}

		public void ResetQwestCancel(Qwest qwest)
		{
			this.PlaceQuestOnMap(qwest);
		}

		public void QwestResolved(Qwest qwest)
		{
			if (this.TimeQwestActive && !qwest.Equals(this.CurrentTimeQwest))
			{
				return;
			}
			if (this.TimeQwestActive && qwest.Equals(this.CurrentTimeQwest))
			{
				this.CurrentTimeQwest = null;
				BackgroundMusic.instance.StopTimeQuestClip();
			}
			this.nextQuests.Clear();
			this.ActiveQwests.Remove(qwest);
			if (qwest.RepeatableQuest)
			{
				this.nextQuests.Add(qwest);
			}
			else
			{
				Dictionary<string, bool> qwestStatus = QwestProfile.QwestStatus;
				if (!qwestStatus.ContainsKey(qwest.Name))
				{
					qwestStatus.Add(qwest.Name, true);
					QwestProfile.QwestStatus = qwestStatus;
				}
				for (int i = 0; i < qwest.QwestTree.Count; i++)
				{
					Qwest item = qwest.QwestTree[i];
					this.nextQuests.Add(item);
				}
			}
			for (int j = 0; j < this.nextQuests.Count; j++)
			{
				Qwest quest = this.nextQuests[j];
				this.PlaceQuestOnMap(quest);
			}
			if (qwest.Equals(this.MarkedQwest))
			{
				this.MarkedQwest = ((this.ActiveQwests.Count <= 0) ? null : this.ActiveQwests[0]);
				this.RefreshQwestArrow();
			}
//			AdsManager.ShowFullscreenAd();
		}

		public void RefreshQwestArrow()
		{
			if (UIMarkManager.InstanceExist)
			{
				if (this.MarkedQwest != null && this.MarkedQwest.GetQwestTarget() != null && QwestProfile.QwestArrow)
				{
					UIMarkManager.Instance.TargetStaticMark = this.MarkedQwest.GetQwestTarget();
					UIMarkManager.Instance.ActivateStaticMark(true);
				}
				else
				{
					UIMarkManager.Instance.ActivateStaticMark(false);
				}
			}
		}

		private void GenerateQwestStartPoint(Qwest qwest)
		{
			QwestStart qwestStart = PoolManager.Instance.GetFromPool<QwestStart>(this.QwestStartPrefab);
			PoolManager.Instance.AddBeforeReturnEvent(qwestStart, delegate(GameObject A_1)
			{
				qwestStart.Qwest = null;
			});
			qwestStart.Qwest = qwest;
			qwestStart.transform.parent = base.transform;
			qwestStart.transform.position = qwest.StartPosition;
			qwestStart.transform.localScale = new Vector3(1f + qwest.AdditionalStartPointRadius, 1f + qwest.AdditionalStartPointRadius, 1f + qwest.AdditionalStartPointRadius);
			this.ToggleQwestMark(qwest, true);
		}

		private void Awake()
		{
			if (GameEventManager.instance == null)
			{
				GameEventManager.instance = this;
				UIGame uigame = UIGame.Instance;
				uigame.OnExitInMenu = (Action)Delegate.Combine(uigame.OnExitInMenu, new Action(this.OnExitInMenu));
				UIGame uigame2 = UIGame.Instance;
				uigame2.OnPausePanelOpen = (Action)Delegate.Combine(uigame2.OnPausePanelOpen, new Action(this.OnPausePanelOpen));
			}
			if (this.MapMarksListTransform != null)
			{
				MarkForMiniMap[] componentsInChildren = this.MapMarksListTransform.GetComponentsInChildren<MarkForMiniMap>();
				if (componentsInChildren != null)
				{
					foreach (MarkForMiniMap markForMiniMap in componentsInChildren)
					{
						markForMiniMap.HideIcon();
					}
				}
			}
			this.allQwests.AddRange(MiamiSerializier.JSONDeserialize<List<Qwest>>(this.SerializedQwests.text));
			Dictionary<string, bool> qwestStatus = QwestProfile.QwestStatus;
			foreach (Qwest qwest in this.allQwests)
			{
				if (!qwestStatus.ContainsKey(qwest.Name) && (qwest.ParentQwest == null || (qwest.ParentQwest != null && qwestStatus.ContainsKey(qwest.ParentQwest.Name))))
				{
					this.PlaceQuestOnMap(qwest);
				}
			}
			this.allAchievements.RemoveAll((Achievement elem) => elem != null);
			this.activeAchievements.RemoveAll((Achievement elem) => elem != null);
			foreach (Achievement achievement in base.GetComponentsInChildren<Achievement>())
			{
				achievement.Init();
				this.allAchievements.Add(achievement);
			}
			if (this.AchievmentsReset)
			{
				this.SaveAchievements();
			}
			this.LoadAchievements();
			foreach (Achievement achievement2 in this.allAchievements)
			{
				if (!achievement2.achiveParams.isDone)
				{
					this.activeAchievements.Add(achievement2);
				}
			}
			Debug.Log ("start");
			Invoke ("wait",1f);
		}
		void wait()
		{
	
			obj_maps = GameObject.FindGameObjectsWithTag ("mapicon");
			for(int a=0;a<obj_maps.Length;a++)
			{
				obj_maps [a].gameObject.GetComponent<MapMarker> ().enabled = true;
			}
		}
		private void PlaceQuestOnMap(Qwest quest)
		{
			this.availableQwests.Add(quest);
			this.GenerateQwestStartPoint(quest);
		}

		private void SaveAchievements()
		{
			foreach (Achievement achievement in this.allAchievements)
			{
				achievement.SaveAchiev();
			}
			CollectionPickUpsManager.Instance.SaveCollections();
		}

		private void LoadAchievements()
		{
			foreach (Achievement achievement in this.allAchievements)
			{
				achievement.LoadAchiev();
			}
		}
		public bool onetime = true;
		public GameObject currentmarker;

		private void Update()
		{
			//if (GameObject.Find ("QuestStartPoint(Clone)").activeInHierarchy) 
			//{
				
			//		GameObject.Find ("QuestStartPoint(Clone)").gameObject.GetComponent<MapMarker> ().enabled = true;
			//		if (onetime == true) {
			//			currentmarker = GameObject.Find ("QuestStartPoint(Clone)").gameObject.GetComponent<MapMarker> ().gameObject;
			//			onetime = false;
			//		}

		//	} 
			//else 
			//{
		//		GameObject.Find ("QuestStartPoint(Clone)").gameObject.GetComponent<MapMarker> ().markerSprite = null;


		//	}

				
			if (this.MMMark != null)
			{
				if (!this.MMMark.isActiveAndEnabled)
				{
					this.MMMark.gameObject.SetActive(true);
				}
				if (this.MarkedQwest != null && this.MarkedQwest.GetQwestTarget() != null)
				{
					this.MMMark.transform.position = this.MarkedQwest.GetQwestTarget().position;
					this.MMMark.ShowIcon();
				}
				else
				{
					this.MMMark.HideIcon();
				}
			}
		}

		private void ToggleQwestMark(Qwest qwest, bool toggle)
		{
			if (qwest.MMMarkId != -1)
			{
				MarkForMiniMap markForMiniMap = qwest.MarkForMiniMap;
				if (markForMiniMap == null)
				{
					markForMiniMap = this.MapMarksListTransform.GetChild(qwest.MMMarkId).GetComponent<MarkForMiniMap>();
					qwest.MarkForMiniMap = markForMiniMap;
				}
				if (toggle)
				{
					markForMiniMap.transform.position = qwest.StartPosition;
					markForMiniMap.gameObject.SetActive(true);
					markForMiniMap.ShowIcon();
				}
				else
				{
					markForMiniMap.HideIcon();
					markForMiniMap.gameObject.SetActive(false);
				}
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (!pauseStatus)
			{
				return;
			}
			this.SaveAchievements();
		}

		private void OnApplicationQuit()
		{
			this.SaveAchievements();
		}

		private void OnExitInMenu()
		{
			this.SaveAchievements();
		}

		private void OnPausePanelOpen()
		{
			this.SaveAchievements();
		}

		private static GameEventManager instance;

		public readonly IEvent Event = new GameEventManager.GameEvent();

		[Separator("Qwest Data")]
		public TextAsset SerializedQwests;

		[Separator("Mini Map")]
		public MarkForMiniMap MMMark;

		public Transform MapMarksListTransform;

		[Separator("Prefabs")]
		public QuestPickUp[] questPickUps;

		public QwestStart QwestStartPrefab;

		public QwestPoint QwestPointPrefab;

		public QwestVehiclePoint QwestVehiclePointPrefab;

		public Qwest MarkedQwest;

		[HideInInspector]
		public Qwest CurrentTimeQwest;

		public bool MassacreTaskActive;

		[Separator("Achievments")]
		public bool AchievmentsReset;

		public List<Achievement> activeAchievements = new List<Achievement>();

		public List<Achievement> allAchievements = new List<Achievement>();

		public readonly List<Qwest> ActiveQwests = new List<Qwest>();

		private readonly List<Qwest> availableQwests = new List<Qwest>();

		private readonly List<Qwest> allQwests = new List<Qwest>();

		private readonly List<Qwest> nextQuests = new List<Qwest>();

		private class GameEvent : IEvent
		{
			public void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
			{
				this.ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().PlayerDeadEvent(SuicideAchievment.DethType.None);
				});
				this.ByByPass(delegate(Achievement achievement)
				{
					achievement.PlayerDeadEvent(i);
				});
			}

			public void NpcKilledEvent(Vector3 position, Faction npcFaction, HitEntity victim, HitEntity killer)
			{
				this.ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().NpcKilledEvent(position, npcFaction, victim, killer);
				});
				this.ByByPass(delegate(Achievement achievement)
				{
					achievement.NpcKilledEvent(position, npcFaction, victim, killer);
				});
			}

			public void PickedQwestItemEvent(Vector3 position, QwestPickupType pickupType, BaseTask relatedTask)
			{
				this.ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().PickedQwestItemEvent(position, pickupType, relatedTask);
				});
				this.ByByPass(delegate(Achievement achievement)
				{
					achievement.PickedQwestItemEvent(position, pickupType, relatedTask);
				});
			}

			public void PointReachedEvent(Vector3 position, BaseTask task)
			{
				this.ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().PointReachedEvent(position, task);
				});
				this.ByByPass(delegate(Achievement achievement)
				{
					achievement.PointReachedEvent(position, task);
				});
			}

			public void PointReachedByVehicleEvent(Vector3 position, BaseTask task, DrivableVehicle vehicle)
			{
				this.ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().PointReachedByVehicleEvent(position, task, vehicle);
				});
				this.ByByPass(delegate(Achievement achievement)
				{
					achievement.PointReachedByVehicleEvent(position, task, vehicle);
				});
			}

			public void GetIntoVehicleEvent(DrivableVehicle vehicle)
			{
				this.ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().GetIntoVehicleEvent(vehicle);
				});
				this.ByByPass(delegate(Achievement achievement)
				{
					achievement.GetIntoVehicleEvent(vehicle);
				});
			}

			public void GetOutVehicleEvent(DrivableVehicle vehicle)
			{
				this.ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().GetOutVehicleEvent(vehicle);
				});
				this.ByByPass(delegate(Achievement achievement)
				{
					achievement.GetOutVehicleEvent(vehicle);
				});
			}

			public void PickUpCollectionEvent(string CollectionName)
			{
				this.ByByPass(delegate(Achievement achievement)
				{
					achievement.PickUpCollectionEvent(CollectionName);
				});
			}

			public void GetLevelEvent(int level)
			{
				this.ByByPass(delegate(Achievement achievement)
				{
					achievement.GetLevelEvent(level);
				});
			}

			public void GetShopEvent()
			{
				this.ByByPass(delegate(Achievement achievement)
				{
					achievement.GetShopEvent();
				});
			}

			public void VehicleDrawingEvent(DrivableVehicle vehicle)
			{
				this.ByByPass(delegate(Achievement achievement)
				{
					achievement.VehicleDrawingEvent(vehicle);
				});
			}

			public void BuyItemEvent(GameItem item)
			{
				this.ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().BuyItemEvent(item);
				});
			}

			private void ByPass(GameEventManager.GameEvent.QwestAction action)
			{
				foreach (Qwest qwest in GameEventManager.Instance.ActiveQwests.ToArray())
				{
					if (!GameEventManager.Instance.TimeQwestActive || GameEventManager.Instance.CurrentTimeQwest.Equals(qwest))
					{
						action(qwest);
					}
				}
			}

			private void ByByPass(GameEventManager.GameEvent.AchievementAction action)
			{
				foreach (Achievement achievement in GameEventManager.Instance.activeAchievements.ToArray())
				{
					action(achievement);
				}
			}

			private delegate void QwestAction(Qwest qwest);

			private delegate void AchievementAction(Achievement achievement);
		}
	}
}
