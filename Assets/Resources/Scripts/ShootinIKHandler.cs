using System;
using Game.Character.CharacterController;
using UnityEngine;

public class ShootinIKHandler : MonoBehaviour
{
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.animationController = base.GetComponent<AnimationController>();
	}

	private void OnAnimatorIK()
	{
		if (this.DoIKAnimation && this.animationController.IsAming())
		{
			this.anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, this.ikWeightHand);
			this.anim.SetIKPosition(AvatarIKGoal.LeftHand, this.leftIKHandTarget.position);
			this.anim.SetIKRotation(AvatarIKGoal.LeftHand, this.leftIKHandTarget.rotation);
			this.anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, this.ikWeightElbow);
			this.anim.SetIKHintPosition(AvatarIKHint.LeftElbow, this.leftIKHandHint.position);
		}
	}

	private Animator anim;

	public float ikWeightHand = 1f;

	public float ikWeightElbow = 1f;

	public bool DoIKAnimation;

	public Transform leftIKHandTarget;

	public Transform leftIKHandHint;

	private AnimationController animationController;
}
