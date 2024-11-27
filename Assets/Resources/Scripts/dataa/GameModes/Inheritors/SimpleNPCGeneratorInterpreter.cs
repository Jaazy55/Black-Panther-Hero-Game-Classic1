using System;
using Naxeex.SpawnSystem;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Simple NPC Generator Interpeter", menuName = "Game Modes/Interpeters/NPC Generator/Simple NPC Generator Interpeter")]
	public class SimpleNPCGeneratorInterpreter : NPCGeneratorInterpreter
	{
		protected override Generator Generator
		{
			get
			{
				if (this.m_Generator == null)
				{
					this.m_Generator = new DataGenerator(this.m_GenerableData);
				}
				return this.m_Generator;
			}
		}

		[SerializeField]
		private GenerableData m_GenerableData;

		private Generator m_Generator;
	}
}
