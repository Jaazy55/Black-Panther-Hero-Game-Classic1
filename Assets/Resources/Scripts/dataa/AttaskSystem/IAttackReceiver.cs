using System;
using System.Collections.Generic;

namespace Naxeex.AttaskSystem
{
	public interface IAttackReceiver
	{
		bool CanBeAttaked(IAttackSource source);

		IEnumerable<AttackProtector> Protectors { get; }

		AttackResult ProcessDamage(Attack attack);
	}
}
