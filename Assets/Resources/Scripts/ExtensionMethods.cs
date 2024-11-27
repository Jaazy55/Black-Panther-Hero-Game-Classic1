using System;
using UnityEngine;

public static class ExtensionMethods
{
	public static T MyGetComponent<T>(this GameObject g) where T : Component
	{
		if (g == null)
		{
			return (T)((object)null);
		}
		T component = g.GetComponent<T>();
		if (component == null)
		{
			return (T)((object)null);
		}
		return component;
	}

	public static void DestroyChildrens(this Transform t)
	{
		bool isPlaying = Application.isPlaying;
		while (t.childCount != 0)
		{
			Transform child = t.GetChild(0);
			if (isPlaying)
			{
				child.SetParent(null);
				UnityEngine.Object.Destroy(child.gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(child.gameObject);
			}
		}
	}

	public static bool LayerInMask(this LayerMask mask, int layer)
	{
		return (mask.value & 1 << layer) == 1 << layer;
	}
}
