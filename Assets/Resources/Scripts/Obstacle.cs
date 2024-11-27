using System;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public struct Obstacle
	{
		public ObstacleType Type;

		public Vector3 WallPoint;

		public Vector3 WallNormal;

		public float Height;

		public float Distance;
	}
}
