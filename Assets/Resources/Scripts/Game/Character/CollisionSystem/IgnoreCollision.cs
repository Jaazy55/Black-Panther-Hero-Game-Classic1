using System;
using Game.Character.Modes;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public class IgnoreCollision : MonoBehaviour
	{
		public bool IsWorkingForCurrentCamera
		{
			get
			{
				if (this.NeededCameraMods == null || this.NeededCameraMods.Length == 0)
				{
					return true;
				}
				Game.Character.Modes.Type type = CameraManager.Instance.GetCurrentCameraMode().Type;
				for (int i = 0; i < this.NeededCameraMods.Length; i++)
				{
					Game.Character.Modes.Type type2 = this.NeededCameraMods[i];
					if (type2 == type)
					{
						return true;
					}
				}
				return false;
			}
		}

		[Header("If empty, works for all mods")]
		public Game.Character.Modes.Type[] NeededCameraMods;
	}
}
