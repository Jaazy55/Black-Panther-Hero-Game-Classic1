using System;
using Game.Managers;
using UnityEngine;

namespace Game.Weapons
{
	public class MeleeHitAssistant : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			this.previousInteger = 0;
			this.meleeAttackState = animator.GetInteger("Melee");
			if (this.weaponController == null)
			{
				this.WeaponControllerInitialize(animator);
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (this.previousInteger == 0)
			{
				this.previousInteger++;
				if (this.weaponController == null)
				{
					this.WeaponControllerInitialize(animator);
				}
				if (this.weaponController != null)
				{
					this.weaponController.MeleeWeaponAttack(this.meleeAttackState);
				}
			}
		}

		private void WeaponControllerInitialize(Animator animator)
		{
			this.weaponController = animator.GetComponent<WeaponController>();
			if (!this.weaponController && GameManager.ShowDebugs)
			{
				UnityEngine.Debug.LogError("Can't find WeaponController");
			}
		}

		private WeaponController weaponController;

		private int previousInteger;

		private int meleeAttackState;
	}
}
