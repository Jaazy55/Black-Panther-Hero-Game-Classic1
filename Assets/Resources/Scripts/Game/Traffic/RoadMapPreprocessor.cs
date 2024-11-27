using System;
using System.Collections.Generic;
using System.Linq;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Traffic
{
	public class RoadMapPreprocessor : MonoBehaviour
	{
		private string PrebuildRoadPoints(Node[] nodes)
		{
			if (nodes == null)
			{
				return string.Empty;
			}
			this.usedNodes.Clear();
			this.nodeToPoints.Clear();
			this.points.Clear();
			foreach (Node node in nodes)
			{
				this.NodeBypass(node);
			}
			this.usedRPs.Clear();
			foreach (RoadPoint rp in this.nodeToPoints.Values)
			{
				this.RoadPointDistanceBypass(rp);
			}
			return MiamiSerializier.JSONSerialize(this.points.ToArray<RoadPoint>());
		}

		private void NodeBypass(Node node)
		{
			if (this.usedNodes.Contains(node) || node.NodeLinks == null || node.NodeLinks.Count == 0)
			{
				return;
			}
			this.usedNodes.Add(node);
			RoadPoint roadPoint = new RoadPoint();
			roadPoint.Point = node.transform.position;
			roadPoint.LineCount = node.LineCount;
			roadPoint.SpeedLimit = ((!node.SpeedLimit.Equals(float.PositiveInfinity)) ? node.SpeedLimit : float.MaxValue);
			roadPoint.RoadLinks = new RoadLink[node.NodeLinks.Count];
			this.points.Add(roadPoint);
			this.nodeToPoints.Add(node, roadPoint);
			int num = 0;
			foreach (Node node2 in node.GetLinkedNodesList())
			{
				if (!this.usedNodes.Contains(node2))
				{
					this.NodeBypass(node2);
				}
				RoadPoint link = this.nodeToPoints[node2];
				roadPoint.RoadLinks[num] = new RoadLink
				{
					Link = link,
					SpacerLineWidth = node.GetSpaceLineWidthToNode(node2)
				};
				num++;
			}
		}

		private void RoadPointDistanceBypass(RoadPoint rp)
		{
			if (this.usedRPs.Contains(rp))
			{
				return;
			}
			this.usedRPs.Add(rp);
			int num = 0;
			for (int i = 0; i < rp.RoadLinks.Length; i++)
			{
				RoadPoint link = rp.RoadLinks[i].Link;
				if (!this.usedRPs.Contains(link))
				{
					float num2 = Vector3.Distance(link.Point, rp.Point);
					if (num2 > TrafficManager.NodesMaxDistance)
					{
						int num3 = (int)(num2 / TrafficManager.NodesMaxDistance);
						float d = num2 / (float)(num3 + 1);
						Vector3 normalized = (link.Point - rp.Point).normalized;
						RoadPoint roadPoint = rp;
						float spacerLineWidth = (link.GetSpaceLineWidthToNode(rp) + rp.GetSpaceLineWidthToNode(link)) / 2f;
						for (int j = 0; j < num3; j++)
						{
							RoadPoint roadPoint2 = new RoadPoint();
							this.usedRPs.Add(roadPoint2);
							this.points.Add(roadPoint2);
							roadPoint2.LineCount = rp.LineCount;
							roadPoint2.Point = rp.Point + normalized * d * (float)(j + 1);
							roadPoint2.RoadLinks = new RoadLink[2];
							roadPoint2.RoadLinks[0] = new RoadLink
							{
								Link = roadPoint,
								SpacerLineWidth = spacerLineWidth
							};
							if (roadPoint.Equals(rp))
							{
								roadPoint.RoadLinks[i].Link = roadPoint2;
								roadPoint.RoadLinks[i].SpacerLineWidth = spacerLineWidth;
							}
							else
							{
								roadPoint.RoadLinks[1] = new RoadLink
								{
									Link = roadPoint2,
									SpacerLineWidth = spacerLineWidth
								};
							}
							roadPoint = roadPoint2;
						}
						for (int k = 0; k < link.RoadLinks.Length; k++)
						{
							if (link.RoadLinks[k].Link.Equals(rp))
							{
								link.RoadLinks[k].Link = roadPoint;
								link.RoadLinks[k].SpacerLineWidth = spacerLineWidth;
								roadPoint.RoadLinks[1] = new RoadLink
								{
									Link = link,
									SpacerLineWidth = spacerLineWidth
								};
								break;
							}
						}
					}
				}
				num++;
			}
		}

		public void Rebuild()
		{
		}

		private static void CureNodes(Node[] nodes)
		{
			foreach (Node node in nodes)
			{
				List<NodeLink> nodeLinks = node.NodeLinks;
				List<int> list = new List<int>();
				int num = 0;
				foreach (NodeLink nodeLink in nodeLinks)
				{
					if (nodeLink.Link == null || nodeLink.Link == node)
					{
						list.Add(num);
					}
					else
					{
						List<Node> linkedNodesList = nodeLink.Link.GetLinkedNodesList();
						if (!linkedNodesList.Contains(node))
						{
							nodeLink.Link.NodeLinks.Add(new NodeLink
							{
								Link = node,
								SpacerLineWidth = nodeLink.SpacerLineWidth
							});
						}
					}
					num++;
				}
				for (int j = list.Count - 1; j >= 0; j--)
				{
					int index = list[j];
					nodeLinks.RemoveAt(index);
				}
			}
		}

		private HashSet<RoadPoint> points = new HashSet<RoadPoint>();

		private HashSet<Node> usedNodes = new HashSet<Node>();

		private HashSet<RoadPoint> usedRPs = new HashSet<RoadPoint>();

		private IDictionary<Node, RoadPoint> nodeToPoints = new Dictionary<Node, RoadPoint>();

		private string serializedMap;
	}
}
