using System;
using UnityEngine.Events;

namespace Naxeex.GameModes
{
	public interface IGameModeManager
	{
		GameMode CurrentMod { get; set; }

		bool IsActivated { get; }

		event UnityAction OnActivate;

		event UnityAction OnDeactivate;

		event UnityAction OnRestart;

		event UnityAction OnFinal;

		event UnityAction<GameMode> OnChangeGameMode;

		void Activate();

		void Deactivate();

		void Restart();

		void Final();
	}
}
