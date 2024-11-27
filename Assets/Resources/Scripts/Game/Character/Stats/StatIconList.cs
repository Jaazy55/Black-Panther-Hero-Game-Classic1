using System;
using UnityEngine;

namespace Game.Character.Stats
{
	[CreateAssetMenu(fileName = "IconList", menuName = "Library/Create Icon stat List", order = 1)]
	public class StatIconList : BaseListScriptable<StatIcon>
	{
		public StatIcon GetDefinition(StatsList typeStat)
		{
			int count = base.Count;
			for (int i = 0; i < count; i++)
			{
				if (base[i].StatType == typeStat)
				{
					return base[i];
				}
			}
			return default(StatIcon);
		}
	}
}
