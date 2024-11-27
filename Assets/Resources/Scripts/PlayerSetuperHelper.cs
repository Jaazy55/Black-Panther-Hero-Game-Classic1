using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Modes;
using Game.GlobalComponent;
using Game.Weapons;
using UnityEngine;

public class PlayerSetuperHelper : MonoBehaviour
{
	[Separator("Main references")]
	public Player PlayerScript;

	public WeaponController WeaponController;

	public AnimationController AnimationController;

	public PlayerInitializer Initializer;

	[Separator("Custom camera")]
	public Game.Character.Modes.Type CameraModeType;

	public string CollisionConfigName = "Default";

	[Separator("Custom buttons")]
	public bool Rope;

	public bool SuperFly;

	public bool SuperKick;

	[Separator("Transformation Types")]
	public ControlsType[] TransformationControlsPanels;
}
