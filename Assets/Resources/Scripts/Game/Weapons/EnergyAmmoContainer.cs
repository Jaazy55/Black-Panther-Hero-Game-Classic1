using System;
using Game.Character;
using Game.Character.Stats;

namespace Game.Weapons
{
	public class EnergyAmmoContainer : JointAmmoContainer
	{
		private void Awake()
		{
			if (this.IsPlayer)
			{
				this.stamina = PlayerInteractionsManager.Instance.Player.stats.stamina;
			}
		}

		public override int GetAmmoCount()
		{
			if (this.IsPlayer)
			{
				return (int)this.stamina.Current;
			}
			return base.GetAmmoCount();
		}

		public bool IsPlayer;

		private CharacterStat stamina;
	}
}
