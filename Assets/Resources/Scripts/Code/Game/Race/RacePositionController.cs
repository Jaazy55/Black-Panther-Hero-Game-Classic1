using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Game.Race
{
	public class RacePositionController : MonoBehaviour
	{
		public static RacePositionController Instance
		{
			get
			{
				if (RacePositionController.instance == null)
				{
					RacePositionController.instance = UnityEngine.Object.FindObjectOfType<RacePositionController>();
				}
				return RacePositionController.instance;
			}
		}

		private void Awake()
		{
			this.raceTable = new List<Racer>();
		}

		public Racer AddItem(Racer racer, bool isPlayer = false)
		{
			if (isPlayer)
			{
				this.playerRacer = racer;
			}
			this.raceTable.Add(racer);
			return racer;
		}

		public int GetIndexOfItem(Racer racer)
		{
			return this.raceTable.IndexOf(racer, 0);
		}

		public Racer GetPlayerRacer()
		{
			return this.playerRacer;
		}

		public List<Racer> GetRaceTable()
		{
			return this.raceTable;
		}

		private void Update()
		{
			this.raceTable.Sort();
		}

		private static RacePositionController instance;

		[SerializeField]
		private List<Racer> raceTable;

		private Racer playerRacer;
	}
}
