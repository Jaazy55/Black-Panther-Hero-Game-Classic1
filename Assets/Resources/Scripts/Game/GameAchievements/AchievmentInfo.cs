using System;
using UnityEngine;

namespace Game.GameAchievements
{
	public class AchievmentInfo
	{
		public string id = "NoNameAchievment";

		public string description = "NoNameAchievment";

		public Sprite icon;

		public bool isDone;

		public int currentProgress;

		public int targetValue;

		public bool showFullInfo;
	}
}
