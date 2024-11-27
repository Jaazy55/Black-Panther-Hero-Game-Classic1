using System;
using UnityEngine;
using XNode;

namespace Naxeex.AI.StateSystem
{
	public class BehaviourGraph<T> : NodeGraph, IBehaviourGraph, ISerializationCallbackReceiver
	{
		IBehaviourState IBehaviourGraph.StartState
		{
			get
			{
				return this.StartState;
			}
			set
			{
				this.StartState = (value as BehaviourState<T>);
			}
		}

		public override Node AddNode(Type type)
		{
			Node node = base.AddNode(type);
			BehaviourState<T> behaviourState = node as BehaviourState<T>;
			return node;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_StartState = this.StartState;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.StartState = (this.m_StartState as BehaviourState<T>);
		}

		[SerializeField]
		private BehaviourState m_StartState;

		public BehaviourState<T> StartState;
	}
}
