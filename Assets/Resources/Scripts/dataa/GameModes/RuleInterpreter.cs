using System;
using UnityEngine;

namespace Naxeex.GameModes
{
	public abstract class RuleInterpreter : ScriptableObject
	{
		public virtual void RuleBegin()
		{
		}

		public virtual void RuleProcess()
		{
		}

		public virtual void RuleEnd()
		{
		}

		public virtual void RuleRestart()
		{
		}
	}
}
