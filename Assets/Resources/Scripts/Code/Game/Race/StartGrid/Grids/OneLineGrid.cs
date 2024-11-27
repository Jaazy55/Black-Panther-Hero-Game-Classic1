using System;
using UnityEngine;

namespace Code.Game.Race.StartGrid.Grids
{
	public class OneLineGrid : StartGrid
	{
		public override Vector3[] CalculateGrid(int count)
		{
			Vector3[] array = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = new Vector3(0f, 0f, (float)(-(float)i * 7));
			}
			return array;
		}
	}
}
