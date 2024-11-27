using System;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class MotorcycleSpecific : CarSpecific, IInitable
	{
		private const float IKDriverUpdateTime = 3f;

		public GameObject DriverModel;

		public GameObject Helm;

		public float HelmAngle = 40f;

		public GameObject FrontWheelPoint;

		public float MaxLeanAngle = 35f;

		public MyCustomIK HandsIKController;
	}
}
