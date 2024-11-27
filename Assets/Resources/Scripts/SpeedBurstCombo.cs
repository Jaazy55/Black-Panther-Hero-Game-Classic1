using System;
using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

public class SpeedBurstCombo : ComboManager.BaseComboEffect
{
	protected override void Start()
	{
		base.Start();
		this.animationController = PlayerInteractionsManager.Instance.Player.GetComponent<AnimationController>();
		this.originalSpeedMults = this.animationController.SpeedMults;
		this.originalAirSpeed = this.animationController.AirSpeed;
		this.originalClimbeSpeed = this.animationController.ClimbingSpeed;
	}

	public override void AddStackEffect()
	{
		this.animationController.SpeedMults = new Vector2(this.originalSpeedMults.x + (float)this.currStacks * this.originalSpeedMults.x * this.SpeedPercentPerStack / 100f, this.originalSpeedMults.y + (float)this.currStacks * this.originalSpeedMults.y * this.SpeedPercentPerStack / 100f);
		this.animationController.AirSpeed = this.originalAirSpeed + (float)this.currStacks * this.originalAirSpeed * this.SpeedPercentPerStack / 100f;
		this.animationController.ClimbingSpeed = this.originalClimbeSpeed + (float)this.currStacks * this.originalClimbeSpeed * this.SpeedPercentPerStack / 100f;
	}

	public override void ClearStacksEffects()
	{
		this.animationController.SpeedMults = this.originalSpeedMults;
		this.animationController.AirSpeed = this.originalAirSpeed;
		this.animationController.ClimbingSpeed = this.originalClimbeSpeed;
	}

	[Separator("SpeedBurstCombo")]
	public float SpeedPercentPerStack = 10f;

	private AnimationController animationController;

	private Vector2 originalSpeedMults;

	private float originalAirSpeed;

	private float originalClimbeSpeed;
}
