using System;
using System.Collections.Generic;
using UnityEngine;

public class DistanceComparer<T> : IComparer<T> where T : Component
{
	public int Compare(T a, T b)
	{
		return (this.playerPosition - a.transform.position).sqrMagnitude.CompareTo((this.playerPosition - b.transform.position).sqrMagnitude);
	}

	public Vector3 playerPosition;
}
