using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	[CreateAssetMenu(fileName = "WeaponAnimatedAttackData", menuName = "Attack System/Weapon Animated Attack Data", order = 15)]
	public class WeaponAnimatedAttackData : AnimatedAttackData
	{
		public override void EnterConditions(Animator animator)
		{
			base.EnterConditions(animator);
		}

		public override void ExitCondition(Animator animator)
		{
			base.ExitCondition(animator);
		}

		[SerializeField]
		private GameObject m_WeaponPrefab;

		private Dictionary<Animator, GameObject> CreatedWeapon = new Dictionary<Animator, GameObject>();
	}
}
