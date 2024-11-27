using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.SpawnSystem
{
	[CreateAssetMenu(fileName = "GenerableData", menuName = "NPC/Create Generable Data", order = 1)]
	public class GenerableData : ScriptableObject
	{
		public IList<GenerableBlock> Blocks
		{
			get
			{
				return this.m_Blocks.Clone() as IList<GenerableBlock>;
			}
		}

		[SerializeField]
		private GenerableBlock[] m_Blocks;
	}
}
