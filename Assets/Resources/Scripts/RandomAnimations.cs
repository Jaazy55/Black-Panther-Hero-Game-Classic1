using System;
using Game.Character.CharacterController;
using UnityEngine;

public class RandomAnimations : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
//		Debug.Log("Name is:" + this.name);
		
//		float num = this.animationController.TargetDistance / this.animationController.Velocity.magnitude;
		//UnityEngine.Debug.Log("Time to wall - " + num);
		//UnityEngine.Debug.Log("Animation Time - " + stateInfo.length);
		//if (num > stateInfo.length * 3f)
		//{
	//		animator.SetInteger("IdleIndex", UnityEngine.Random.Range(0, this.count));
	//	}
	//	else
	//	{
			animator.SetInteger("IdleIndex", -1);
	//	}
	}

	private void Init(Animator animator)
	{
		if (this.animationController == null)
		{
			this.animationController = animator.GetComponent<AnimationController>();
		}
	}

	public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
		this.Init(animator);
	}

	[SerializeField]
	private int count;

	private AnimationController animationController;
}
