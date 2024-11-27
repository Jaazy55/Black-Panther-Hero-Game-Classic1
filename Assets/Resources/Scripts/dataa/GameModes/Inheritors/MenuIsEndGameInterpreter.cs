using System;
using System.Runtime.CompilerServices;
using Game.UI;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Menu Is End Game Interpeter", menuName = "Game Modes/Interpeters/Menu Is End Game Interpeter")]
	public class MenuIsEndGameInterpreter : RuleInterpreter
	{
		public override void RuleBegin()
		{
			UIGame instance = UIGame.Instance;
			Delegate onExitInMenu = instance.OnExitInMenu;
			if (MenuIsEndGameInterpreter._003C_003Ef__mg_0024cache0 == null)
			{
				MenuIsEndGameInterpreter._003C_003Ef__mg_0024cache0 = new Action(Manager.Final);
			}
			instance.OnExitInMenu = (Action)Delegate.Combine(onExitInMenu, MenuIsEndGameInterpreter._003C_003Ef__mg_0024cache0);
		}

		public override void RuleEnd()
		{
			if (UIGame.Instance != null)
			{
				UIGame instance = UIGame.Instance;
				Delegate onExitInMenu = instance.OnExitInMenu;
				if (MenuIsEndGameInterpreter._003C_003Ef__mg_0024cache1 == null)
				{
					MenuIsEndGameInterpreter._003C_003Ef__mg_0024cache1 = new Action(Manager.Final);
				}
				instance.OnExitInMenu = (Action)Delegate.Remove(onExitInMenu, MenuIsEndGameInterpreter._003C_003Ef__mg_0024cache1);
			}
		}

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache1;
	}
}
