using System;
using UnityEngine;

namespace Game.Traffic
{
	[Serializable]
	public class NodeLink
	{
		public Node Link;

		[Range(0f, 10f)]
		public float SpacerLineWidth;
	}
}
