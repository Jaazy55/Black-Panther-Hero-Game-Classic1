using System;
using Naxeex.SpawnSystem;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Wave NPC Generator Interpeter", menuName = "Game Modes/Interpeters/NPC Generator/Wave NPC Generator Interpeter")]
	public class WaveNPCGeneratorInterpreter : NPCGeneratorInterpreter
	{
		protected override Generator Generator
		{
			get
			{
				return this.m_Generator ?? this.EmptyGenerator;
			}
		}

		public override void RuleBegin()
		{
			base.RuleBegin();
			Manager.OnFinal += this.FinalHanlder;
			ArenaWave.Reset();
			ArenaWave.OnNumberUpdate += this.ArenaWaveHandler;
			ArenaWave.Next();
		}

		public override void RuleProcess()
		{
			base.RuleProcess();
			if (this.m_Generator != null && !this.m_Generator.HasCurrent && this.Entities.Count == 0)
			{
				ArenaWave.Next();
			}
		}

		public override void RuleRestart()
		{
			base.RuleRestart();
			ArenaWave.Reset();
			ArenaWave.Next();
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
			Manager.OnFinal -= this.FinalHanlder;
			ArenaWave.OnNumberUpdate -= this.ArenaWaveHandler;
			ArenaWave.Reset();
		}

		private void ArenaWaveHandler(int currentWave)
		{
			int num = currentWave;
			int num2 = 0;
			while (num2 < this.dataArrays.Length && num >= 0 && this.dataArrays[num2].CycleCount > 0)
			{
				num -= this.dataArrays[num2].CycleCount * this.dataArrays[num2].Database.Length;
				num2++;
			}
			if (num2 < 0)
			{
				this.m_Generator = null;
			}
			int num3 = num % this.dataArrays[num2].Database.Length;
			if (num3 < 0)
			{
				num3 += this.dataArrays[num2].Database.Length;
			}
			this.m_Generator = new DataGenerator(this.dataArrays[num2].Database[num3]);
		}

		private void FinalHanlder()
		{
			this.m_Generator = null;
			ArenaWave.SaveCurrentValue();
		}

		[SerializeField]
		private CycleGenerableDataArray[] dataArrays;

		private Generator m_Generator;

		private Generator EmptyGenerator = new EmptyGenerator();
	}
}
