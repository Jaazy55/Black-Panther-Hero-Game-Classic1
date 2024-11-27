using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class Utils
	{
		public static void CopyTransforms(Transform ethalon, Transform subject)
		{
			subject.localPosition = ethalon.localPosition;
			subject.localRotation = ethalon.localRotation;
			subject.localScale = ethalon.localScale;
			for (int i = 0; i < ethalon.childCount; i++)
			{
				Transform child = ethalon.GetChild(i);
				Transform transform = subject.GetChild(i);
				if (transform != null && child.name.Equals(transform.name))
				{
					Utils.CopyTransforms(child, transform);
				}
				else
				{
					transform = subject.Find(child.name);
					if (transform != null)
					{
						Utils.CopyTransforms(child, transform);
					}
				}
			}
		}

		public const float MsecToKmh = 3.6f;

		public const float KmhToMsec = 0.2777778f;
	}
}
