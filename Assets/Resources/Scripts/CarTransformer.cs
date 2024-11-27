using System;
using Game.Enemy;
using Game.Vehicle;
using UnityEngine;

public class CarTransformer : MonoBehaviour
{
	private void Start()
	{
		VehicleStatus componentInChildren = base.GetComponentInChildren<VehicleStatus>();
		HumanoidStatusNPC component = this.NPCRobotPrefab.GetComponent<HumanoidStatusNPC>();
		componentInChildren.Health = component.Health;
		componentInChildren.Defence.Set(component.Defence);
		componentInChildren.ExperienceForAKill = component.ExperienceForAKill;
	}

	public GameObject NPCRobotPrefab;
}
