using System;
using System.Collections.Generic;
using Game.Traffic;
using UnityEngine;

namespace Code.Game.Race.RaceTrackManager
{
	[RequireComponent(typeof(Node))]
	public class RaceNodeHelper : MonoBehaviour
	{
		public static void CloneNode()
		{
		}

		public static void LinkNodes()
		{
		}

		public static void UnlinkNodes()
		{
		}

		private static NodeLink FindLink(List<NodeLink> linkList, Node exampleNode)
		{
			NodeLink result = null;
			foreach (NodeLink nodeLink in linkList)
			{
				if (nodeLink.Link == exampleNode)
				{
					result = nodeLink;
				}
			}
			return result;
		}
	}
}
