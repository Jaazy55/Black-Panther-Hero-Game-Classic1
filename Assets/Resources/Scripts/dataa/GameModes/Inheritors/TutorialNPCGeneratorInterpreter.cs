using System;
using Naxeex.SpawnSystem;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	public class TutorialNPCGeneratorInterpreter : NPCGeneratorInterpreter
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
			if (this.SimpleGenerator == null)
			{
				this.SimpleGenerator = new DataGenerator(this.m_GeneratorData);
			}
			if (this.SimpleGenerator != null)
			{
				this.SimpleGenerator.Reset();
				this.m_Generator = this.SimpleGenerator;
			}
		}

		public override void RuleRestart()
		{
			base.RuleRestart();
			if (this.SimpleGenerator != null)
			{
				this.SimpleGenerator.Reset();
				this.m_Generator = this.SimpleGenerator;
			}
		}

		public override void RuleProcess()
		{
			base.RuleProcess();
			if (this.m_Generator != null && !this.m_Generator.HasCurrent && this.Entities.Count == 0)
			{
				this.m_Generator = null;
				ArenaTutorial.State = ArenaTutorial.TutorialState.RoulettleClick;
				Manager.Final();
			}
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
			Manager.OnFinal -= this.FinalHanlder;
		}

		private void FinalHanlder()
		{
			this.m_Generator = null;
			ArenaWave.SaveCurrentValue();
		}

		[SerializeField]
		private GenerableData m_GeneratorData;

		private Generator m_Generator;

		private Generator EmptyGenerator = new EmptyGenerator();

		private Generator SimpleGenerator;
	}
}
