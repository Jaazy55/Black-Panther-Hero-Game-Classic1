using System;
using Game.Character.CollisionSystem;
using UnityEngine;

namespace Game.Character.Input
{
	public abstract class GameInput : MonoBehaviour
	{
		public abstract InputPreset PresetType { get; }

		public bool ResetInputArray { get; protected set; }

		protected virtual void Awake()
		{
			this.mouseFilter = new InputFilter(10, 0.5f);
			this.padFilter = new InputFilter(10, 0.6f);
			this.ResetInputArray = true;
		}

		public abstract void UpdateInput(Input[] inputs);

		protected void SetInput(Input[] inputs, InputType type, object value)
		{
			if (inputs[(int)type].Enabled)
			{
				inputs[(int)type].Value = value;
				inputs[(int)type].Valid = true;
			}
		}

		public static bool FindWaypointPosition(Vector2 mousePos, out Vector3 pos)
		{
			Camera unityCamera = CameraManager.Instance.UnityCamera;
			Ray ray = unityCamera.ScreenPointToRay(mousePos);
			RaycastHit[] array = Physics.RaycastAll(ray, float.MaxValue);
			if (array.Length == 0)
			{
				pos = Vector3.zero;
				return false;
			}
			Array.Sort<RaycastHit>(array, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
			float num = float.MaxValue;
			Vector3 vector = Vector3.zero;
			foreach (RaycastHit raycastHit in array)
			{
				Collider collider = raycastHit.collider;
				ViewCollision.CollisionClass collisionClass = CameraCollision.Instance.GetCollisionClass(collider);
				if (raycastHit.distance < num && collisionClass == ViewCollision.CollisionClass.Collision)
				{
					num = raycastHit.distance;
					vector = raycastHit.point;
				}
			}
			pos = vector;
			return true;
		}

		protected InputFilter mouseFilter;

		protected InputFilter padFilter;

		protected float doubleClickTimeout;
	}
}
