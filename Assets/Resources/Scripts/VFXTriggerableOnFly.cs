using System;
using Game.Character.CharacterController;

public class VFXTriggerableOnFly : TriggerableVFX
{
	protected override void Start()
	{
		if (PlayerManager.Instance.DefaulPlayer.IsFlying)
		{
			base.StartVFX();
		}
		this.SetTriggerEvent();
	}

	public override void SetTriggerEvent()
	{
		AnimationController defaulAnimationController = PlayerManager.Instance.DefaulAnimationController;
		defaulAnimationController.StartSuperFlyEvent = (AnimationController.SuperFlyDelegate)Delegate.Combine(defaulAnimationController.StartSuperFlyEvent, new AnimationController.SuperFlyDelegate(base.StartVFX));
		AnimationController defaulAnimationController2 = PlayerManager.Instance.DefaulAnimationController;
		defaulAnimationController2.StopSuperFlyEvent = (AnimationController.SuperFlyDelegate)Delegate.Combine(defaulAnimationController2.StopSuperFlyEvent, new AnimationController.SuperFlyDelegate(base.StopVFX));
		Player defaulPlayer = PlayerManager.Instance.DefaulPlayer;
		defaulPlayer.PlayerDisableEvent = (Player.PlayerEnableDisableDelegate)Delegate.Combine(defaulPlayer.PlayerDisableEvent, new Player.PlayerEnableDisableDelegate(base.StopVFX));
		Player defaulPlayer2 = PlayerManager.Instance.DefaulPlayer;
		defaulPlayer2.PlayerEnableEvent = (Player.PlayerEnableDisableDelegate)Delegate.Combine(defaulPlayer2.PlayerEnableEvent, new Player.PlayerEnableDisableDelegate(base.StopVFX));
	}

	public override void UnsetTriggerEvent()
	{
		AnimationController defaulAnimationController = PlayerManager.Instance.DefaulAnimationController;
		defaulAnimationController.StartSuperFlyEvent = (AnimationController.SuperFlyDelegate)Delegate.Remove(defaulAnimationController.StartSuperFlyEvent, new AnimationController.SuperFlyDelegate(base.StartVFX));
		AnimationController defaulAnimationController2 = PlayerManager.Instance.DefaulAnimationController;
		defaulAnimationController2.StopSuperFlyEvent = (AnimationController.SuperFlyDelegate)Delegate.Remove(defaulAnimationController2.StopSuperFlyEvent, new AnimationController.SuperFlyDelegate(base.StopVFX));
		Player defaulPlayer = PlayerManager.Instance.DefaulPlayer;
		defaulPlayer.PlayerDisableEvent = (Player.PlayerEnableDisableDelegate)Delegate.Remove(defaulPlayer.PlayerDisableEvent, new Player.PlayerEnableDisableDelegate(base.StopVFX));
		Player defaulPlayer2 = PlayerManager.Instance.DefaulPlayer;
		defaulPlayer2.PlayerEnableEvent = (Player.PlayerEnableDisableDelegate)Delegate.Remove(defaulPlayer2.PlayerEnableEvent, new Player.PlayerEnableDisableDelegate(base.StopVFX));
	}
}
