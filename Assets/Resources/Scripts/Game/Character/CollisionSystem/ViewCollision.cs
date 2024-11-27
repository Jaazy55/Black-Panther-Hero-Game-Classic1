using System;
using Game.Character.Config;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public abstract class ViewCollision
	{
		protected ViewCollision(Config.Config config)
		{
			this.config = config;
		}

		public abstract float Process(Vector3 cameraTarget, Vector3 cameraDir, float distance);

		public static ViewCollision.CollisionClass GetCollisionClass(Collider collider, string ignoreTag, string transparentTag)
		{
			ViewCollision.CollisionClass result = ViewCollision.CollisionClass.Collision;
			if (collider.isTrigger)
			{
				result = ViewCollision.CollisionClass.Trigger;
			}
			else
			{
				IgnoreCollision component = collider.gameObject.GetComponent<IgnoreCollision>();
				if ((component && component.IsWorkingForCurrentCamera) || collider.gameObject.CompareTag(ignoreTag))
				{
					result = ViewCollision.CollisionClass.Ignore;
				}
				else if (collider.gameObject.GetComponent<TransparentCollision>() || collider.gameObject.CompareTag(transparentTag))
				{
					result = ViewCollision.CollisionClass.IgnoreTransparent;
				}
			}
			return result;
		}

		protected void UpdateTransparency(Collider collider)
		{
			TransparencyManager.Instance.UpdateObject(collider.gameObject);
		}

		protected readonly Config.Config config;

		public enum CollisionClass
		{
			Collision,
			Trigger,
			Ignore,
			IgnoreTransparent
		}
	}
}
