using System;
using System.Collections.Generic;

namespace Naxeex.AttaskSystem
{
	public class Attack
	{
		public Attack(float damage, IAttackReceiver receiver, IAttackSource source, params AttackModifier[] attackModifiers) : this(damage, null, false, receiver, source, attackModifiers)
		{
		}

		public Attack(float damage, ValidateFunction validateInvoke, IAttackReceiver receiver, IAttackSource source, params AttackModifier[] attackModifiers) : this(damage, validateInvoke, false, receiver, source, attackModifiers)
		{
		}

		public Attack(float damage, ValidateFunction validateInvoke, bool invoked, IAttackReceiver receiver, IAttackSource source, params AttackModifier[] attackModifiers)
		{
			this.m_Damage = damage;
			this.m_ModifierCollection = attackModifiers;
			this.m_ValidateInvokeFunction = validateInvoke;
			this.m_IsInvoked = invoked;
			this.m_Receiver = receiver;
			this.m_Source = source;
		}

		public Attack(Attack attack)
		{
			this.m_Damage = attack.Damage;
			IList<AttackModifier> modifiers = attack.Modifiers;
			this.m_ModifierCollection = new AttackModifier[modifiers.Count];
			for (int i = modifiers.Count - 1; i >= 0; i--)
			{
				this.m_ModifierCollection[i] = modifiers[i];
			}
			this.m_IsInvoked = false;
		}

		public float Damage
		{
			get
			{
				return this.m_Damage;
			}
		}

		public IList<AttackModifier> Modifiers
		{
			get
			{
				return this.m_ModifierCollection;
			}
		}

		public IAttackReceiver Receiver
		{
			get
			{
				return this.m_Receiver;
			}
		}

		public IAttackSource Source
		{
			get
			{
				return this.m_Source;
			}
		}

		public bool IsValidateInvoke
		{
			get
			{
				return this.m_ValidateInvokeFunction == null || this.m_ValidateInvokeFunction();
			}
		}

		public bool IsInvoked
		{
			get
			{
				return this.m_IsInvoked;
			}
		}

		public void Invoke()
		{
			if (this.m_IsInvoked)
			{
				return;
			}
			AttackResult result;
			if (this.IsValidateInvoke && this.m_Receiver != null)
			{
				result = this.m_Receiver.ProcessDamage(this);
			}
			else
			{
				result = AttackResult.Fail;
			}
			if (this.m_Source != null)
			{
				this.m_Source.ResultHandler(this, result);
			}
			this.m_IsInvoked = true;
		}

		private readonly float m_Damage;

		private readonly AttackModifier[] m_ModifierCollection;

		private readonly ValidateFunction m_ValidateInvokeFunction;

		private readonly IAttackReceiver m_Receiver;

		private readonly IAttackSource m_Source;

		private bool m_IsInvoked;
	}
}
