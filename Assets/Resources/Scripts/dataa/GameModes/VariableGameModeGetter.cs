using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Naxeex.GameModes
{
	[CreateAssetMenu(fileName = "Variable Game Mode Getter", menuName = "Game Modes/Variable Game Mode Getter")]
	public class VariableGameModeGetter : GameModeGetter
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction<GameMode> ChangeGameMode;

		public void SetGameMode(GameMode gameMode)
		{
			this.m_Mode = gameMode;
			if (this.ChangeGameMode != null)
			{
				this.ChangeGameMode(this.m_Mode);
			}
		}

		public override GameMode GetGameMode()
		{
			return this.m_Mode;
		}

		[SerializeField]
		private GameMode m_Mode;
	}
}
