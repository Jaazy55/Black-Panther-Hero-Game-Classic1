using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character
{
	public class PlayerSurfaceSensor : SurfaceSensor
	{
		protected override void Awake()
		{
			base.Awake();
			this.underwaterEffect = UnderwaterEffect.Instance;
		}

		public override void Init()
		{
			base.Init();
			if (this.player)
			{
				PlayerDieManager.Instance.PlayerResurrectEvent = new PlayerDieManager.PlayerResurrect(this.OnResurrect);
			}
		}

		private void OnResurrect(float resurrectionTime)
		{
			base.CheckWaterTriggers();
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.player && PlayerInteractionsManager.Instance.inVehicle)
			{
				this.player.CheckDrowning();
			}
			if (this.underwaterEffect != null && (base.AboveWater || base.InWater))
			{
				this.underwaterEffect.SetDepth(base.CurrWaterSurfaceHeight);
			}
		}

		[Space(10f)]
		public Player player;

		private UnderwaterEffect underwaterEffect;
	}
}
