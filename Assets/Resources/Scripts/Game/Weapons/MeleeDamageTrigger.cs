using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.Managers;
using UnityEngine;

namespace Game.Weapons
{
	public class MeleeDamageTrigger : MonoBehaviour
	{
		public void Init(MeleeWeapon weapon)
		{
			if (this.DamageTrigger == null)
			{
				this.DamageTrigger = base.GetComponent<Collider>();
			}
			this.owner = base.GetComponentInParent<HitEntity>();
			this.parentWeapon = weapon;
		}

		public void SetAttackStatus(bool status)
		{
			if (this.DamageTrigger == null)
			{
				this.DamageTrigger = base.GetComponent<Collider>();
			}
			this.DamageTrigger.enabled = status;
			if (status)
			{
				this.hitedObjectsList.Clear();
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			HitEntity hitEntity = col.GetComponent<HitEntity>();

			//Debug.Log("Collision Enter HitEntity = " + hitEntity.name);

			BodyPartDamageReciever bodyPartDamageReciever = hitEntity as BodyPartDamageReciever;
			RagdollDamageReciever ragdollDamageReciever = hitEntity as RagdollDamageReciever;
			if (bodyPartDamageReciever != null)
			{
				hitEntity = bodyPartDamageReciever.RerouteEntity;
			}
			if (ragdollDamageReciever != null)
			{
				hitEntity = ragdollDamageReciever.RdStatus;
			}
			if (hitEntity != null && hitEntity != this.owner) //&& !this.hitedObjectsList.Contains(hitEntity)   //Waseem
			{
				Debug.Log("It Enters hitEntity");
				if (this.DebugLog)// && GameManager.ShowDebugs //Waseem
				{
					UnityEngine.Debug.Log("Boom");
				}

				if (FactionsManager.Instance.GetRelations(this.owner.Faction, hitEntity.Faction) == Relations.Friendly)
				{
					return;
				}
				Vector3 vector = hitEntity.transform.position + hitEntity.NPCShootVectorOffset - base.transform.position;
				if (this.parentWeapon.InflictDamageEvent != null)
				{
					this.parentWeapon.InflictDamageEvent(this.parentWeapon, this.owner, hitEntity, base.transform.position, vector.normalized, 0f);
				}
				this.parentWeapon.PlayHitSound(base.transform.position);
				this.hitedObjectsList.Add(hitEntity);
				//clearAll();
			}
		}

		private void clearAll()
		{
			//DamageTrigger = null;
			//Damage = 0;
			owner = null;
			parentWeapon = null;
			for (int i = 0; i < hitedObjectsList.Count; i++)
			{
				hitedObjectsList[i] = null;
			}

		}

		public bool DebugLog;

		public Collider DamageTrigger;

		public float Damage;

		private HitEntity owner;

		private MeleeWeapon parentWeapon;

		private readonly List<HitEntity> hitedObjectsList = new List<HitEntity>();
	}
}
