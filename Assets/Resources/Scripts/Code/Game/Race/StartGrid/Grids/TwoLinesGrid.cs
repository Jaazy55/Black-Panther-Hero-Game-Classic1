using System;
using UnityEngine;

namespace Code.Game.Race.StartGrid.Grids
{
	public class TwoLinesGrid : StartGrid
	{
		public override Vector3[] CalculateGrid(int count)
		{
			Vector3[] array = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				if (i % 2 == 0)
				{
					array[i] = new Vector3(0f, 0f, (float)(-(float)i * 4));
				}
				else
				{
					array[i] = array[i - 1] + new Vector3(4f, 0f, 0f);
				}
			}
			return array;
		}
	}
}
