using System;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Random Game Mode Getter", menuName = "Game Modes/Random Game Mode Getter")]
	public class RandomGameModeGetter : GameModeGetter
	{
		public override GameMode GetGameMode()
		{
			int num = UnityEngine.Random.Range(0, this.m_Modes.Length);
			return this.m_Modes[num];
		}

		[SerializeField]
		private GameMode[] m_Modes;
	}
}
