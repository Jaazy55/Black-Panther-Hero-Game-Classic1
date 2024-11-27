using System;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class ObstacleHelper
	{
		public static Obstacle FindObstacle(Vector3 pos, Vector3 dir, float maxDistance, float maxHeight, LayerMask layerMask)
		{
			Ray ray = new Ray(pos + Vector3.up * 0.5f, dir);
			RaycastHit[] array = Physics.RaycastAll(ray, maxDistance, layerMask);
			UnityEngine.Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.yellow, 1f);
			float num = float.PositiveInfinity;
			Vector3 a = Vector3.zero;
			Vector3 wallNormal = Vector3.zero;
			bool flag = false;
			foreach (RaycastHit raycastHit in array)
			{
				if (raycastHit.distance < num && !raycastHit.collider.isTrigger && Vector3.Dot(raycastHit.normal, Vector3.up) < 0.2f)
				{
					num = raycastHit.distance;
					a = raycastHit.point;
					wallNormal = raycastHit.normal;
					flag = true;
				}
			}
			if (flag)
			{
				float distance = num;
				ray.origin = a + dir * 0.25f + Vector3.up * maxHeight;
				ray.direction = Vector3.up * -1f;
				array = Physics.RaycastAll(ray, maxHeight);
				UnityEngine.Debug.DrawRay(ray.origin, ray.direction * maxHeight, Color.yellow, 1f);
				num = float.PositiveInfinity;
				Vector3 vector = Vector3.zero;
				bool flag2 = false;
				foreach (RaycastHit raycastHit2 in array)
				{
					if (raycastHit2.distance < num && !raycastHit2.collider.isTrigger)
					{
						num = raycastHit2.distance;
						vector = raycastHit2.point;
						flag2 = true;
					}
				}
				if (flag2)
				{
					return new Obstacle
					{
						Distance = distance,
						Height = num,
						WallPoint = vector,
						WallNormal = wallNormal,
						Type = ObstacleHelper.GetType(pos, vector)
					};
				}
			}
			return new Obstacle
			{
				Type = ObstacleType.None
			};
		}

		private static ObstacleType GetType(Vector3 ground, Vector3 wall)
		{
			float num = wall.y - ground.y;
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(wall, Vector3.up, out raycastHit);
			if (flag && Vector3.Distance(raycastHit.point, wall) < 2f)
			{
				return ObstacleType.None;
			}
			if (num < 1f)
			{
				return ObstacleType.ObstacleLow;
			}
			if (num < 1.5f)
			{
				return ObstacleType.ObstacleMedium;
			}
			if (num < 3f)
			{
				return ObstacleType.ObstacleHigh;
			}
			return ObstacleType.None;
		}

		private const float HeightObstacleLow = 1f;

		private const float HeightObstacleMid = 1.5f;

		private const float HeightObstacleHigh = 3f;

		private const float HeightCharacter = 2f;
	}
}
