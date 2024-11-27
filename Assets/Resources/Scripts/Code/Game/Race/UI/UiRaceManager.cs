using System;
using System.Collections;
using Code.Game.Race.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Game.Race.UI
{
	public class UiRaceManager : MonoBehaviour
	{
		public static UiRaceManager Instance
		{
			get
			{
				if (UiRaceManager.instance == null)
				{
					throw new Exception("UiRaceManager is not initialized");
				}
				return UiRaceManager.instance;
			}
		}

		public AnimationClip GetAnimationCountdown()
		{
			return this.animationCountdown;
		}

		public void AddItemToResultTable(Racer racer, bool isPlayer = false)
		{
			this.resultTableInfoPanelView.AddRaceItem(racer, isPlayer);
		}

		private void Awake()
		{
			UiRaceManager.instance = this;
		}

		private void Start()
		{
			RaceManager.OnRaceStateChanged += this.OnRaceStateChanged;
		}

		private void OnRaceStateChanged(RaceState raceState)
		{
			this.raceState = raceState;
			switch (this.raceState)
			{
			case RaceState.BeforeStart:
				this.ActivateRaceUiElements(false);
				this.resultTableInfoPanelView.gameObject.SetActive(false);
				this.resultTableInfoPanelView.ClearResultTable();
				break;
			case RaceState.Start:
				base.StartCoroutine(this.CountDown());
				break;
			case RaceState.Race:
				this.ActivateRaceUiElements(true);
				this.resultTableInfoPanelView.ClearResultTable();
				break;
			case RaceState.Finish:
				this.ActivateRaceUiElements(false);
				this.resultTableInfoPanelView.gameObject.SetActive(true);
				this.Finish();
				break;
			default:
				throw new ArgumentOutOfRangeException("raceState", raceState, "Unsupported type of RaceState");
			}
		}

		private void ActivateRaceUiElements(bool value)
		{
			this.racePositionView.gameObject.SetActive(value);
			if (RaceManager.Instance.GetLaps() > 1)
			{
				this.lapNumberView.gameObject.SetActive(value);
			}
		}

		private IEnumerator CountDown()
		{
			float animTime = this.animationCountdown.length;
			this.countdown.SetBool("StartCountdown", true);
			yield return new WaitForSeconds(animTime);
			yield return new WaitForSeconds(1f);
			this.countdown.SetBool("StartCountdown", false);
			yield break;
		}

		private void Update()
		{
			this.RefreshPosition();
			this.RefreshLapNumber();
		}

		private void RefreshPosition()
		{
			if (this.raceState == RaceState.Race)
			{
				this.racePositionView.text = RacePositionController.Instance.GetIndexOfItem(RacePositionController.Instance.GetPlayerRacer()) + 1 + "/" + RacePositionController.Instance.GetRaceTable().Count;
			}
		}

		private void RefreshLapNumber()
		{
			if (this.raceState == RaceState.Race && RaceManager.Instance.GetLaps() > 1)
			{
				this.lapNumberView.text = string.Concat(new object[]
				{
					"LAP ",
					RaceManager.Instance.GetCurrentLap() + 1,
					"/",
					RaceManager.Instance.GetLaps()
				});
			}
		}

		private void Finish()
		{
			int itemPosition = this.resultTableInfoPanelView.GetItemPosition();
			for (int i = itemPosition; i < RacePositionController.Instance.GetRaceTable().Count; i++)
			{
				this.AddItemToResultTable(RacePositionController.Instance.GetRaceTable()[i], false);
			}
		}

		private static UiRaceManager instance;

		private RaceState raceState;

		[SerializeField]
		private Text racePositionView;

		[SerializeField]
		private Text lapNumberView;

		[SerializeField]
		private ResultTableInfoPanel resultTableInfoPanelView;

		[SerializeField]
		private Animator countdown;

		[SerializeField]
		private AnimationClip animationCountdown;
	}
}
