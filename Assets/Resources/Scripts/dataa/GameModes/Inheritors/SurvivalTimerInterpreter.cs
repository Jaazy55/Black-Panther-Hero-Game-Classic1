using System;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Survival Timer Interpeter", menuName = "Game Modes/Interpeters/Survival Timer Interpeter")]
	public class SurvivalTimerInterpreter : RuleInterpreter
	{
		public override void RuleBegin()
		{
			base.RuleBegin();
			Manager.OnFinal += this.StopTimer;
			SurvivalTimer.Start();
		}

		public override void RuleRestart()
		{
			base.RuleRestart();
			SurvivalTimer.Stop();
			SurvivalTimer.Start();
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
			SurvivalTimer.Stop();
		}

		private void StopTimer()
		{
			SurvivalTimer.Stop();
		}
	}
}
