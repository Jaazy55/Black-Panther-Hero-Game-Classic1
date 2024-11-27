using System;
using UnityEngine;

namespace Game.Tools
{
	public static class FastMath
	{
		public static Vector3 LineIntersectionRect(Vector3 start, Vector3 end, Rect rect)
		{
			Vector3 zero = Vector3.zero;
			if (FastMath.Intersects(start, end, new Vector3(rect.xMax, rect.yMin, 0f), new Vector3(rect.xMax, rect.yMax, 0f), out zero))
			{
				return zero;
			}
			if (FastMath.Intersects(start, end, new Vector3(rect.xMin, rect.yMin, 0f), new Vector3(rect.xMin, rect.yMax, 0f), out zero))
			{
				return zero;
			}
			if (FastMath.Intersects(start, end, new Vector3(rect.xMin, rect.yMax, 0f), new Vector3(rect.xMax, rect.yMax, 0f), out zero))
			{
				return zero;
			}
			if (FastMath.Intersects(start, end, new Vector3(rect.xMin, rect.yMin, 0f), new Vector3(rect.xMax, rect.yMin, 0f), out zero))
			{
				return zero;
			}
			return zero;
		}

		public static bool Intersects(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2, out Vector3 intersection)
		{
			intersection = Vector3.zero;
			Vector3 a3 = a2 - a1;
			Vector3 vector = b2 - b1;
			float num = a3.x * vector.y - a3.y * vector.x;
			if (num == 0f)
			{
				return false;
			}
			Vector3 vector2 = b1 - a1;
			float num2 = (vector2.x * vector.y - vector2.y * vector.x) / num;
			if (num2 < 0f || num2 > 1f)
			{
				return false;
			}
			float num3 = (vector2.x * a3.y - vector2.y * a3.x) / num;
			if (num3 < 0f || num3 > 1f)
			{
				return false;
			}
			intersection = a1 + num2 * a3;
			return true;
		}

		public static bool PointInRect(Vector3 viewportPos, Rect rect)
		{
			return viewportPos.x >= rect.xMin && viewportPos.x <= rect.xMax && viewportPos.y >= rect.yMin && viewportPos.y <= rect.yMax;
		}

		public static Vector3 SetVectorLength(Vector3 vector, float size)
		{
			Vector3 a = Vector3.Normalize(vector);
			return a * size;
		}
	}
}
