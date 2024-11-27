using System;
using UnityEngine;

namespace Naxeex.GameModes
{
	public abstract class GameModeGetter : ScriptableObject
	{
		public abstract GameMode GetGameMode();
	}
}
