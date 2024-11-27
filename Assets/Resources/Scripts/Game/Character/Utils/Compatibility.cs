using System;
using UnityEngine;

namespace Game.Character.Utils
{
	public static class Compatibility
	{
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
					Compatibility.SetActiveRecursively(obj.transform.GetChild(i).gameObject, status);
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

		public static void SetCursorVisible(bool status)
		{
			Cursor.visible = status;
		}

		public static void LockCursor(bool status)
		{
			Cursor.lockState = ((!status) ? CursorLockMode.None : CursorLockMode.None);
		}

		public static bool IsCursorLocked()
		{
			return Cursor.lockState == CursorLockMode.None;
		}
	}
}
