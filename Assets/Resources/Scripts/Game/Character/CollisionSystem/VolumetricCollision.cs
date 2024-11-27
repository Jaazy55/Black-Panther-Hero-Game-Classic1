using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Character.Config;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public class VolumetricCollision : ViewCollision
	{
		public VolumetricCollision(Config.Config config) : base(config)
		{
			this.rayHitComparer = new RayHitComparer();
			this.hits = new List<RaycastHit>(40);
			this.rays = new Ray[10];
			for (int i = 0; i < this.rays.Length; i++)
			{
				this.rays[i] = new Ray(Vector3.zero, Vector3.zero);
			}
		}

		public override float Process(Vector3 cameraTarget, Vector3 dir, float distance)
		{
			float value = distance;
			float @float = this.config.GetFloat("RaycastTolerance");
			float float2 = this.config.GetFloat("MinDistance");
			Vector2 vector = this.config.GetVector2("ConeRadius");
			float float3 = this.config.GetFloat("ConeSegments");
			Vector3 a = cameraTarget - dir * distance;
			Vector3 a2 = Vector3.Cross(dir, Vector3.up);
			Vector3 vector2 = Vector3.zero;
			int num = 0;
			while ((float)num < float3)
			{
				float angle = (float)num / float3 * 360f;
				Quaternion rotation = Quaternion.AngleAxis(angle, dir);
				Vector3 vector3 = cameraTarget + rotation * (a2 * vector.x);
				Vector3 a3 = a + rotation * (a2 * vector.y);
				vector2 = a3 - vector3;
				this.rays[num].origin = vector3;
				switch (num)
				{
				case 0:
					this.rays[num].direction = a3 - vector3 + Vector3.left;
					break;
				case 1:
					this.rays[num].direction = a3 - vector3 + Vector3.forward;
					break;
				case 2:
					this.rays[num].direction = a3 - vector3 + Vector3.right;
					break;
				case 3:
					this.rays[num].direction = a3 - vector3 + Vector3.back;
					break;
				}
				num++;
			}
			float magnitude = vector2.magnitude;
			this.hits.Clear();
			for (int i = 0; i < this.rays.Length; i++)
			{
				Ray ray = this.rays[i];
				int num2 = Physics.RaycastNonAlloc(ray, this.currentHits, magnitude + @float);
				for (int j = 0; j < num2; j++)
				{
					this.hits.Add(this.currentHits[j]);
				}
			}
			this.hits.Sort(this.rayHitComparer);
			float num3 = float.PositiveInfinity;
			string @string = this.config.GetString("IgnoreCollisionTag");
			string string2 = this.config.GetString("TransparentCollisionTag");
			for (int k = 0; k < this.hits.Count; k++)
			{
				RaycastHit raycastHit = this.hits[k];
				ViewCollision.CollisionClass collisionClass = ViewCollision.GetCollisionClass(raycastHit.collider, @string, string2);
				if (raycastHit.distance < num3 && collisionClass == ViewCollision.CollisionClass.Collision)
				{
					num3 = raycastHit.distance;
					value = raycastHit.distance - @float + -0.5f;
				}
				if (collisionClass == ViewCollision.CollisionClass.IgnoreTransparent)
				{
					base.UpdateTransparency(raycastHit.collider);
				}
			}
			return Mathf.Clamp(value, float2, distance);
		}

		private const float offset = -0.5f;

		private readonly List<RaycastHit> hits;

		private readonly Ray[] rays;

		private readonly RayHitComparer rayHitComparer;

		private const int maxRays = 10;

		private readonly RaycastHit[] currentHits = new RaycastHit[10];
	}
}
