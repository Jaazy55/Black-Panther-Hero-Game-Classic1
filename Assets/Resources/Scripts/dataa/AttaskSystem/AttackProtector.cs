using System;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	public abstract class AttackProtector : ScriptableObject
	{
		public abstract float PreProtection(Attack attack);

		public abstract float PostProtection(Attack attack);
	}
}
