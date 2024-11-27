using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.GameModes
{
	[CreateAssetMenu(fileName = "Rule", menuName = "Game Modes/Rule")]
	public class Rule : ScriptableObject
	{
		public RuleInterpreter[] RuleInterpreters
		{
			get
			{
				return this.m_RuleInterpreters.ToArray();
			}
		}

		[SerializeField]
		private List<RuleInterpreter> m_RuleInterpreters;
	}
}
