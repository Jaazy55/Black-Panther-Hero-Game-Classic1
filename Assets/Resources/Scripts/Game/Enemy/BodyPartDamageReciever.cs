using System;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Enemy
{
	public class BodyPartDamageReciever : HitEntity
	{
		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			float num = (damageType != DamageType.MeleeHit) ? this.DamageCounter : 1f;
			this.RerouteEntity.OnHit(damageType, owner, damage * num, hitPos, hitVector, defenceReduction);
		}

		[Separator("Reciever Specific Parametrs")]
		public HitEntity RerouteEntity;

		public float DamageCounter;
	}
}
