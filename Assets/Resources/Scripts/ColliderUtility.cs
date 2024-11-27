using System;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderUtility
{
	public static ColliderUtility.BorderInfo GetBorderInfo(Collider collider, Vector3 point)
	{
		Vector3[] array = new Vector3[8];
		float[] array2 = new float[8];
		Vector3 center = collider.bounds.center;
		Vector3 extents = collider.bounds.extents;
		int num = 0;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					array[num] = extents[0] * (float)((i != 0) ? -1 : 1) * Vector3.right + extents[1] * (float)((j != 0) ? -1 : 1) * Vector3.up + extents[2] * (float)((k != 0) ? -1 : 1) * Vector3.forward;
					array[num] = collider.transform.position + collider.transform.rotation * array[num];
					num++;
				}
			}
		}
		Vector3 vector = center - point;
		vector.y = 0f;
		vector.Normalize();
		Vector3 vector2 = Vector3.zero;
		Vector3 vertex = default(Vector3);
		Vector3 vertex2 = default(Vector3);
		float num2 = 0f;
		float num3 = 0f;
		for (int l = 0; l < 8; l++)
		{
			array[l].y = 0f;
			vector2 = (array[l] - point).normalized;
			vector2.y = 0f;
			array2[l] = Vector3.Angle(vector2, vector) * Mathf.Sign(Vector3.Cross(vector2, vector).y);
			if (array2[l] > 0f)
			{
				if (array2[l] >= num3)
				{
					num3 = array2[l];
					vertex2 = array[l];
				}
			}
			else if (array2[l] <= num2)
			{
				num2 = array2[l];
				vertex = array[l];
			}
		}
		ColliderUtility.SideInfo sideInfo = new ColliderUtility.SideInfo
		{
			Vertex = vertex,
			BaseCollider = collider
		};
		ColliderUtility.SideInfo sideInfo2 = new ColliderUtility.SideInfo
		{
			Vertex = vertex2,
			BaseCollider = collider
		};
		vector2 = sideInfo.Vertex - point;
		vector2.y = 0f;
		vector2.Normalize();
		sideInfo.AngleNorm = Vector3.Angle(vector2, Vector3.right) * Mathf.Sign(Vector3.Cross(vector2, Vector3.right).y);
		vector2 = sideInfo2.Vertex - point;
		vector2.y = 0f;
		vector2.Normalize();
		sideInfo2.AngleNorm = Vector3.Angle(vector2, Vector3.right) * Mathf.Sign(Vector3.Cross(vector2, Vector3.right).y);
		return new ColliderUtility.BorderInfo(sideInfo, sideInfo2);
	}

	public static IList<ColliderUtility.BorderInfo> GetBorderInfo(IList<Collider> colliders, Vector3 point)
	{
		IList<ColliderUtility.BorderInfo> list = new List<ColliderUtility.BorderInfo>();
		foreach (Collider collider in colliders)
		{
			list.Add(ColliderUtility.GetBorderInfo(collider, point));
		}
		ColliderUtility.BorderInfoMeneger.Instance.SortBorderInfos(ref list);
		return list;
	}

	public static IList<ColliderUtility.BorderInfo> Inverse(this IList<ColliderUtility.BorderInfo> borderInfos)
	{
		List<ColliderUtility.BorderInfo> list = new List<ColliderUtility.BorderInfo>();
		List<ColliderUtility.BorderInfo> list2 = new List<ColliderUtility.BorderInfo>();
		if (borderInfos.Count > 0)
		{
			ColliderUtility.BorderInfo item = new ColliderUtility.BorderInfo(borderInfos[0].Right, borderInfos[0].Left);
			list.Add(item);
		}
		if (list.Count > 0)
		{
			foreach (ColliderUtility.BorderInfo subtrahend in borderInfos)
			{
				foreach (ColliderUtility.BorderInfo minuend in list)
				{
					list2.AddRange(ColliderUtility.BorderInfoMeneger.Instance.Subtract(minuend, subtrahend));
				}
				list.Clear();
				list.AddRange(list2);
			}
		}
		return list;
	}

	public static float GetMaxColliderRadius(Component component)
	{
		Collider[] componentsInChildren = component.GetComponentsInChildren<Collider>();
		float num = 0f;
		foreach (Collider collider in componentsInChildren)
		{
			if (!collider.isTrigger)
			{
				Vector3 extents = collider.bounds.extents;
				extents.y = 0f;
				num = Mathf.Max(num, extents.magnitude);
			}
		}
		return num;
	}

	public static bool Contains(this IList<ColliderUtility.BorderInfo> borderInfos, float angle)
	{
		bool result = false;
		foreach (ColliderUtility.BorderInfo borderInfo in borderInfos)
		{
			if (borderInfo.SectorContains(angle))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public class SideInfo
	{
		public SideInfo()
		{
		}

		public SideInfo(Vector3 vertex, Collider collider, float angleNorm)
		{
			this.BaseCollider = collider;
			this.Vertex = vertex;
			this.AngleNorm = angleNorm;
		}

		public Collider BaseCollider;

		public Vector3 Vertex;

		public float AngleNorm;
	}

	public class BorderInfo
	{
		public BorderInfo(ColliderUtility.SideInfo left, ColliderUtility.SideInfo right)
		{
			this.Left = left;
			this.Right = right;
		}

		public bool SectorContains(float comparedAngle)
		{
			comparedAngle %= 180f;
			if (this.Left.AngleNorm < this.Right.AngleNorm)
			{
				return this.Left.AngleNorm < comparedAngle && comparedAngle < this.Right.AngleNorm;
			}
			return this.Left.AngleNorm < comparedAngle || comparedAngle < this.Right.AngleNorm;
		}

		public ColliderUtility.SideInfo Left;

		public ColliderUtility.SideInfo Right;
	}

	private class BorderInfoMeneger
	{
		private BorderInfoMeneger()
		{
		}

		public static ColliderUtility.BorderInfoMeneger Instance
		{
			get
			{
				if (ColliderUtility.BorderInfoMeneger._instance == null)
				{
					ColliderUtility.BorderInfoMeneger._instance = new ColliderUtility.BorderInfoMeneger();
				}
				return ColliderUtility.BorderInfoMeneger._instance;
			}
		}

		public void AddBorderInfo(ColliderUtility.BorderInfo addedBorder, ref IList<ColliderUtility.BorderInfo> borders)
		{
			bool flag = true;
			while (flag)
			{
				flag = false;
				for (int i = 0; i < borders.Count; i++)
				{
					if (borders[i] != addedBorder)
					{
						bool flag2 = borders[i].SectorContains(addedBorder.Left.AngleNorm);
						bool flag3 = borders[i].SectorContains(addedBorder.Right.AngleNorm);
						if (!flag2 || !flag3)
						{
							if (flag2)
							{
								borders[i].Right = addedBorder.Right;
								flag = true;
							}
							else if (flag3)
							{
								borders[i].Left = addedBorder.Left;
								flag = true;
							}
							else if (addedBorder.SectorContains(borders[i].Right.AngleNorm))
							{
								borders[i].Right = addedBorder.Right;
								borders[i].Left = addedBorder.Left;
								flag = true;
							}
						}
						if (flag)
						{
							ColliderUtility.BorderInfo borderInfo = borders[i];
							if (borders.Contains(addedBorder))
							{
								borders.Remove(addedBorder);
							}
							addedBorder = borderInfo;
							break;
						}
					}
				}
				if (!flag && !borders.Contains(addedBorder))
				{
					borders.Add(addedBorder);
				}
			}
		}

		public IList<ColliderUtility.BorderInfo> Subtract(ColliderUtility.BorderInfo minuend, ColliderUtility.BorderInfo subtrahend)
		{
			IList<ColliderUtility.BorderInfo> list = new List<ColliderUtility.BorderInfo>();
			bool flag = minuend.SectorContains(subtrahend.Left.AngleNorm);
			bool flag2 = minuend.SectorContains(subtrahend.Right.AngleNorm);
			if (flag || flag2)
			{
				if (!flag)
				{
					minuend.Left = subtrahend.Right;
					list.Add(minuend);
				}
				else if (!flag2)
				{
					minuend.Right = subtrahend.Left;
					list.Add(minuend);
				}
				else
				{
					ColliderUtility.BorderInfo item = new ColliderUtility.BorderInfo(minuend.Left, subtrahend.Left);
					ColliderUtility.BorderInfo item2 = new ColliderUtility.BorderInfo(subtrahend.Right, minuend.Right);
					list.Add(item);
					list.Add(item2);
				}
			}
			else if (!subtrahend.SectorContains(minuend.Right.AngleNorm))
			{
				list.Add(minuend);
			}
			return list;
		}

		public void SortBorderInfos(ref IList<ColliderUtility.BorderInfo> borders)
		{
			IList<ColliderUtility.BorderInfo> list = new List<ColliderUtility.BorderInfo>();
			foreach (ColliderUtility.BorderInfo addedBorder in borders)
			{
				ColliderUtility.BorderInfoMeneger.Instance.AddBorderInfo(addedBorder, ref list);
			}
			borders = list;
		}

		private bool SectorContains(float leftAngle, float rightAngle, float comparedAngle)
		{
			comparedAngle %= 180f;
			leftAngle %= 180f;
			rightAngle %= 180f;
			if (leftAngle < rightAngle)
			{
				return leftAngle < comparedAngle && comparedAngle < rightAngle;
			}
			return leftAngle < comparedAngle || comparedAngle < rightAngle;
		}

		private static ColliderUtility.BorderInfoMeneger _instance;
	}
}
