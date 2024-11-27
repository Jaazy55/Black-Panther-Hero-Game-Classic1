using System;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Enemy
{
	public class NPCMeleeHitAssistant : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
		{
			this.WeaponControllerInitialize(animator);
			this.meleeAttackState = animator.GetInteger("Melee");
			this.hitted = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (!this.hitted)
			{
				this.weaponController.MeleeWeaponAttack(this.meleeAttackState);
				this.hitted = true;
			}
		}

		private void WeaponControllerInitialize(Animator animator)
		{
			if (this.weaponController == null)
			{
				BaseNPC component = animator.GetComponent<BaseNPC>();
				SmartHumanoidController smartHumanoidController = component.CurrentController as SmartHumanoidController;
				if (smartHumanoidController != null)
				{
					this.weaponController = smartHumanoidController.WeaponController;
					PoolManager.Instance.AddBeforeReturnEvent(smartHumanoidController, delegate(GameObject poolingObject)
					{
						this.weaponController = null;
					});
				}
			}
		}

		private SmartHumanoidWeaponController weaponController;

		private int meleeAttackState;

		private bool hitted;
	}
}
