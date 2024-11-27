using System;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class CastResult
	{
		public GameObject TargetObject;

		public Vector3 HitPosition;

		public TargetType TargetType;

		public HitEntity HitEntity;

		public Vector3 HitVector;

		public float RayLength;
	}
}
