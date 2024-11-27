using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Character.Config;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public class TargetCollision
	{
		public TargetCollision(Config.Config config)
		{
			this.rayHitComparer = new RayHitComparer();
			this.config = config;
		}

		public float CalculateTarget(Vector3 targetHead, Vector3 cameraTarget)
		{
			string @string = this.config.GetString("IgnoreCollisionTag");
			string string2 = this.config.GetString("TransparentCollisionTag");
			float @float = this.config.GetFloat("TargetSphereRadius");
			float num = 1f;
			Vector3 normalized = (cameraTarget - targetHead).normalized;
			Ray ray = new Ray(targetHead, normalized);
			this.hits.Clear();
			int num2 = Physics.RaycastNonAlloc(ray, this.currentHits, normalized.magnitude + @float);
			for (int i = 0; i < num2; i++)
			{
				this.hits.Add(this.currentHits[i]);
			}
			this.hits.Sort(this.rayHitComparer);
			float num3 = float.PositiveInfinity;
			bool flag = false;
			for (int j = 0; j < this.hits.Count; j++)
			{
				RaycastHit raycastHit = this.hits[j];
				ViewCollision.CollisionClass collisionClass = ViewCollision.GetCollisionClass(raycastHit.collider, @string, string2);
				if (raycastHit.distance < num3 && collisionClass == ViewCollision.CollisionClass.Collision)
				{
					num3 = raycastHit.distance;
					num = raycastHit.distance - @float;
					flag = true;
				}
			}
			if (flag)
			{
				return Mathf.Clamp01(num / (targetHead - cameraTarget).magnitude);
			}
			return 1f;
		}

		private const string Ignorecollisiontag = "IgnoreCollisionTag";

		private const string Transparentcollisiontag = "TransparentCollisionTag";

		private const string Targetsphereradius = "TargetSphereRadius";

		private readonly List<RaycastHit> hits = new List<RaycastHit>();

		private readonly RayHitComparer rayHitComparer;

		private readonly Config.Config config;

		private readonly RaycastHit[] currentHits = new RaycastHit[10];
	}
}
