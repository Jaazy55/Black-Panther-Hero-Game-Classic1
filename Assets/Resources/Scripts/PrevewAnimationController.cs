using System;
using UnityEngine;

public class PrevewAnimationController : MonoBehaviour
{
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animationStateHash = Animator.StringToHash("AnimationType");
		this.currenAnimType = (PrevewAnimationController.PreviewAnimType)this.animator.GetInteger(this.animationStateHash);
	}

	public void SetPreviewAnimation(PrevewAnimationController.PreviewAnimType animaType)
	{
		if (animaType != this.currenAnimType)
		{
			this.animator.SetInteger(this.animationStateHash, (int)animaType);
			this.currenAnimType = animaType;
		}
	}

	private const string AnimationStateName = "AnimationType";

	private Animator animator;

	private PrevewAnimationController.PreviewAnimType currenAnimType;

	private int animationStateHash;

	public PrevewAnimationController.PreviewAnimType animationToPlay = PrevewAnimationController.PreviewAnimType.Rifle;

	public enum PreviewAnimType
	{
		None = -1,
		Idle,
		Rifle,
		Pistol,
		Melee,
		Shotgun,
		Minigun,
		RPG,
		Braslet,
		Body,
		Hands,
		Legs,
		Knife,
		BaseBoll,
		ShowMeThatShit,
		AXE,
		IronFly,
		ShootRope,
		SuperLanding,
		ClimbWalls,
		SuperKick
	}
}
