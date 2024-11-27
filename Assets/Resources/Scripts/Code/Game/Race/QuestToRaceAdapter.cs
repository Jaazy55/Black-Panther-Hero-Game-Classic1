using System;
using System.Collections;
using Game.GlobalComponent.Qwest;
using Game.UI;
using UnityEngine;

namespace Code.Game.Race
{
	public class QuestToRaceAdapter : MonoBehaviour
	{
		public static QuestToRaceAdapter Instance
		{
			get
			{
				if (QuestToRaceAdapter.instance == null)
				{
					throw new Exception("QuestToRaceAdapter is not initialized");
				}
				return QuestToRaceAdapter.instance;
			}
		}

		public void InitQuest(int raceNumber, Qwest qwest)
		{
			base.StartCoroutine(this.StartUniversalYesNoPanel(raceNumber, qwest));
		}

		private void Awake()
		{
			QuestToRaceAdapter.instance = this;
		}

		private IEnumerator StartUniversalYesNoPanel(int raceNumber, Qwest qwest)
		{
			yield return new WaitForSeconds(0.1f);
			UniversalYesNoPanel.Instance.DisplayOffer(null, this.displayText, delegate ()
			{
				RaceManager.Instance.InitRace(raceNumber);
			}, delegate ()
			{
				this.StartCoroutine(this.Timer(this.delayTime, qwest));
			}, false);
			yield break;
		}

		private IEnumerator Timer(int sec, Qwest qwest)
		{
			GameEventManager.Instance.QwestCancel(qwest);
			yield return new WaitForSeconds((float)sec);
			GameEventManager.Instance.ResetQwestCancel(qwest);
			yield break;
		}

		private static QuestToRaceAdapter instance;

		[SerializeField]
		private string displayText = "Are you sure you want to start the race?";

		[SerializeField]
		private int delayTime = 10;
	}
}
