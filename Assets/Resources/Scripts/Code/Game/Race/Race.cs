using System;
using System.Linq;
using Code.Game.Race.RaceTrackManager;
using Code.Game.Race.StartGrid;
using Game.GlobalComponent.Qwest;
using UnityEngine;

namespace Code.Game.Race
{
	public class Race : MonoBehaviour
	{
		public Racer[] GetRacers()
		{
			return this.racers;
		}

		public Transform GetStartPoint()
		{
			return this.startPoint;
		}

		public Transform[] GetCheckPoints()
		{
			return (from obj in this.raceCheckPoints.GetRaceNodesList()
			select obj.transform).ToArray<Transform>();
		}

		public int GetLapsCount()
		{
			return this.lapsCount;
		}

		public int GetPlayerPlaceOnStartGrid()
		{
			return this.playerPlaceOnStartGrid;
		}

		public void SetPlayerPlaceOnStartGrid(int place)
		{
			this.playerPlaceOnStartGrid = place;
		}

		public StartGrid.StartGrid GetGrid()
		{
			return this.grid;
		}

		public int GetMinPlaceForWin()
		{
			return this.minPlaceForWin;
		}

		public UniversalReward[] GetRewards()
		{
			return this.rewards;
		}

		public string GetMarksType()
		{
			return this.marksType;
		}

		[SerializeField]
		private Racer[] racers;

		[SerializeField]
		private Transform startPoint;

		[SerializeField]
		private RaceTrackPointClick raceCheckPoints;

		[SerializeField]
		private int lapsCount;

		[SerializeField]
		private int playerPlaceOnStartGrid;

		[SerializeField]
		private StartGrid.StartGrid grid;

		[SerializeField]
		private int minPlaceForWin;

		[SerializeField]
		private UniversalReward[] rewards;

		[SerializeField]
		[SelectiveString("MarkType")]
		private string marksType;
	}
}
