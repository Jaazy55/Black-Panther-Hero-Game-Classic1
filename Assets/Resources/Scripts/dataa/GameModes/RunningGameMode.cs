using System;
using System.Collections.Generic;

namespace Naxeex.GameModes
{
	public class RunningGameMode
	{
		public RunningGameMode(GameMode gameMode)
		{
			this.m_interpreters = new List<RuleInterpreter>();
			Rule[] rules = gameMode.Rules;
			foreach (Rule rule in rules)
			{
				if (!(rule == null))
				{
					RuleInterpreter[] ruleInterpreters = rule.RuleInterpreters;
					if (ruleInterpreters != null)
					{
						foreach (RuleInterpreter ruleInterpreter in ruleInterpreters)
						{
							if (!(ruleInterpreter == null))
							{
								if (!this.m_interpreters.Contains(ruleInterpreter))
								{
									this.m_interpreters.Add(ruleInterpreter);
								}
							}
						}
					}
				}
			}
		}

		private readonly List<RuleInterpreter> m_interpreters;
	}
}
