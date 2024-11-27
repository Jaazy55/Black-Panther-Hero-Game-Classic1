using System;
using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Zombie Posion Interpeter", menuName = "Game Modes/Interpeters/Zombie Posion Interpeter")]
	public class ZombiePosionInterpreter : RuleInterpreter
	{
		protected Player Player
		{
			get
			{
				if (this.m_Player == null && PlayerInteractionsManager.HasInstance)
				{
					this.m_Player = PlayerInteractionsManager.Instance.Player;
				}
				return this.m_Player;
			}
		}

		public override void RuleBegin()
		{
			base.RuleBegin();
			ZombiePosionTimer.ActivateValue = this.MaxTime;
			ZombiePosionTimer.Value = 0f;
			this.LastDamageTime = Time.time;
			if (this.ZombieGenerator != null)
			{
				this.ZombieGenerator.OnEntityGenerate += this.GenerateHandler;
				this.ZombieGenerator.OnEntityDegenerate += this.DegenerateHandler;
			}
		}

		public override void RuleProcess()
		{
			base.RuleProcess();
			if (Time.time - this.LastDamageTime > this.IncreaseTime)
			{
				ZombiePosionTimer.Value += Time.deltaTime;
			}
			if (ZombiePosionTimer.Activate && this.Player != null && !this.Player.IsDead)
			{
				this.Player.Health.Change(-this.PercentDamagePerSecond * this.Player.Health.Max * Time.deltaTime);
			}
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
			ZombiePosionTimer.Value = 0f;
			this.LastDamageTime = Time.time;
			if (this.ZombieGenerator != null)
			{
				this.ZombieGenerator.OnEntityGenerate -= this.GenerateHandler;
				this.ZombieGenerator.OnEntityDegenerate -= this.DegenerateHandler;
			}
		}

		public override void RuleRestart()
		{
			base.RuleRestart();
			ZombiePosionTimer.Value = 0f;
		}

		private void GenerateHandler(HitEntity entity)
		{
			if (entity != null)
			{
				entity.DiedEvent += this.DieHandler;
				entity.DamageEvent += this.DamageHandler;
			}
		}

		private void DegenerateHandler(HitEntity entity)
		{
			if (entity != null)
			{
				entity.DiedEvent -= this.DieHandler;
				entity.DamageEvent -= this.DamageHandler;
			}
		}

		private void DieHandler()
		{
			ZombiePosionTimer.Value -= this.DecreaseTime;
			this.LastDamageTime = Time.time;
		}

		private void DamageHandler(float Damage, HitEntity owner)
		{
			if (owner != null && owner.Faction != Faction.Player)
			{
				return;
			}
			ZombiePosionTimer.Value -= this.DecreaseTime;
			this.LastDamageTime = Time.time;
		}

		[SerializeField]
		private float IncreaseTime;

		[SerializeField]
		private float DecreaseTime;

		[SerializeField]
		private float MaxTime;

		[SerializeField]
		[Range(0f, 1f)]
		private float PercentDamagePerSecond;

		[SerializeField]
		private NPCGeneratorInterpreter ZombieGenerator;

		private Player m_Player;

		private float LastDamageTime;
	}
}
