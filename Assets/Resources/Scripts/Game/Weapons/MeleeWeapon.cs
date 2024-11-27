using System;
using System.Collections;
using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using Game.Character.Extras;
using Game.Character.Stats;
using UnityEngine;

namespace Game.Weapons
{
	public class MeleeWeapon : Weapon
	{
		protected override void Start()
		{
			base.Start();
			this.Archetype = WeaponArchetype.Melee;
			this.player = (base.WeaponOwner as Player);
			this.InitUpgrades();
		}

		private void InitUpgrades()
		{
			this.animationsLength = new float[this.MeleeAnimations.Length];
			this.baseDelay = this.AttackDelay;
			for (int i = 0; i < this.MeleeAnimations.Length; i++)
			{
				MeleeWeapon.MeleeAnim meleeAnim = this.MeleeAnimations[i];
				meleeAnim.DamageTrigger.Init(this);
				meleeAnim.DamageTrigger.Damage = this.Damage;
				meleeAnim.DamageTrigger.SetAttackStatus(false);
				this.animationsLength[i] = meleeAnim.AnimationLength;
			}
			if (!this.player)
			{
				return;
			}
			this.meleeWeaponPlayerAttackSpeed = this.player.stats.GetPlayerStat(StatsList.MeleeWeaponAttackSpeed);
			this.UpdateDelay(this.meleeWeaponPlayerAttackSpeed);
		}

		public void UpdateStats(Player player)
		{
			float playerStat = player.stats.GetPlayerStat(StatsList.MeleeWeaponAttackSpeed);
			if (playerStat <= this.meleeWeaponPlayerAttackSpeed)
			{
				return;
			}
			this.UpdateDelay(playerStat);
			this.meleeWeaponPlayerAttackSpeed = playerStat;
		}

		private void UpdateDelay(float attackSpeed)
		{
			if (this.animationsLength == null)
			{
				return;
			}
			this.AttackDelay = this.baseDelay / attackSpeed;
			for (int i = 0; i < this.MeleeAnimations.Length; i++)
			{
				MeleeWeapon.MeleeAnim meleeAnim = this.MeleeAnimations[i];
				meleeAnim.AnimationLength = this.animationsLength[i] / attackSpeed;
			}
		}

		public MeleeAttackState GetMeleeAttackState()
		{
			if (base.IsOnCooldown)
			{
				return MeleeAttackState.Idle;
			}
			if (this.MeleeAnimations.Length > 0)
			{
				this.animationRandom = UnityEngine.Random.Range(0, this.MeleeAnimations.Length);
				return this.MeleeAnimations[this.animationRandom].AnimationState;
			}
			return MeleeAttackState.None;
		}

		private IEnumerator TriggerController(MeleeDamageTrigger trigger, float timer, bool status)
		{
			yield return new WaitForSeconds(timer);
			trigger.SetAttackStatus(status);
			yield break;
		}

		public override void Init()
		{
			base.Init();
			this.DamageTriggerActivator(true);
		}

		public override void DeInit()
		{
			base.DeInit();
			this.DamageTriggerActivator(false);
		}

		private void DamageTriggerActivator(bool activate)
		{
			foreach (MeleeWeapon.MeleeAnim meleeAnim in this.MeleeAnimations)
			{
				meleeAnim.DamageTrigger.SetAttackStatus(false);
				meleeAnim.DamageTrigger.gameObject.SetActive(activate);
				if (activate)
				{
					meleeAnim.DamageTrigger.Damage = this.Damage;
				}
			}
		}

		public void MeleeAttack(int attackState)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			foreach (MeleeWeapon.MeleeAnim meleeAnim in this.MeleeAnimations)
			{
				if (meleeAnim.AnimationState == (MeleeAttackState)attackState)
				{
					this.lastAttackTime = Time.time;
					base.StartCoroutine(this.TriggerController(meleeAnim.DamageTrigger, meleeAnim.AnimationLength / 5f, true));
					base.StartCoroutine(this.TriggerController(meleeAnim.DamageTrigger, meleeAnim.AnimationLength, false));
					if (this.PerformAttackEvent != null)
					{
						this.PerformAttackEvent(this);
					}
				}
			}
			this.HitAlarm();
		}

		private void HitAlarm()
		{
			if (!this.player)
			{
				return;
			}
			EntityManager.Instance.OverallAlarm(this.player, null, base.transform.position, 5f);
		}

		public override void Attack(HitEntity owner)
		{
		}

		public override void Attack(HitEntity owner, Vector3 direction)
		{
		}

		public override void Attack(HitEntity owner, HitEntity victim)
		{
		}

		public void SetDamage(float newDamage)
		{
			this.Damage = newDamage;
			for (int i = 0; i < this.MeleeAnimations.Length; i++)
			{
				this.MeleeAnimations[i].DamageTrigger.Damage = this.Damage;
			}
		}

		private const int AlarmRange = 5;

		[Separator("Melee weapon settings")]
		public MeleeWeapon.MeleeWeaponType MeleeType;

		public MeleeWeapon.MeleeAnim[] MeleeAnimations;

		private int animationRandom;

		private float fistPlayerAttackSpeed = 1f;

		private float meleeWeaponPlayerAttackSpeed = 1f;

		private float baseDelay;

		private float[] animationsLength;

		private Player player;

		public enum MeleeWeaponType
		{
			Hand,
			Knife,
			DoubleHand,
			BattleAxe
		}

		[Serializable]
		public class MeleeAnim
		{
			public MeleeAttackState AnimationState;

			public MeleeDamageTrigger DamageTrigger;

			public float AnimationLength;
		}
	}
}
