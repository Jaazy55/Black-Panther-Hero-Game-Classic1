using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	public sealed class AttackManager : MonoBehaviour
	{
		private static AttackManager Instance
		{
			get
			{
				if (AttackManager.m_instance == null)
				{
					AttackManager.m_instance = new GameObject("AttackManager").AddComponent<AttackManager>();
					UnityEngine.Object.DontDestroyOnLoad(AttackManager.m_instance.gameObject);
				}
				return AttackManager.m_instance;
			}
		}

		public static Attack Create(IAttackReceiver receiver, IAttackSource source)
		{
			List<AttackModifier> list = new List<AttackModifier>(source.Modifiers);
			Attack attack = new Attack(source.Damage, receiver, source, list.ToArray());
			AttackManager.Instance.AllAttackStack.Add(attack);
			AttackManager.Instance.NeedInvokeAttackStack.Add(attack);
			return attack;
		}

		public static Attack Create(IAttackReceiver receiver, IAttackSource source, float Time)
		{
			List<AttackModifier> list = new List<AttackModifier>(source.Modifiers);
			Attack attack = new Attack(source.Damage, receiver, source, list.ToArray());
			AttackManager.Instance.AllAttackStack.Add(attack, Time);
			AttackManager.Instance.NeedInvokeAttackStack.Add(attack, Time);
			return attack;
		}

		private void Update()
		{
			IEnumerable<Attack> attacksBefore = this.NeedInvokeAttackStack.GetAttacksBefore(Time.time);
			foreach (Attack attack in attacksBefore)
			{
				attack.Invoke();
				this.NeedInvokeAttackStack.Remove(attack);
			}
			this.Timer -= Time.deltaTime;
			if (this.Timer < 0f)
			{
				this.AllAttackStack.Clear();
				this.Timer = this.AutoClearTime;
			}
		}

		private static AttackManager m_instance;

		[SerializeField]
		private float AutoClearTime = 100f;

		private float Timer;

		private AttackStack AllAttackStack = new AttackStack();

		private AttackStack NeedInvokeAttackStack = new AttackStack();
	}
}
