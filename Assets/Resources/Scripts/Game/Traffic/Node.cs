using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Traffic
{
	public class Node : MonoBehaviour
	{
		public List<Node> GetLinkedNodesList()
		{
			List<Node> list = new List<Node>();
			foreach (NodeLink nodeLink in this.NodeLinks)
			{
				list.Add(nodeLink.Link);
			}
			return list;
		}

		public float GetSpaceLineWidthToNode(Node toNode)
		{
			float result = 0f;
			foreach (NodeLink nodeLink in this.NodeLinks)
			{
				if (nodeLink.Link == toNode)
				{
					result = nodeLink.SpacerLineWidth;
					break;
				}
			}
			return result;
		}

		private void OnDrawGizmos()
		{
		}

		public int LineCount = 1;

		public float SpeedLimit = float.PositiveInfinity;

		[Separator("Debug")]
		public float DebugSize = 10f;

		public Color DebugColor = new Color(1f, 1f, 0f, 0.5f);

		public List<Node> Links = new List<Node>();

		public List<NodeLink> NodeLinks = new List<NodeLink>();
	}
}
