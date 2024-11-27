using System;
using Game.GlobalComponent;
using UnityEngine;

public class PlaySoundTypeOnAnimation : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		PointSoundManager.Instance.PlaySoundAtPoint(animator.transform.position, this.soundType);
	}

	public TypeOfSound soundType;
}
