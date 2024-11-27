using System;
using System.Collections.Generic;
using UnityEngine;

namespace XNode
{
	[Serializable]
	public abstract class Node : ScriptableObject
	{
		public IEnumerable<NodePort> Ports
		{
			get
			{
				foreach (NodePort port in this.ports.Values)
				{
					yield return port;
				}
				yield break;
			}
		}

		public IEnumerable<NodePort> Outputs
		{
			get
			{
				foreach (NodePort port in this.Ports)
				{
					if (port.IsOutput)
					{
						yield return port;
					}
				}
				yield break;
			}
		}

		public IEnumerable<NodePort> Inputs
		{
			get
			{
				foreach (NodePort port in this.Ports)
				{
					if (port.IsInput)
					{
						yield return port;
					}
				}
				yield break;
			}
		}

		public IEnumerable<NodePort> InstancePorts
		{
			get
			{
				foreach (NodePort port in this.Ports)
				{
					if (port.IsDynamic)
					{
						yield return port;
					}
				}
				yield break;
			}
		}

		public IEnumerable<NodePort> InstanceOutputs
		{
			get
			{
				foreach (NodePort port in this.Ports)
				{
					if (port.IsDynamic && port.IsOutput)
					{
						yield return port;
					}
				}
				yield break;
			}
		}

		public IEnumerable<NodePort> InstanceInputs
		{
			get
			{
				foreach (NodePort port in this.Ports)
				{
					if (port.IsDynamic && port.IsInput)
					{
						yield return port;
					}
				}
				yield break;
			}
		}

		protected void OnEnable()
		{
			this.UpdateStaticPorts();
			this.Init();
		}

		public void UpdateStaticPorts()
		{
			NodeDataCache.UpdatePorts(this, this.ports);
		}

		protected virtual void Init()
		{
		}

		public void VerifyConnections()
		{
			foreach (NodePort nodePort in this.Ports)
			{
				nodePort.VerifyConnections();
			}
		}

		public NodePort AddInstanceInput(Type type, Node.ConnectionType connectionType = Node.ConnectionType.Multiple, string fieldName = null)
		{
			return this.AddInstancePort(type, NodePort.IO.Input, connectionType, fieldName);
		}

		public NodePort AddInstanceOutput(Type type, Node.ConnectionType connectionType = Node.ConnectionType.Multiple, string fieldName = null)
		{
			return this.AddInstancePort(type, NodePort.IO.Output, connectionType, fieldName);
		}

		private NodePort AddInstancePort(Type type, NodePort.IO direction, Node.ConnectionType connectionType = Node.ConnectionType.Multiple, string fieldName = null)
		{
			if (fieldName == null)
			{
				fieldName = "instanceInput_0";
				int num = 0;
				while (this.HasPort(fieldName))
				{
					fieldName = "instanceInput_" + ++num;
				}
			}
			else if (this.HasPort(fieldName))
			{
				UnityEngine.Debug.LogWarning("Port '" + fieldName + "' already exists in " + base.name, this);
				return this.ports[fieldName];
			}
			NodePort nodePort = new NodePort(fieldName, type, direction, connectionType, this);
			this.ports.Add(fieldName, nodePort);
			return nodePort;
		}

		public void RemoveInstancePort(string fieldName)
		{
			this.RemoveInstancePort(this.GetPort(fieldName));
		}

		public void RemoveInstancePort(NodePort port)
		{
			if (port == null)
			{
				throw new ArgumentNullException("port");
			}
			if (port.IsStatic)
			{
				throw new ArgumentException("cannot remove static port");
			}
			port.ClearConnections();
			this.ports.Remove(port.fieldName);
		}

		[ContextMenu("Clear Instance Ports")]
		public void ClearInstancePorts()
		{
			List<NodePort> list = new List<NodePort>(this.InstancePorts);
			foreach (NodePort port in list)
			{
				this.RemoveInstancePort(port);
			}
		}

		public NodePort GetOutputPort(string fieldName)
		{
			NodePort port = this.GetPort(fieldName);
			if (port == null || port.direction != NodePort.IO.Output)
			{
				return null;
			}
			return port;
		}

		public NodePort GetInputPort(string fieldName)
		{
			NodePort port = this.GetPort(fieldName);
			if (port == null || port.direction != NodePort.IO.Input)
			{
				return null;
			}
			return port;
		}

		public NodePort GetPort(string fieldName)
		{
			if (!string.IsNullOrEmpty(fieldName) && this.ports.ContainsKey(fieldName))
			{
				return this.ports[fieldName];
			}
			return null;
		}

		public bool HasPort(string fieldName)
		{
			return this.ports.ContainsKey(fieldName);
		}

		public T GetInputValue<T>(string fieldName, T fallback = default(T))
		{
			NodePort port = this.GetPort(fieldName);
			if (port != null && port.IsConnected)
			{
				return port.GetInputValue<T>();
			}
			return fallback;
		}

		public T[] GetInputValues<T>(string fieldName, params T[] fallback)
		{
			NodePort port = this.GetPort(fieldName);
			if (port != null && port.IsConnected)
			{
				return port.GetInputValues<T>();
			}
			return fallback;
		}

		public virtual object GetValue(NodePort port)
		{
			UnityEngine.Debug.LogWarning("No GetValue(NodePort port) override defined for " + base.GetType());
			return null;
		}

		public virtual void OnCreateConnection(NodePort from, NodePort to)
		{
		}

		public virtual void OnRemoveConnection(NodePort port)
		{
		}

		public void ClearConnections()
		{
			foreach (NodePort nodePort in this.Ports)
			{
				nodePort.ClearConnections();
			}
		}

		public override int GetHashCode()
		{
			return JsonUtility.ToJson(this).GetHashCode();
		}

		[SerializeField]
		public NodeGraph graph;

		[SerializeField]
		public Vector2 position;

		[SerializeField]
		private Node.NodePortDictionary ports = new Node.NodePortDictionary();

		public enum ShowBackingValue
		{
			Never,
			Unconnected,
			Always
		}

		public enum ConnectionType
		{
			Multiple,
			Override
		}

		[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
		public class InputAttribute : Attribute
		{
			public InputAttribute(Node.ShowBackingValue backingValue = Node.ShowBackingValue.Unconnected, Node.ConnectionType connectionType = Node.ConnectionType.Multiple)
			{
				this.backingValue = backingValue;
				this.connectionType = connectionType;
			}

			public Node.ShowBackingValue backingValue;

			public Node.ConnectionType connectionType;
		}

		[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
		public class OutputAttribute : Attribute
		{
			public OutputAttribute(Node.ShowBackingValue backingValue = Node.ShowBackingValue.Never, Node.ConnectionType connectionType = Node.ConnectionType.Multiple)
			{
				this.backingValue = backingValue;
				this.connectionType = connectionType;
			}

			public Node.ShowBackingValue backingValue;

			public Node.ConnectionType connectionType;
		}

		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
		public class CreateNodeMenuAttribute : Attribute
		{
			public CreateNodeMenuAttribute(string menuName)
			{
				this.menuName = menuName;
			}

			public string menuName;
		}

		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
		public class NodeTint : Attribute
		{
			public NodeTint(float r, float g, float b)
			{
				this.color = new Color(r, g, b);
			}

			public NodeTint(string hex)
			{
				ColorUtility.TryParseHtmlString(hex, out this.color);
			}

			public NodeTint(byte r, byte g, byte b)
			{
				this.color = new Color32(r, g, b, byte.MaxValue);
			}

			public Color color;
		}

		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
		public class NodeWidth : Attribute
		{
			public NodeWidth(int width)
			{
				this.width = width;
			}

			public int width;
		}

		[Serializable]
		private class NodePortDictionary : Dictionary<string, NodePort>, ISerializationCallbackReceiver
		{
			public void OnBeforeSerialize()
			{
				this.keys.Clear();
				this.values.Clear();
				foreach (KeyValuePair<string, NodePort> keyValuePair in this)
				{
					this.keys.Add(keyValuePair.Key);
					this.values.Add(keyValuePair.Value);
				}
			}

			public void OnAfterDeserialize()
			{
				base.Clear();
				if (this.keys.Count != this.values.Count)
				{
					throw new Exception(string.Concat(new object[]
					{
						"there are ",
						this.keys.Count,
						" keys and ",
						this.values.Count,
						" values after deserialization. Make sure that both key and value types are serializable."
					}));
				}
				for (int i = 0; i < this.keys.Count; i++)
				{
					base.Add(this.keys[i], this.values[i]);
				}
			}

			[SerializeField]
			private List<string> keys = new List<string>();

			[SerializeField]
			private List<NodePort> values = new List<NodePort>();
		}
	}
}
