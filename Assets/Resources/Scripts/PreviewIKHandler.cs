using System;
using UnityEngine;

public class PreviewIKHandler : MonoBehaviour
{
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
	}

	public void TurnThisShitOff()
	{
		this.DoIKAnimation = false;
	}

	public void TurnThisShitOn()
	{
		this.DoIKAnimation = true;
	}

	private void OnAnimatorIK()
	{
		if (this.DoIKAnimation)
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
}
