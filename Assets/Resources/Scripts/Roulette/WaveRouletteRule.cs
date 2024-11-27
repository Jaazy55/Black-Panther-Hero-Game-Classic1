using System;
using Naxeex.GameModes;
using UnityEngine;

namespace Roulette
{
	[CreateAssetMenu(fileName = "Wave Roulette Interpeter", menuName = "Game Modes/Interpeters/Wave Roulette Interpeter")]
	public class WaveRouletteRule : RuleInterpreter
	{
		public override void RuleBegin()
		{
			base.RuleBegin();
			ArenaWave.OnNumberUpdate += this.WaveHandler;
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
			ArenaWave.OnNumberUpdate -= this.WaveHandler;
		}

		private void WaveHandler(int wave)
		{
			if (!WaveRouletteController.HasInstance)
			{
				return;
			}
			if (wave > 1 && wave % 2 == 1)
			{
				if (wave > 40)
				{
					WaveRouletteController.Instance.Spins += 4;
				}
				else if (wave > 5)
				{
					WaveRouletteController.Instance.Spins += 3;
				}
				else if (wave == 5)
				{
					WaveRouletteController.Instance.Spins += 2;
				}
				else
				{
					WaveRouletteController.Instance.Spins++;
				}
			}
		}
	}
}
