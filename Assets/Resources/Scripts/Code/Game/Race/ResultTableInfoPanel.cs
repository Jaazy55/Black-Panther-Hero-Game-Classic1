using System;
using System.Diagnostics;
using Code.Game.Race.Utils;
using UnityEngine;

namespace Code.Game.Race
{
	public class ResultTableInfoPanel : MonoBehaviour
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event RacerFinished OnRacerFinished;

		public int GetItemPosition()
		{
			return this.itemPosition;
		}

		public int GetPlayerPosition()
		{
			return this.playerPosition;
		}

		public void ClearResultTable()
		{
			this.itemPosition = 0;
			this.playerPosition = 0;
			this.raceItemViewHolder.DestroyChildrens();
		}

		public void AddRaceItem(Racer racer, bool isPlayer = false)
		{
			this.itemPosition++;
			if (isPlayer)
			{
				this.playerPosition = this.itemPosition;
			}
			if (ResultTableInfoPanel.OnRacerFinished != null)
			{
				ResultTableInfoPanel.OnRacerFinished(this.itemPosition, this.playerPosition);
			}
			this.AddView(racer);
		}

		private void AddView(Racer racer)
		{
			RaceItemView raceItemView = UnityEngine.Object.Instantiate<RaceItemView>(this.raceItemViewPrefab, this.raceItemViewHolder, false);
			if (raceItemView == null)
			{
				return;
			}
			raceItemView.SetPosition(this.itemPosition);
			raceItemView.SetRacerName(racer.GetRacerName());
		}

		private void OnDestroy()
		{
			ResultTableInfoPanel.OnRacerFinished = null;
		}

		[SerializeField]
		private RaceItemView raceItemViewPrefab;

		[SerializeField]
		private RectTransform raceItemViewHolder;

		private int itemPosition;

		private int playerPosition;
	}
}
