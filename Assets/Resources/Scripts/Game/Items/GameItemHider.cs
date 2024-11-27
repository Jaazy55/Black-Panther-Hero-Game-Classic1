using System;
using UnityEngine;

namespace Game.Items
{
	public abstract class GameItemHider : ScriptableObject
	{
		public abstract bool IsHide { get; }
	}
}
