using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class PlaySoundOnAnimation : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			PointSoundManager.Instance.PlayCustomClipAtPoint(animator.transform.position, this.sound, null);
		}

		public AudioClip sound;
	}
}
