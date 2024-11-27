using System;
using Game.Weapons;
using UnityEngine;

namespace Game.Character
{
	public class ShootingAssistant : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
		{
			if (this.weaponController == null)
			{
				this.WeaponControllerInitialize(animator);
			}
			if (this.weaponController != null)
			{
				animator.SetFloat("ShootSpeed", 1f / this.weaponController.CurrentWeapon.AttackDelay);
			}
			this.shooted = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
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
			this.weaponController = animator.GetComponent<WeaponController>();
			if (!this.weaponController)
			{
				UnityEngine.Debug.LogError("Can't find WeaponController");
			}
		}

		private WeaponController weaponController;

		private bool shooted;
	}
}
