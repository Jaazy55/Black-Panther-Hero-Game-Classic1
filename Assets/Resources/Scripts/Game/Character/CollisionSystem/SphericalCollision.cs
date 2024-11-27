using System;
using Game.Character.CharacterController;
using Game.Character.Config;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public class SphericalCollision : ViewCollision
	{
		public SphericalCollision(Config.Config config) : base(config)
		{
			this.rayHitComparer = new RayHitComparer();
		}

		public override float Process(Vector3 cameraTarget, Vector3 dir, float distance)
		{
			float value = distance;
			float @float = this.config.GetFloat("MinDistance");
			float float2 = this.config.GetFloat("SphereCastTolerance");
			float float3 = this.config.GetFloat("SphereCastRadius");
			this.ray.origin = cameraTarget + dir * float3;
			this.ray.direction = -dir;
			Collider[] array = Physics.OverlapSphere(this.ray.origin, float3);
			bool flag = false;
			string @string = this.config.GetString("IgnoreCollisionTag");
			string string2 = this.config.GetString("TransparentCollisionTag");
			for (int i = 0; i < array.Length; i++)
			{
				if (ViewCollision.GetCollisionClass(array[i], @string, string2) == ViewCollision.CollisionClass.Collision)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				this.ray.origin = this.ray.origin + dir * float3;
				this.hits = Physics.RaycastAll(this.ray, distance - float3 + float2);
			}
			else
			{
				this.hits = Physics.SphereCastAll(this.ray, float3, distance + float3);
			}
			Array.Sort<RaycastHit>(this.hits, this.rayHitComparer);
			float num = float.PositiveInfinity;
			foreach (RaycastHit raycastHit in this.hits)
			{
				ViewCollision.CollisionClass collisionClass = ViewCollision.GetCollisionClass(raycastHit.collider, @string, string2);
				if (raycastHit.distance < num && collisionClass == ViewCollision.CollisionClass.Collision)
				{
					num = raycastHit.distance;
					value = raycastHit.distance - float2;
				}
				if (collisionClass == ViewCollision.CollisionClass.IgnoreTransparent)
				{
					base.UpdateTransparency(raycastHit.collider);
				}
			}
			return Mathf.Clamp(value, @float, distance);
		}

		private Ray ray;

		private RaycastHit[] hits;

		private readonly RayHitComparer rayHitComparer;
	}
}
