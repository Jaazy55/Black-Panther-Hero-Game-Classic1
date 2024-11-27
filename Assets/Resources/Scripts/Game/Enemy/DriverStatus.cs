using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.Enemy
{
	public class DriverStatus : HitEntity
	{
		public bool IsPlayer { get; protected set; }

		public bool InArmoredVehicle { get; protected set; }

		public void Init(GameObject originalDriverGO, bool Vulnerable)
		{
			base.Initialization(true);
			this.InArmoredVehicle = !Vulnerable;
			this.Dead = false;
			this.dummyDriver = originalDriverGO.GetComponent<DummyDriver>();
			if (this.dummyDriver == null)
			{
				this.playerScript = originalDriverGO.GetComponent<Player>();
				if (this.playerScript != null)
				{
					this.Health.Max = this.playerScript.Health.Max;
					this.Health.Current = this.playerScript.Health.Current;
					this.Defence = this.playerScript.Defence;
					this.Faction = this.playerScript.Faction;
					this.IsPlayer = true;
				}
			}
			else
			{
				BaseStatusNPC component = this.dummyDriver.DriverNPC.GetComponent<BaseStatusNPC>();
				this.Health.Max = component.Health.Max;
				this.Health.Current = component.Health.Current;
				this.Defence = component.Defence;
				this.Faction = component.Faction;
				this.IsPlayer = false;
			}
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			if (this.InArmoredVehicle && damageType != DamageType.Instant && (!this.IsPlayer || !this.playerScript.IsTransformer))
			{
				return;
			}
			if (owner != null && owner.Faction == Faction.Player)
			{
				FactionsManager.Instance.PlayerAttackHuman(this);
			}
			if (this.IsPlayer)
			{
				this.playerScript.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
			}
			else
			{
				base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
			}
		}

		protected override void OnDie()
		{
			base.OnDie();
			if (this.IsPlayer)
			{
				PlayerInteractionsManager.Instance.DieInCar();
			}
			else
			{
				this.dummyDriver.DriverDie();
			}
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		private GameObject driver;

		private DummyDriver dummyDriver;

		private Player playerScript;
	}
}
