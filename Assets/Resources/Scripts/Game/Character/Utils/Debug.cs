using System;
using System.Diagnostics;
using UnityEngine;

namespace Game.Character.Utils
{
	public static class Debug
	{
		[Conditional("UNITY_EDITOR")]
		public static void Assert(bool condition, string message = "")
		{
			if (!condition)
			{
				UnityEngine.Debug.LogError("Assert! " + message);
				UnityEngine.Debug.Break();
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void Log(string format, params object[] args)
		{
			UnityEngine.Debug.Log(string.Format(format, args));
		}

		[Conditional("UNITY_EDITOR")]
		public static void Log(object arg)
		{
			UnityEngine.Debug.Log(arg.ToString());
		}

		public static Vector3 GetCentroid(GameObject obj)
		{
			MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>();
			Vector3 a = Vector3.zero;
			if (componentsInChildren != null && componentsInChildren.Length != 0)
			{
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					a += meshRenderer.bounds.center;
				}
				return a / (float)componentsInChildren.Length;
			}
			SkinnedMeshRenderer componentInChildren = obj.GetComponentInChildren<SkinnedMeshRenderer>();
			if (componentInChildren)
			{
				return componentInChildren.bounds.center;
			}
			return obj.transform.position;
		}

		public static void SetVisible(GameObject obj, bool status, bool includeInactive)
		{
			if (obj)
			{
				MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>(includeInactive);
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					meshRenderer.enabled = status;
				}
				SkinnedMeshRenderer[] componentsInChildren2 = obj.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
				{
					skinnedMeshRenderer.enabled = status;
				}
			}
		}

		public static void ClearLog()
		{
		}

		public static bool IsActive(GameObject obj)
		{
			return obj && obj.activeSelf;
		}

		public static void SetActive(GameObject obj, bool status)
		{
			if (obj)
			{
				obj.SetActive(status);
			}
		}

		public static void SetActiveRecursively(GameObject obj, bool status)
		{
			if (obj)
			{
				int childCount = obj.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Game.Character.Utils.Debug.SetActiveRecursively(obj.transform.GetChild(i).gameObject, status);
				}
				obj.SetActive(status);
			}
		}

		public static void EnableCollider(GameObject obj, bool status)
		{
			if (obj)
			{
				Collider[] componentsInChildren = obj.GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					collider.enabled = status;
				}
			}
		}

		public static void Destroy(UnityEngine.Object obj, bool allowDestroyingAssets)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(obj);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(obj, allowDestroyingAssets);
			}
		}
	}
}
