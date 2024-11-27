using System;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	public abstract class AttackModifier : ScriptableObject
	{
		public abstract float PreModify(Attack attack);

		public abstract float PostModify(Attack attack);
	}
}
