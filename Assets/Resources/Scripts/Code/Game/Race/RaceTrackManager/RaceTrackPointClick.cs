using System;
using System.Collections.Generic;
using Game.Traffic;
using UnityEngine;

namespace Code.Game.Race.RaceTrackManager
{
	[ExecuteInEditMode]
	public class RaceTrackPointClick : MonoBehaviour
	{
		public static bool IsSelectedRaceTraffic()
		{
			return RaceTrackPointClick.selectRaceTraffic;
		}

		public List<GameObject> GetRaceNodesList()
		{
			return this.raceNodesList;
		}

		public GameObject GetRaceNode()
		{
			return this.raceNode;
		}

		public void SetRaceNode(GameObject raceNode)
		{
			this.raceNode = raceNode;
		}

		public static RaceTrackPointClick Instance
		{
			get
			{
				if (RaceTrackPointClick.instance == null)
				{
					RaceTrackPointClick.instance = UnityEngine.Object.FindObjectOfType<RaceTrackPointClick>();
					if (RaceTrackPointClick.instance == null)
					{
						UnityEngine.Debug.LogError("RaceTrackPointClick отсутствует на сцене");
					}
				}
				return RaceTrackPointClick.instance;
			}
		}

		public void CtrlZ()
		{
			if (this.raceNodesList.Count > 0)
			{
				UnityEngine.Object.DestroyImmediate(this.raceNodesList[this.raceNodesList.Count - 1]);
				this.raceNodesList.Remove(this.raceNodesList[this.raceNodesList.Count - 1]);
				if (this.raceNodesList.Count <= 0)
				{
					return;
				}
				GameObject gameObject = this.raceNodesList[this.raceNodesList.Count - 1];
				if (gameObject.transform.parent.name == "RaceNodes")
				{
					this.raceNode = gameObject;
				}
			}
		}

		public void ClearLastLinks()
		{
			if (this.raceNodesList.Count > 0)
			{
				this.raceNodesList[this.raceNodesList.Count - 1].GetComponent<Node>().NodeLinks.Clear();
			}
		}

		private static RaceTrackPointClick instance;

		private static bool selectRaceTraffic;

		[SerializeField]
		private List<GameObject> raceNodesList = new List<GameObject>();

		[SerializeField]
		private GameObject raceNode;

		[SerializeField]
		private bool raceTrafficSelected;

		private bool selectCarTrafficCurrState;
	}
}
