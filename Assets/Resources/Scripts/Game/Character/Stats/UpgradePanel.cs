using System;
using UnityEngine;

namespace Game.Character.Stats
{
	[Serializable]
	public class UpgradePanel
	{
		public string name;

		public PanelsList type;

		public GameObject OnState;

		public GameObject OffState;
	}
}
