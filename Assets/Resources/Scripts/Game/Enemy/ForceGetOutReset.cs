using System;
using UnityEngine;

namespace Game.Enemy
{
	public class ForceGetOutReset : StateMachineBehaviour
	{
		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			base.OnStateMachineEnter(animator, stateMachinePathHash);
			this.reseted = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime > 0.9f && !this.reseted)
			{
				animator.SetBool("ForceGet", false);
				this.reseted = true;
			}
		}

		private bool reseted;
	}
}
