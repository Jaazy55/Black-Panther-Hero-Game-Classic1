using System;
using Game.Traffic;
using UnityEngine;

public class TrafficNodeClick : MonoBehaviour
{
	public void CreateLink()
	{
		if (this.sourceNodeIsSet && this.linkedNodeIsSet)
		{
			this.newLink.Link = this.linkedNode;
			this.sourceNode.NodeLinks.Add(this.newLink);
			MonoBehaviour.print("New link added in: " + this.sourceNode.gameObject.name + " to: " + this.linkedNode.gameObject.name);
			this.ClearSources();
		}
	}

	public void ClearSources()
	{
		this.sourceNode = null;
		this.linkedNode = null;
		this.sourceNodeIsSet = false;
		this.linkedNodeIsSet = false;
		this.newLink = new NodeLink();
	}

	public void DeleteLink()
	{
		if (this.sourceNodeIsSet)
		{
			if (this.sourceNode.NodeLinks.Count > 0)
			{
				this.sourceNode.NodeLinks.RemoveAt(this.sourceNode.NodeLinks.Count - 1);
				MonoBehaviour.print("Last link in: " + this.sourceNode.gameObject.name + "was been deleted.");
			}
			else
			{
				MonoBehaviour.print(this.sourceNode.gameObject.name + " don't have links.");
			}
		}
	}

	public bool isWork;

	[Tooltip("Press S to add selected node in source node")]
	public Node sourceNode;

	[Tooltip("Press S to add selected node in linked node")]
	public Node linkedNode;

	public bool sourceNodeIsSet;

	public bool linkedNodeIsSet;

	private NodeLink newLink;

	[InspectorButton("CreateLink")]
	public string linkNodes;

	[InspectorButton("ClearSources")]
	public string clearNodes;

	[InspectorButton("DeleteLink")]
	public string deleteLink;
}
