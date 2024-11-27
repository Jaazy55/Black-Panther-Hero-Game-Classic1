using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class RayHitComparer : IComparer<RaycastHit>
	{
		public int Compare(RaycastHit x, RaycastHit y)
		{
			return x.distance.CompareTo(y.distance);
		}
	}
}
