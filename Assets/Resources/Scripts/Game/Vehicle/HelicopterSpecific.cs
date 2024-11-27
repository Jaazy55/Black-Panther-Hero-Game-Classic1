using System;
using UnityEngine;

namespace Game.Vehicle
{
	public class HelicopterSpecific : VehicleSpecific
	{
		public const string OpenCabineTrigger = "Open";

		public const string EnterAnimatorBool = "EnterIn";

		public Animator CabinAnimator;

		public float GetOutAnimationTime;
	}
}
