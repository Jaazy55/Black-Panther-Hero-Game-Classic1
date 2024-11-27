using System;
using System.Collections.Generic;
using UnityEngine;

namespace XNode
{
	[Serializable]
	public abstract class NodeGraph : ScriptableObject
	{
		public T AddNode<T>() where T : Node
		{
			return this.AddNode(typeof(T)) as T;
		}

		public virtual Node AddNode(Type type)
		{
			Node node = ScriptableObject.CreateInstance(type) as Node;
			this.nodes.Add(node);
			node.graph = this;
			return node;
		}

		public virtual Node CopyNode(Node original)
		{
			Node node = UnityEngine.Object.Instantiate<Node>(original);
			node.ClearConnections();
			this.nodes.Add(node);
			node.graph = this;
			return node;
		}

		public void RemoveNode(Node node)
		{
			node.ClearConnections();
			this.nodes.Remove(node);
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(node);
			}
		}

		public void Clear()
		{
			if (Application.isPlaying)
			{
				for (int i = 0; i < this.nodes.Count; i++)
				{
					UnityEngine.Object.Destroy(this.nodes[i]);
				}
			}
			this.nodes.Clear();
		}

		public NodeGraph Copy()
		{
			NodeGraph nodeGraph = UnityEngine.Object.Instantiate<NodeGraph>(this);
			for (int i = 0; i < this.nodes.Count; i++)
			{
				if (!(this.nodes[i] == null))
				{
					Node node = UnityEngine.Object.Instantiate<Node>(this.nodes[i]);
					node.graph = nodeGraph;
					nodeGraph.nodes[i] = node;
				}
			}
			for (int j = 0; j < nodeGraph.nodes.Count; j++)
			{
				if (!(nodeGraph.nodes[j] == null))
				{
					foreach (NodePort nodePort in nodeGraph.nodes[j].Ports)
					{
						nodePort.Redirect(this.nodes, nodeGraph.nodes);
					}
				}
			}
			return nodeGraph;
		}

		private void OnDestroy()
		{
			this.Clear();
		}

		[SerializeField]
		public List<Node> nodes = new List<Node>();
	}
}
