using System;
using UnityEngine;

namespace Naxeex.GameModes.UI
{
	public abstract class InfoManager : ScriptableObject
	{
		public virtual bool IsProcess(GameMode gameMode)
		{
			return gameMode == this.ManagmentObject;
		}

		public virtual void BeginProcess(GameModeInfo infoPanel)
		{
		}

		public virtual void UpdateProcess(GameModeInfo infoPanel)
		{
		}

		public virtual void EndProcess(GameModeInfo infoPanel)
		{
		}

		[SerializeField]
		private GameMode ManagmentObject;
	}
}
