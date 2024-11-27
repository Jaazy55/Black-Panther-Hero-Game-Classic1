using System;
using Game.Character;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class CarSpecific : VehicleSpecific, IInitable
	{
		public virtual void Init()
		{
			if (this.BodyRenderers.Length > 0)
			{
				PlayerInteractionsManager.Instance.LastDrivableVehicle.ChangeBodyColor(this.BodyRenderers);
			}
		}

		public virtual void DeInit()
		{
			GameObject gameObject = PoolManager.Instance.PrefabOf(base.gameObject);
			if (gameObject != null)
			{
				CarSpecific component = gameObject.GetComponent<CarSpecific>();
				if (component != null && this.CarAnimator != null)
				{
					Utils.CopyTransforms(component.CarAnimator.transform, this.CarAnimator.transform);
				}
			}
		}

		public const string LeftDoorOpenTrigger = "LeftOpen";

		public const string RightDoorOpenTrigger = "RightOpen";

		public const string EnterInCarAnimatorBool = "EnterInCar";

		[Separator("Setup for Controller")]
		public Animator CarAnimator;

		public float GetOutAnimationTime = 3f;

		public float PitchOffset;

		public GameObject[] Taillights;

		public AudioClip GearShiftSound;

		public AudioClip[] EngineSounds;

		public AudioClip BrakeSound;

		public Renderer[] BodyRenderers;
	}
}
