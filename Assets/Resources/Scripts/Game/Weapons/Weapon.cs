using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Shop;
using UnityEngine;

namespace Game.Weapons
{
	public abstract class Weapon : MonoBehaviour
	{
		public HitEntity WeaponOwner { get; protected set; }

		public bool IsOnCooldown
		{
			get
			{
				return Time.time - this.lastAttackTime < this.AttackDelay;
			}
		}

		protected virtual void Start()
		{
			if (this.AttackDistance <= 0f)
			{
				if (this.Archetype == WeaponArchetype.Melee)
				{
					this.AttackDistance = 1f;
				}
				if (this.Archetype == WeaponArchetype.Ranged || this.Archetype == WeaponArchetype.Grenade)
				{
					this.AttackDistance = CameraManager.Instance.UnityCamera.farClipPlane;
				}
			}
			if (this.Damage <= 0f)
			{
				this.Damage = 1f;
			}
			if (this.AttackDelay <= 0f)
			{
				this.AttackDelay = 1f;
			}
		}

		public virtual void Init()
		{
			this.WeaponOwner = base.GetComponentInParent<HitEntity>();
			BodyPartDamageReciever bodyPartDamageReciever = this.WeaponOwner as BodyPartDamageReciever;
			if (bodyPartDamageReciever != null)
			{
				this.WeaponOwner = bodyPartDamageReciever.RerouteEntity;
			}
		}

		public virtual void DeInit()
		{
		}

		public void SetWeaponOwner(HitEntity newOwner)
		{
			this.WeaponOwner = newOwner;
		}

		public abstract void Attack(HitEntity owner);

		public abstract void Attack(HitEntity owner, Vector3 direction);

		public abstract void Attack(HitEntity owner, HitEntity victim);

		protected void PlaySound(AudioSource audioSource, AudioClip audioClip)
		{
			if (audioClip == null || audioSource == null)
			{
				return;
			}
			audioSource.PlayOneShot(audioClip);
		}

		public void PlayAttackSound(AudioSource audioSource)
		{
			this.PlaySound(audioSource, this.SoundAttack);
		}

		public void PlayHitSound(Vector3 hitPosition)
		{
			PointSoundManager.Instance.Play3DSoundOnPoint(hitPosition, this.SoundHit);
		}

		public bool DebugLog;

		public WeaponArchetype Archetype;

		public WeaponTypes Type;

		public AmmoTypes AmmoType;

		public WeaponNameList WeaponNameInList;

		public string Name;

		public float AttackDistance;

		public float Damage;

		public DamageType WeaponDamageType = DamageType.Bullet;

		public float DefenceIgnorence;

		public float AttackDelay;

		public AudioClip SoundAttack;

		public AudioClip SoundHit;

		public Sprite image;

		public bool Unique;

		public GameObject[] WeaponObjects;

		public Vector2 AimOffset;

		[SerializeField]
		protected float lastAttackTime;

		public Weapon.AttackEvent PerformAttackEvent;

		public Weapon.DamageEvent InflictDamageEvent;

		public delegate void AttackEvent(Weapon weapon);

		public delegate void DamageEvent(Weapon weapon, HitEntity owner, HitEntity victim, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f);
	}
}
