using System;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Enemy
{
	public class NPCShootingAssistant : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
		{
			this.WeaponControllerInitialize(animator);
			if (this.weaponController != null)
			{
				animator.SetFloat("ShootSpeed", 1f / this.weaponController.CurrentWeapon.AttackDelay);
			}
			this.shooted = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (Time.timeScale == 0f)
			{
				return;
			}
			if (!this.shooted)
			{
				if (this.weaponController != null)
				{
					this.weaponController.AttackWithWeapon();
				}
				this.shooted = true;
			}
			if (stateInfo.normalizedTime > 0.9f)
			{
				this.shooted = false;
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

		private bool shooted;
	}
}
