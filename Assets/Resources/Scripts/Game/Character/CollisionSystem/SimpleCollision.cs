using System;
using Game.Character.CharacterController;
using Game.Character.Config;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public class SimpleCollision : ViewCollision
	{
		public SimpleCollision(Config.Config config) : base(config)
		{
			this.rayHitComparer = new RayHitComparer();
		}

		public override float Process(Vector3 cameraTarget, Vector3 dir, float distance)
		{
			float value = distance;
			float @float = this.config.GetFloat("RaycastTolerance");
			float float2 = this.config.GetFloat("MinDistance");
			float num = float.PositiveInfinity;
			this.ray.origin = cameraTarget;
			this.ray.direction = -dir;
			this.hits = Physics.RaycastAll(this.ray, distance + @float);
			Array.Sort<RaycastHit>(this.hits, this.rayHitComparer);
			string @string = this.config.GetString("IgnoreCollisionTag");
			string string2 = this.config.GetString("TransparentCollisionTag");
			foreach (RaycastHit raycastHit in this.hits)
			{
				ViewCollision.CollisionClass collisionClass = ViewCollision.GetCollisionClass(raycastHit.collider, @string, string2);
				if (raycastHit.distance < num && collisionClass == ViewCollision.CollisionClass.Collision)
				{
					num = raycastHit.distance;
					value = raycastHit.distance - @float;
				}
				if (collisionClass == ViewCollision.CollisionClass.IgnoreTransparent)
				{
					base.UpdateTransparency(raycastHit.collider);
				}
			}
			return Mathf.Clamp(value, float2, distance);
		}

		private Ray ray;

		private RaycastHit[] hits;

		private readonly RayHitComparer rayHitComparer;
	}
}
