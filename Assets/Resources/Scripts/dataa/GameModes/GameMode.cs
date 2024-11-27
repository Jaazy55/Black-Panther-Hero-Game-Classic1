using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.GameModes
{
	[CreateAssetMenu(fileName = "Game Mode", menuName = "Game Modes/Game Mode")]
	public class GameMode : ScriptableObject
	{
		protected List<RuleInterpreter> Interpreters
		{
			get
			{
				if (this.m_Interpreters == null)
				{
					this.m_Interpreters = new List<RuleInterpreter>();
					foreach (Rule rule in this.m_rules)
					{
						RuleInterpreter[] ruleInterpreters = rule.RuleInterpreters;
						if (ruleInterpreters != null)
						{
							foreach (RuleInterpreter item in ruleInterpreters)
							{
								if (!this.m_Interpreters.Contains(item))
								{
									this.m_Interpreters.Add(item);
								}
							}
						}
					}
				}
				return this.m_Interpreters;
			}
		}

		public Rule[] Rules
		{
			get
			{
				return this.m_rules.ToArray();
			}
		}

		[SerializeField]
		private List<Rule> m_rules;

		private List<RuleInterpreter> m_Interpreters;
	}
}
