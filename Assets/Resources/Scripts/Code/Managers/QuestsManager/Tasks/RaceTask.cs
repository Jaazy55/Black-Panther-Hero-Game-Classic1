using System;
using Code.Game.Race;
using Code.Game.Race.Utils;
using Game.GlobalComponent.Qwest;
using Game.Vehicle;
using UnityEngine;

namespace Code.Managers.QuestsManager.Tasks
{
	public class RaceTask : BaseTask
	{
		public override void TaskStart()
		{
			base.TaskStart();
			RaceManager.OnRaceStateChanged += this.CheckForComplete;
			ResultTableInfoPanel.OnRacerFinished += this.CheckForLose;
			QuestToRaceAdapter.Instance.InitQuest(this.RaceNumber, this.CurrentQwest);
		}

		private void CheckForLose(int opponentPlace, int playerPlace)
		{
			this.playerPlace = playerPlace;
			if (playerPlace > 0)
			{
				return;
			}
			int minPlaceForWin = RaceManager.Instance.GetCurrentRace().GetMinPlaceForWin();
			if (opponentPlace == minPlaceForWin)
			{
				GameEventManager.Instance.QwestFailed(this.CurrentQwest);
			}
		}

		private void CheckForComplete(RaceState raceState)
		{
			this.raceState = raceState;
			if (raceState == RaceState.Finish)
			{
				this.CurrentQwest.Rewards = RaceManager.Instance.GetCurrentRace().GetRewards()[this.playerPlace - 1];
				this.CurrentQwest.MoveToTask(this.NextTask);
			}
		}

		public override void Cancel()
		{
			base.Cancel();
			RaceManager.OnRaceStateChanged -= this.CheckForComplete;
			ResultTableInfoPanel.OnRacerFinished -= this.CheckForLose;
		}

		public override void Finished()
		{
			base.Finished();
			if (this.raceState != RaceState.Finish)
			{
				RaceManager.Instance.DeInit();
			}
			RaceManager.OnRaceStateChanged -= this.CheckForComplete;
			ResultTableInfoPanel.OnRacerFinished -= this.CheckForLose;
		}

		public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
		{
			QwestTimerManager.Instance.EndCountdown();
			GameEventManager.Instance.QwestFailed(this.CurrentQwest);
		}

		public override void GetOutVehicleEvent(DrivableVehicle vehicle)
		{
			if (vehicle.CompareTag("PlayerRacer"))
			{
				QwestTimerManager.Instance.StartCountdown(30f, this.CurrentQwest);
				this.searchGo = new GameObject
				{
					name = "SearchProcessing_" + base.GetType().Name
				};
				SearchProcess<DrivableVehicle> process = new SearchProcess<DrivableVehicle>(new Func<DrivableVehicle, bool>(this.CheckCondition))
				{
					countMarks = 1,
					markType = RaceManager.Instance.GetCurrentRace().GetMarksType()
				};
				SearchProcessing searchProcessing = this.searchGo.AddComponent<SearchProcessing>();
				searchProcessing.process = process;
				searchProcessing.Init();
			}
		}

		public override void GetIntoVehicleEvent(DrivableVehicle vehicle)
		{
			if (vehicle.CompareTag("PlayerRacer"))
			{
				if (this.searchGo)
				{
					UnityEngine.Object.Destroy(this.searchGo);
				}
				QwestTimerManager.Instance.EndCountdown();
			}
		}

		private bool CheckCondition(DrivableVehicle vehicle)
		{
			return vehicle.CompareTag("PlayerRacer");
		}

		public int RaceNumber;

		private RaceState raceState;

		private GameObject searchGo;

		private int playerPlace;
	}
}
