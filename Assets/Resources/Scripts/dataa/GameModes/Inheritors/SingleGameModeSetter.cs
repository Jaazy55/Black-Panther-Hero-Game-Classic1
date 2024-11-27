using System;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	public class SingleGameModeSetter : GameModeSetter
	{
		public override void SetGameMod()
		{
			this.m_ModeGetter.SetGameMode(this.m_SettableGameMode);
		}

		[SerializeField]
		private VariableGameModeGetter m_ModeGetter;

		[SerializeField]
		private GameMode m_SettableGameMode;
	}
}
