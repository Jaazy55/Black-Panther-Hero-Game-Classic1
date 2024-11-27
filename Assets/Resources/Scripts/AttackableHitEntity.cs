using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Naxeex.AttaskSystem;
using UnityEngine;

public class AttackableHitEntity : MonoBehaviour, IAttackReceiver
{
	public IEnumerable<AttackProtector> Protectors
	{
		get
		{
			return this.m_Protectors.Clone() as AttackProtector[];
		}
	}

	public bool CanBeAttaked(IAttackSource source)
	{
		return !this.m_HitEntity.IsDead;
	}

	public AttackResult ProcessDamage(Attack attack)
	{
		if (!this.m_HitEntity.IsDead)
		{
			try
			{
				HitEntity owner = null;
				if (attack.Source is Component)
				{
					owner = (attack.Source as Component).GetComponent<HitEntity>();
				}
				this.m_HitEntity.OnHit(DamageType.MeleeHit, owner, attack.Damage, (!(this.m_Rigidbody != null)) ? Vector3.zero : this.m_Rigidbody.worldCenterOfMass, Vector3.zero, 0f);
			}
			catch
			{
				return AttackResult.Fail;
			}
			return (!this.m_HitEntity.IsDead) ? AttackResult.Damage : AttackResult.Death;
		}
		return AttackResult.Miss;
	}

	private void Awake()
	{
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
	}

	[SerializeField]
	private HitEntity m_HitEntity;

	[SerializeField]
	private AttackProtector[] m_Protectors;

	private Rigidbody m_Rigidbody;
}
