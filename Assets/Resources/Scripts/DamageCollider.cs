using System;
using Game.Character.CharacterController;
using UnityEngine;

public class DamageCollider : HitEntity
{
	public void OnCollisionStay(Collision collision)
	{
		Rigidbody rigidbody = collision.rigidbody;
		if (rigidbody == null)
		{
			return;
		}
		HitEntity component = rigidbody.GetComponent<HitEntity>();
		if (component == null)
		{
			return;
		}
		if (component.Faction == Faction.Player)
		{
			component.OnHit(DamageType.Collision, this, this.m_PlayerDamage, collision.contacts[0].point, collision.contacts[0].normal, 0f);
		}
		else
		{
			component.OnHit(DamageType.Collision, this, this.m_OtherDamage, collision.contacts[0].point, collision.contacts[0].normal, 0f);
		}
	}

	[SerializeField]
	private float m_PlayerDamage;

	[SerializeField]
	private float m_OtherDamage;
}
