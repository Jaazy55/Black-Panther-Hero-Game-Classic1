using System;
using Naxeex.GameModes;
using UnityEngine;

namespace Roulette
{
	[CreateAssetMenu(fileName = "Survivor Roulette Interpeter", menuName = "Game Modes/Interpeters/Survivor Roulette Interpeter")]
	public class SurvivorRouletteRule : RuleInterpreter
	{
		public override void RuleBegin()
		{
			base.RuleBegin();
			this.LastTimeCheck = 0;
		}

		public override void RuleRestart()
		{
			base.RuleRestart();
			this.LastTimeCheck = 0;
		}

		public override void RuleProcess()
		{
			base.RuleProcess();
			if (!SurvivalRouletteController.HasInstance)
			{
				return;
			}
			int num = SurvivalTimer.CurrentTimeToInt / 60;
			if (num != this.LastTimeCheck)
			{
				this.LastTimeCheck = num;
				if (this.LastTimeCheck > 1 && this.LastTimeCheck % 2 == 1)
				{
					if (this.LastTimeCheck > 40)
					{
						SurvivalRouletteController.Instance.Spins += 4;
					}
					else if (this.LastTimeCheck > 5)
					{
						SurvivalRouletteController.Instance.Spins += 3;
					}
					else if (this.LastTimeCheck == 5)
					{
						SurvivalRouletteController.Instance.Spins += 2;
					}
					else
					{
						SurvivalRouletteController.Instance.Spins++;
					}
				}
			}
		}

		private int LastTimeCheck;
	}
}
