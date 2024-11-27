using System;
using Game.Character.CharacterController;
using Game.Weapons;
using UnityEngine;

namespace Game.Character
{
	public class PlayerInitializer : MonoBehaviour
	{
		public void Initialaize()
		{
			if (!this.PlayerAm)
			{
				this.PlayerAm = base.GetComponent<AnimationController>();
			}
			if (!this.PlayerWc)
			{
				this.PlayerWc = base.GetComponent<WeaponController>();
			}
			if (!this.PlayerStatus)
			{
				this.PlayerStatus = base.GetComponent<Player>();
			}
			if (!this.WaterSensor)
			{
				this.WaterSensor = base.GetComponentInChildren<SurfaceSensor>();
			}
			this.PlayerAm.Initialization();
			this.PlayerWc.Initialization();
			this.PlayerStatus.Initialization(true);
			this.WaterSensor.Init();
		}

		public AnimationController PlayerAm;

		public WeaponController PlayerWc;

		public Player PlayerStatus;

		public SurfaceSensor WaterSensor;
	}
}
