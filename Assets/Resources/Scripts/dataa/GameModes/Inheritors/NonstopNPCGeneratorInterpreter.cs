using System;
using Game.Character.CharacterController;
using Naxeex.SpawnSystem;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Nonstop NPC Generator Interpeter", menuName = "Game Modes/Interpeters/NPC Generator/Nonstop NPC Generator Interpeter")]
	public class NonstopNPCGeneratorInterpreter : NPCGeneratorInterpreter
	{
		protected override Generator Generator
		{
			get
			{
				return (this.Entities.Count >= this.MaxCount || this.CurrentGenerator == null) ? this.EmptyGenerator : this.CurrentGenerator;
			}
		}

		protected override void OnGenerate(HitEntity entity)
		{
			base.OnGenerate(entity);
			if (this.CurrentGenerator != null && !this.CurrentGenerator.HasCurrent)
			{
				this.Index++;
				this.UpdateGenerator();
			}
		}

		public override void RuleBegin()
		{
			base.RuleBegin();
			this.Index = 0;
			this.UpdateGenerator();
		}

		public override void RuleRestart()
		{
			base.RuleRestart();
			this.Index = 0;
			this.UpdateGenerator();
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
			this.CurrentGenerator = null;
			this.Index = -1;
		}

		private void UpdateGenerator()
		{
			int num = this.Index;
			int i;
			for (i = 0; i < this.dataArrays.Length; i++)
			{
				num -= this.dataArrays[i].CycleCount * this.dataArrays[i].Database.Length;
				if (num < 0 || this.dataArrays[i].CycleCount <= 0)
				{
					break;
				}
			}
			if (i < 0 || i >= this.dataArrays.Length)
			{
				this.CurrentGenerator = null;
				return;
			}
			int num2 = num % this.dataArrays[i].Database.Length;
			if (num2 < 0)
			{
				num2 += this.dataArrays[i].Database.Length;
			}
			this.CurrentGenerator = new DataGenerator(this.dataArrays[i].Database[num2]);
		}

		[SerializeField]
		private int MaxCount = 10;

		[SerializeField]
		private CycleGenerableDataArray[] dataArrays;

		private Generator CurrentGenerator;

		private Generator EmptyGenerator = new EmptyGenerator();

		private int Index = -1;
	}
}
