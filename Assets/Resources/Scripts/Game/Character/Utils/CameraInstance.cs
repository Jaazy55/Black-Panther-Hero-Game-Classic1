using System;
using UnityEngine;

namespace Game.Character.Utils
{
	internal class CameraInstance
	{
		public static GameObject GetCameraRoot()
		{
			if (!CameraInstance.cameraRoot)
			{
				CameraInstance.cameraRoot = GameObject.Find(CameraInstance.RootName);
				if (!CameraInstance.cameraRoot)
				{
					CameraInstance.cameraRoot = new GameObject(CameraInstance.RootName);
				}
			}
			return CameraInstance.cameraRoot;
		}

		public static T CreateInstance<T>(string name) where T : Component
		{
			GameObject gameObject = CameraInstance.GetCameraRoot();
			T componentInChildren = gameObject.GetComponentInChildren<T>();
			if (componentInChildren)
			{
				return componentInChildren;
			}
			return new GameObject(name)
			{
				transform = 
				{
					parent = gameObject.transform
				}
			}.AddComponent<T>();
		}

		public static string RootName = "GameCamera";

		private static GameObject cameraRoot;
	}
}
