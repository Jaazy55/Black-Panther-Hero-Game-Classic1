using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace XNode
{
	public static class NodeDataCache
	{
		private static bool Initialized
		{
			get
			{
				return NodeDataCache.portDataCache != null;
			}
		}

		public static void UpdatePorts(Node node, Dictionary<string, NodePort> ports)
		{
			if (!NodeDataCache.Initialized)
			{
				NodeDataCache.BuildCache();
			}
			Dictionary<string, NodePort> dictionary = new Dictionary<string, NodePort>();
			Type type = node.GetType();
			if (NodeDataCache.portDataCache.ContainsKey(type))
			{
				for (int i = 0; i < NodeDataCache.portDataCache[type].Count; i++)
				{
					dictionary.Add(NodeDataCache.portDataCache[type][i].fieldName, NodeDataCache.portDataCache[type][i]);
				}
			}
			foreach (NodePort nodePort in ports.Values.ToList<NodePort>())
			{
				if (dictionary.ContainsKey(nodePort.fieldName))
				{
					NodePort nodePort2 = dictionary[nodePort.fieldName];
					if (nodePort.connectionType != nodePort2.connectionType || nodePort.IsDynamic || nodePort.direction != nodePort2.direction)
					{
						ports.Remove(nodePort.fieldName);
					}
					else
					{
						nodePort.ValueType = nodePort2.ValueType;
					}
				}
				else if (nodePort.IsStatic)
				{
					ports.Remove(nodePort.fieldName);
				}
			}
			foreach (NodePort nodePort3 in dictionary.Values)
			{
				if (!ports.ContainsKey(nodePort3.fieldName))
				{
					ports.Add(nodePort3.fieldName, new NodePort(nodePort3, node));
				}
			}
		}

		private static void BuildCache()
		{
			NodeDataCache.portDataCache = new NodeDataCache.PortDataCache();
			Type baseType = typeof(Node);
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly assembly = Assembly.GetAssembly(baseType);
			if (assembly.FullName.StartsWith("Assembly-CSharp") && !assembly.FullName.Contains("-firstpass"))
			{
				list.AddRange(from t in assembly.GetTypes()
				where !t.IsAbstract && baseType.IsAssignableFrom(t)
				select t);
			}
			else
			{
				foreach (Assembly assembly2 in assemblies)
				{
					if (!assembly2.FullName.StartsWith("Unity"))
					{
						if (assembly2.FullName.Contains("Version=0.0.0"))
						{
							list.AddRange((from t in assembly2.GetTypes()
							where !t.IsAbstract && baseType.IsAssignableFrom(t)
							select t).ToArray<Type>());
						}
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				NodeDataCache.CachePorts(list[j]);
			}
		}

		private static void CachePorts(Type nodeType)
		{
			FieldInfo[] fields = nodeType.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				object[] customAttributes = fields[i].GetCustomAttributes(false);
				Node.InputAttribute inputAttribute = customAttributes.FirstOrDefault((object x) => x is Node.InputAttribute) as Node.InputAttribute;
				Node.OutputAttribute outputAttribute = customAttributes.FirstOrDefault((object x) => x is Node.OutputAttribute) as Node.OutputAttribute;
				if (inputAttribute != null || outputAttribute != null)
				{
					if (inputAttribute != null && outputAttribute != null)
					{
						UnityEngine.Debug.LogError(string.Concat(new string[]
						{
							"Field ",
							fields[i].Name,
							" of type ",
							nodeType.FullName,
							" cannot be both input and output."
						}));
					}
					else
					{
						if (!NodeDataCache.portDataCache.ContainsKey(nodeType))
						{
							NodeDataCache.portDataCache.Add(nodeType, new List<NodePort>());
						}
						NodeDataCache.portDataCache[nodeType].Add(new NodePort(fields[i]));
					}
				}
			}
		}

		private static NodeDataCache.PortDataCache portDataCache;

		[Serializable]
		private class PortDataCache : Dictionary<Type, List<NodePort>>, ISerializationCallbackReceiver
		{
			public void OnBeforeSerialize()
			{
				this.keys.Clear();
				this.values.Clear();
				foreach (KeyValuePair<Type, List<NodePort>> keyValuePair in this)
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
					throw new Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.", new object[0]));
				}
				for (int i = 0; i < this.keys.Count; i++)
				{
					base.Add(this.keys[i], this.values[i]);
				}
			}

			[SerializeField]
			private List<Type> keys = new List<Type>();

			[SerializeField]
			private List<List<NodePort>> values = new List<List<NodePort>>();
		}
	}
}
