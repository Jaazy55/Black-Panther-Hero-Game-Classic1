using System;
using System.Collections.Generic;

namespace Naxeex.AttaskSystem
{
	public interface IAttackSource
	{
		bool CanAttack(IAttackReceiver receiver);

		float Damage { get; }

		IEnumerable<AttackModifier> Modifiers { get; }

		void ResultHandler(Attack attack, AttackResult result);
	}
}
