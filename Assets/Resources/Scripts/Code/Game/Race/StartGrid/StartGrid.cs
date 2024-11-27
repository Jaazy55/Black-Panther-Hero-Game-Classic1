using System;
using UnityEngine;

namespace Code.Game.Race.StartGrid
{
	public abstract class StartGrid : MonoBehaviour
	{
		public abstract Vector3[] CalculateGrid(int count);
	}
}
