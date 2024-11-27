using System;
using System.Collections;
using System.Diagnostics;
using Code.Game.Race.UI;
using Code.Game.Race.Utils;
using Game.Character;
using Game.GlobalComponent;
using Game.Traffic;
using Game.Vehicle;
using UnityEngine;

namespace Code.Game.Race
{
	public class RaceManager : MonoBehaviour
	{
        
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event RaceStateChanged OnRaceStateChanged;

		public static RaceManager Instance
		{
			get
			{
				if (RaceManager.instance == null)
				{
					throw new Exception("RaceManager is not initialized");
				}
				return RaceManager.instance;
			}
		}

		public int GetLaps()
		{
			return this.laps;
		}

		public int GetCurrentLap()
		{
			return this.lapIndex;
		}

		public Race GetCurrentRace()
		{
			return this.currentRace;
		}

		public void InitRace(int raceNumber)
		{
			raceNumber = Mathf.Clamp(raceNumber, 0, this.races.Length - 1);
			this.currentRace = this.races[raceNumber];
			this.laps = this.currentRace.GetLapsCount();
			this.lapIndex = 0;
			this.checkPoints = this.currentRace.GetCheckPoints();
			this.checkpointIndex = 0;
			RacePositionController.Instance.GetRaceTable().Clear();
			this.laps = Mathf.Clamp(this.laps, 1, int.MaxValue);
			this.racers = new Racer[this.currentRace.GetRacers().Length];
			this.isAutopiloted = false;
			this.vehicleSpawnPositions = this.currentRace.GetGrid().CalculateGrid(this.racers.Length + 1);
			this.vehicleSpawnPositions = RaceUtils.CalculateGridAdvanced(this.vehicleSpawnPositions, this.currentRace.GetStartPoint().rotation);
			this.currentRace.SetPlayerPlaceOnStartGrid(Mathf.Clamp(this.currentRace.GetPlayerPlaceOnStartGrid(), 1, this.vehicleSpawnPositions.Length));
			RaceUtils.SwapElements<Vector3>(this.vehicleSpawnPositions, this.currentRace.GetPlayerPlaceOnStartGrid() - 1, this.vehicleSpawnPositions.Length - 1);
			this.SpawnRacers();
		}

		private void SpawnRacers()
		{
			TrafficManager.Instance.AllowSpawnTraffic(false);
			for (int i = 0; i < this.racers.Length; i++)
			{
				Vector3 bestPoint = this.currentRace.GetStartPoint().position + this.vehicleSpawnPositions[i];
				DrivableVehicle drivableVehicle = this.currentRace.GetRacers()[i].GetDrivableVehicle();
				DrivableVehicle drivableVehicle2 = TrafficManager.Instance.SpawnConcreteVehicleForRace(drivableVehicle, bestPoint, "Racer", true);
				this.racers[i] = new Racer(drivableVehicle2, this.currentRace.GetRacers()[i].GetRacerName());
				this.racers[i].GetDrivableVehicle().transform.rotation = this.currentRace.GetStartPoint().rotation;
			}
			base.StartCoroutine(this.InitPlayerRacer());
		}

		public void DeInit()
		{
			TrafficManager.Instance.AllowSpawnTraffic(true);
			if (this.currentRaceState != RaceState.Finish)
			{
				this.QuitRace();
			}
			else
			{
				GameplayUtils.ResumeGame();
			}
			this.SetRaceState(RaceState.BeforeStart);
			RaceUtils.BlockVehicleControl(this.playerDrivableVehicle.controller, false);
			base.StopAllCoroutines();
			this.AddAutopilotsForRacers();
		}

		private void Finish()
		{
			GameplayUtils.PauseGame();
			this.QuitRace();
			this.SetRaceState(RaceState.Finish);
			RaceUtils.BlockVehicleControl(this.playerDrivableVehicle.controller, true);
          //  Firebase.Analytics.FirebaseAnalytics.LogEvent("success");  //Hussain

        }

        private void QuitRace()
		{

			RaceUtils.RefreshCheckPointArrow(null, false);
			UnityEngine.Object.Destroy(this.nextPointHighlighter);
			foreach (Racer racer in this.racers)
			{
				racer.GetDrivableVehicle().tag = "Untagged";
			}
			this.playerDrivableVehicle.tag = "Untagged";
			// Firebase.Analytics.FirebaseAnalytics.LogEvent("quit");   //Hussain
		}

		private void Awake()
		{
			RaceManager.instance = this;
		}

		private void FixedUpdate()
		{
			if (this.playerDrivableVehicle != null && this.playerDrivableVehicle.CompareTag("PlayerRacer") && this.currentRaceState == RaceState.Race)
			{
				this.RaceFixedUpdate();
			}
		}

		private void EnterToCutscene()
		{
			this.enterCutsceneManager.Init(new CutsceneManager.Callback(this.ExitFromCutscene), new CutsceneManager.Callback(this.ExitFromCutscene));
		}

		private void ExitFromCutscene()
		{
			this.exitCutsceneManager.Init(new CutsceneManager.Callback(this.Callback), new CutsceneManager.Callback(this.Callback));
		}

		private void Callback()
		{
			UnityEngine.Debug.Log("Callback");
		}

		private IEnumerator InitPlayerRacer()
		{
			yield return new WaitForSeconds(0.5f);
			this.EnterToCutscene();
			yield return new WaitForSeconds(1f);
			Vector3 driverVehicleSpawnPosition = this.GetDriverVehicleSpawnPosition();
			DrivableVehicle playerDrivableVehiclePrefab = this.garage.ObjectPrefab.GetComponent<DrivableVehicle>();
			this.playerDrivableVehicle = TrafficManager.Instance.SpawnConcreteVehicleForRace(playerDrivableVehiclePrefab, driverVehicleSpawnPosition, "PlayerRacer", false);
			this.playerDrivableVehicle.transform.rotation = this.currentRace.GetStartPoint().rotation;
			PlayerInteractionsManager.Instance.Player.transform.position = this.playerDrivableVehicle.GetExitPosition(false);
			PlayerInteractionsManager.Instance.LastDrivableVehicle = this.playerDrivableVehicle;
			PlayerInteractionsManager.Instance.GetIntoVehicle();
			yield return new WaitUntil(() => this.playerDrivableVehicle.controller != null);
			RaceUtils.BlockVehicleControl(this.playerDrivableVehicle.controller, true);
			yield return new WaitForSeconds(6f);
			this.playerRacer = new Racer(this.playerDrivableVehicle, "Player");
			RacePositionController.Instance.AddItem(this.playerRacer, true);
			base.StartCoroutine(this.StartRace());
			yield break;
		}

		private IEnumerator StartRace()
		{
			this.SetRaceState(RaceState.Start);
			if (PlayerInteractionsManager.Instance.sitInVehicle && this.playerDrivableVehicle.controller.VehicleSpecific.HasRadio)
			{
				//RadioManager.Instance.DisableRadio();
			}
			float animTime = UiRaceManager.Instance.GetAnimationCountdown().length;
			yield return new WaitForSeconds(animTime);
			if (PlayerInteractionsManager.Instance.sitInVehicle && this.playerDrivableVehicle.controller.VehicleSpecific.HasRadio)
			{
				//RadioManager.Instance.EnableRadio();
			}
			this.SetRaceState(RaceState.Race);
			TrafficManager.Instance.AllowSpawnTraffic(true);
			RaceUtils.BlockVehicleControl(this.playerDrivableVehicle.controller, false);
			this.AddAutopilotsForRacers();
			this.nextPointHighlighter = UnityEngine.Object.Instantiate<GameObject>(this.nextPointHighlighterPrefab);
			this.nextPointHighlighter.transform.position = this.checkPoints[0].transform.position;
			RaceUtils.RefreshCheckPointArrow(this.checkPoints[this.checkpointIndex].transform, this.currentRaceState == RaceState.Race);
			yield break;
		//	Firebase.Analytics.FirebaseAnalytics.LogEvent("stratRace");   //Hussain

		}

		private void AddAutopilotsForRacers()
		{
			if (this.isAutopiloted)
			{
				return;
			}
			foreach (Racer racer in this.racers)
			{
				TrafficManager.Instance.AddAutopilotForRacer(racer, this.checkPoints, this.laps);
			}
			this.isAutopiloted = true;
		}

		private Vector3 GetDriverVehicleSpawnPosition()
		{
			return this.currentRace.GetStartPoint().position + this.vehicleSpawnPositions[this.vehicleSpawnPositions.Length - 1];
		}

		private void RaceFixedUpdate()
		{
			Transform transform = this.checkPoints[this.checkpointIndex];
			this.distanceToPoint = Vector3.Distance(this.playerDrivableVehicle.transform.position, transform.position);
			this.playerRacer.SetLap(this.lapIndex);
			this.playerRacer.SetWaypointIndex(this.checkpointIndex);
			this.playerRacer.SetDistanceToPoint(this.distanceToPoint);
			if (this.distanceToPoint < 30f)
			{
				this.checkpointIndex = (this.checkpointIndex + 1) % this.checkPoints.Length;
				this.nextPointHighlighter.transform.position = this.checkPoints[this.checkpointIndex].transform.position;
				if (this.checkpointIndex == 0)
				{
					this.lapIndex++;
					if (this.laps == this.lapIndex)
					{
						UiRaceManager.Instance.AddItemToResultTable(this.playerRacer, true);
						this.Finish();
					}
				}
				RaceUtils.RefreshCheckPointArrow(this.checkPoints[this.checkpointIndex].transform, this.currentRaceState == RaceState.Race);
			}
		}

		private void SetRaceState(RaceState raceState)
		{
			this.currentRaceState = raceState;
			if (RaceManager.OnRaceStateChanged != null)
			{
				RaceManager.OnRaceStateChanged(this.currentRaceState);
			}
		}

		private void OnDestroy()
		{
			RaceManager.OnRaceStateChanged = null;
		}

		private const float CheckDistance = 30f;

		private static RaceManager instance;

		[SerializeField]
		private GameObject nextPointHighlighterPrefab;

		[SerializeField]
		private ControlableObjectRespawner garage;

		[SerializeField]
		private Race[] races;

		[SerializeField]
		private CutsceneManager enterCutsceneManager;

		[SerializeField]
		private CutsceneManager exitCutsceneManager;

		private RaceState currentRaceState;

		private GameObject nextPointHighlighter;

		private DrivableVehicle playerDrivableVehicle;

		private Race currentRace;

		private Racer[] racers;

		private bool isAutopiloted;

		private Vector3[] vehicleSpawnPositions;

		private Transform[] checkPoints;

		private int checkpointIndex;

		private float distanceToPoint;

		private int laps;

		private int lapIndex;

		private Racer playerRacer;
	}
}
