using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XNode
{
	[Serializable]
	public class NodePort
	{
		public NodePort(FieldInfo fieldInfo)
		{
			this._fieldName = fieldInfo.Name;
			this.ValueType = fieldInfo.FieldType;
			this._dynamic = false;
			object[] customAttributes = fieldInfo.GetCustomAttributes(false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				if (customAttributes[i] is Node.InputAttribute)
				{
					this._direction = NodePort.IO.Input;
					this._connectionType = (customAttributes[i] as Node.InputAttribute).connectionType;
				}
				else if (customAttributes[i] is Node.OutputAttribute)
				{
					this._direction = NodePort.IO.Output;
					this._connectionType = (customAttributes[i] as Node.OutputAttribute).connectionType;
				}
			}
		}

		public NodePort(NodePort nodePort, Node node)
		{
			this._fieldName = nodePort._fieldName;
			this.ValueType = nodePort.valueType;
			this._direction = nodePort.direction;
			this._dynamic = nodePort._dynamic;
			this._connectionType = nodePort._connectionType;
			this._node = node;
		}

		public NodePort(string fieldName, Type type, NodePort.IO direction, Node.ConnectionType connectionType, Node node)
		{
			this._fieldName = fieldName;
			this.ValueType = type;
			this._direction = direction;
			this._node = node;
			this._dynamic = true;
			this._connectionType = connectionType;
		}

		public int ConnectionCount
		{
			get
			{
				return this.connections.Count;
			}
		}

		public NodePort Connection
		{
			get
			{
				for (int i = 0; i < this.connections.Count; i++)
				{
					if (this.connections[i] != null)
					{
						return this.connections[i].Port;
					}
				}
				return null;
			}
		}

		public NodePort.IO direction
		{
			get
			{
				return this._direction;
			}
		}

		public Node.ConnectionType connectionType
		{
			get
			{
				return this._connectionType;
			}
		}

		public bool IsConnected
		{
			get
			{
				return this.connections.Count != 0;
			}
		}

		public bool IsInput
		{
			get
			{
				return this.direction == NodePort.IO.Input;
			}
		}

		public bool IsOutput
		{
			get
			{
				return this.direction == NodePort.IO.Output;
			}
		}

		public string fieldName
		{
			get
			{
				return this._fieldName;
			}
		}

		public Node node
		{
			get
			{
				return this._node;
			}
		}

		public bool IsDynamic
		{
			get
			{
				return this._dynamic;
			}
		}

		public bool IsStatic
		{
			get
			{
				return !this._dynamic;
			}
		}

		public Type ValueType
		{
			get
			{
				if (this.valueType == null && !string.IsNullOrEmpty(this._typeQualifiedName))
				{
					this.valueType = Type.GetType(this._typeQualifiedName, false);
				}
				return this.valueType;
			}
			set
			{
				this.valueType = value;
				if (value != null)
				{
					this._typeQualifiedName = value.AssemblyQualifiedName;
				}
			}
		}

		public void VerifyConnections()
		{
			for (int i = this.connections.Count - 1; i >= 0; i--)
			{
				if (!(this.connections[i].node != null) || string.IsNullOrEmpty(this.connections[i].fieldName) || this.connections[i].node.GetPort(this.connections[i].fieldName) == null)
				{
					this.connections.RemoveAt(i);
				}
			}
		}

		public object GetOutputValue()
		{
			if (this.direction == NodePort.IO.Input)
			{
				return null;
			}
			return this.node.GetValue(this);
		}

		public object GetInputValue()
		{
			NodePort connection = this.Connection;
			if (connection == null)
			{
				return null;
			}
			return connection.GetOutputValue();
		}

		public object[] GetInputValues()
		{
			object[] array = new object[this.ConnectionCount];
			for (int i = 0; i < this.ConnectionCount; i++)
			{
				NodePort port = this.connections[i].Port;
				if (port == null)
				{
					this.connections.RemoveAt(i);
					i--;
				}
				else
				{
					array[i] = port.GetOutputValue();
				}
			}
			return array;
		}

		public T GetInputValue<T>()
		{
			object inputValue = this.GetInputValue();
			return (!(inputValue is T)) ? default(T) : ((T)((object)inputValue));
		}

		public T[] GetInputValues<T>()
		{
			object[] inputValues = this.GetInputValues();
			T[] array = new T[inputValues.Length];
			for (int i = 0; i < inputValues.Length; i++)
			{
				if (inputValues[i] is T)
				{
					array[i] = (T)((object)inputValues[i]);
				}
			}
			return array;
		}

		public bool TryGetInputValue<T>(out T value)
		{
			object inputValue = this.GetInputValue();
			if (inputValue is T)
			{
				value = (T)((object)inputValue);
				return true;
			}
			value = default(T);
			return false;
		}

		public float GetInputSum(float fallback)
		{
			object[] inputValues = this.GetInputValues();
			if (inputValues.Length == 0)
			{
				return fallback;
			}
			float num = 0f;
			for (int i = 0; i < inputValues.Length; i++)
			{
				if (inputValues[i] is float)
				{
					num += (float)inputValues[i];
				}
			}
			return num;
		}

		public int GetInputSum(int fallback)
		{
			object[] inputValues = this.GetInputValues();
			if (inputValues.Length == 0)
			{
				return fallback;
			}
			int num = 0;
			for (int i = 0; i < inputValues.Length; i++)
			{
				if (inputValues[i] is int)
				{
					num += (int)inputValues[i];
				}
			}
			return num;
		}

		public void Connect(NodePort port)
		{
			if (this.connections == null)
			{
				this.connections = new List<NodePort.PortConnection>();
			}
			if (port == null)
			{
				UnityEngine.Debug.LogWarning("Cannot connect to null port");
				return;
			}
			if (port == this)
			{
				UnityEngine.Debug.LogWarning("Cannot connect port to self.");
				return;
			}
			if (this.IsConnectedTo(port))
			{
				UnityEngine.Debug.LogWarning("Port already connected. ");
				return;
			}
			if (this.direction == port.direction)
			{
				UnityEngine.Debug.LogWarning("Cannot connect two " + ((this.direction != NodePort.IO.Input) ? "output" : "input") + " connections");
				return;
			}
			if (port.connectionType == Node.ConnectionType.Override && port.ConnectionCount != 0)
			{
				port.ClearConnections();
			}
			if (this.connectionType == Node.ConnectionType.Override && this.ConnectionCount != 0)
			{
				this.ClearConnections();
			}
			this.connections.Add(new NodePort.PortConnection(port));
			if (port.connections == null)
			{
				port.connections = new List<NodePort.PortConnection>();
			}
			if (!port.IsConnectedTo(this))
			{
				port.connections.Add(new NodePort.PortConnection(this));
			}
			this.node.OnCreateConnection(this, port);
			port.node.OnCreateConnection(this, port);
		}

		public NodePort GetConnection(int i)
		{
			if (this.connections[i].node == null || string.IsNullOrEmpty(this.connections[i].fieldName))
			{
				this.connections.RemoveAt(i);
				return null;
			}
			NodePort port = this.connections[i].node.GetPort(this.connections[i].fieldName);
			if (port == null)
			{
				this.connections.RemoveAt(i);
				return null;
			}
			return port;
		}

		public int GetConnectionIndex(NodePort port)
		{
			for (int i = 0; i < this.ConnectionCount; i++)
			{
				if (this.connections[i].Port == port)
				{
					return i;
				}
			}
			return -1;
		}

		public bool IsConnectedTo(NodePort port)
		{
			for (int i = 0; i < this.connections.Count; i++)
			{
				if (this.connections[i].Port == port)
				{
					return true;
				}
			}
			return false;
		}

		public void Disconnect(NodePort port)
		{
			for (int i = this.connections.Count - 1; i >= 0; i--)
			{
				if (this.connections[i].Port == port)
				{
					this.connections.RemoveAt(i);
				}
			}
			if (port != null)
			{
				for (int j = 0; j < port.connections.Count; j++)
				{
					if (port.connections[j].Port == this)
					{
						port.connections.RemoveAt(j);
					}
				}
			}
			this.node.OnRemoveConnection(this);
			if (port != null)
			{
				port.node.OnRemoveConnection(port);
			}
		}

		public void ClearConnections()
		{
			while (this.connections.Count > 0)
			{
				this.Disconnect(this.connections[0].Port);
			}
		}

		public List<Vector2> GetReroutePoints(int index)
		{
			return this.connections[index].reroutePoints;
		}

		public void Redirect(List<Node> oldNodes, List<Node> newNodes)
		{
			UnityEngine.Debug.Log("Redirect");
			foreach (NodePort.PortConnection portConnection in this.connections)
			{
				int num = oldNodes.IndexOf(portConnection.node);
				if (num >= 0)
				{
					portConnection.node = newNodes[num];
				}
			}
		}

		private Type valueType;

		[SerializeField]
		private string _fieldName;

		[SerializeField]
		private Node _node;

		[SerializeField]
		private string _typeQualifiedName;

		[SerializeField]
		private List<NodePort.PortConnection> connections = new List<NodePort.PortConnection>();

		[SerializeField]
		private NodePort.IO _direction;

		[SerializeField]
		private Node.ConnectionType _connectionType;

		[SerializeField]
		private bool _dynamic;

		public enum IO
		{
			Input,
			Output
		}

		[Serializable]
		private class PortConnection
		{
			public PortConnection(NodePort port)
			{
				this.port = port;
				this.node = port.node;
				this.fieldName = port.fieldName;
			}

			public NodePort Port
			{
				get
				{
					return (this.port == null) ? (this.port = this.GetPort()) : this.port;
				}
			}

			private NodePort GetPort()
			{
				if (this.node == null || string.IsNullOrEmpty(this.fieldName))
				{
					return null;
				}
				return this.node.GetPort(this.fieldName);
			}

			[SerializeField]
			public string fieldName;

			[SerializeField]
			public Node node;

			[NonSerialized]
			private NodePort port;

			[SerializeField]
			public List<Vector2> reroutePoints = new List<Vector2>();
		}
	}
}
