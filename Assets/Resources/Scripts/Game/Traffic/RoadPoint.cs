using System;
using UnityEngine;

namespace Game.Traffic
{
	public class RoadPoint
	{
		public float GetSpaceLineWidthToNode(RoadPoint toNode)
		{
			float result = 0f;
			foreach (RoadLink roadLink in this.RoadLinks)
			{
				if (roadLink.Link == toNode)
				{
					result = roadLink.SpacerLineWidth;
					break;
				}
			}
			return result;
		}

		public Vector3 Point;

		public RoadLink[] RoadLinks;

		public int LineCount;

		public float SpeedLimit = float.MaxValue;
	}
}
