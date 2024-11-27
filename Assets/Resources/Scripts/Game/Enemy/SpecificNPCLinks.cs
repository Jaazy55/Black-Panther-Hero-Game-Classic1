using System;
using UnityEngine;

namespace Game.Enemy
{
	public class SpecificNPCLinks : MonoBehaviour
	{
		public GameObject Hips;

		public GameObject Head;

		public GameObject LeftHand;

		public GameObject RightHand;

		public Collider[] ModelColliders;

		public WeaponForSmartHumanoidNPC UsingWeapons;

		public ActionType SmartActionType;

		public float SmartSprintTime = 2f;
	}
}
