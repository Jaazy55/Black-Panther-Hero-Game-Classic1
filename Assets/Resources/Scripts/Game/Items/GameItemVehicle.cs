using System;
using Game.Character.Stats;
using Game.Vehicle;
using UnityEngine;

namespace Game.Items
{
	public class GameItemVehicle : GameItem
	{
		public static float maxSpeed { get; private set; }

		public static float maxAcceleration { get; private set; }

		public static float maxHealth { get; private set; }

		public override void Init()
		{
			base.Init();
			DrivableVehicle component = this.VehiclePrefab.GetComponent<DrivableVehicle>();
			VehicleStatus componentInChildren = this.VehiclePrefab.GetComponentInChildren<VehicleStatus>();
			if (component.MaxSpeed > GameItemVehicle.maxSpeed)
			{
				GameItemVehicle.maxSpeed = component.MaxSpeed;
			}
			if (component.Acceleration > GameItemVehicle.maxAcceleration)
			{
				GameItemVehicle.maxAcceleration = component.Acceleration;
			}
			if (componentInChildren.Health.Max > GameItemVehicle.maxHealth)
			{
				GameItemVehicle.maxHealth = componentInChildren.Health.Max;
			}
			for (int i = 0; i < this.StatAttributes.Length; i++)
			{
				StatsList statType = this.StatAttributes[i].StatType;
				if (statType != StatsList.DrivingMaxSpeed)
				{
					if (statType != StatsList.CarAcceleration)
					{
						if (statType == StatsList.CarHealth)
						{
							this.StatAttributes[i].SetStatValue(componentInChildren.Health.Max);
						}
					}
					else
					{
						this.StatAttributes[i].SetStatValue(component.Acceleration);
					}
				}
				else
				{
					this.StatAttributes[i].SetStatValue(component.MaxSpeed);
				}
			}
		}

		public override bool SameParametrWithOther(object[] parametrs)
		{
			return this.VehiclePrefab == (GameObject)parametrs[0];
		}

		public GameObject VehiclePrefab;

		public StatAttribute[] StatAttributes;

		public AdditionalFeature[] AdditionalFeatures;
	}
}
