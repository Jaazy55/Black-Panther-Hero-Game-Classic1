using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Naxeex.AI.StateSystem
{
	[Node.NodeWidth(400)]
	public abstract class BehaviourState<T> : BehaviourState, ISerializationCallbackReceiver, IBehaviourState
	{
		public StateAction<T>[] Actions
		{
			get
			{
				if (this.generic_Actions == null)
				{
					this.InitActions();
				}
				return this.generic_Actions;
			}
			set
			{
				this.generic_Actions = value;
				this.m_Actions = value;
			}
		}

		public ProcessedTransition<T>[] ProcessedTransitions
		{
			get
			{
				if (this.generic_ProcessedTransitions == null)
				{
					this.InitProcessedTransitions();
				}
				return this.generic_ProcessedTransitions;
			}
			private set
			{
				this.generic_ProcessedTransitions = value;
				this.m_ProcessedTransitions = value;
			}
		}

		public EventTransition<T>[] EventTransitions
		{
			get
			{
				if (this.generic_EventTransitions == null)
				{
					this.InitEventTransitions();
				}
				return this.generic_EventTransitions;
			}
			private set
			{
				this.generic_EventTransitions = value;
				this.m_EventTransitions = value;
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_Actions = this.generic_Actions;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.InitActions();
		}

		IBehaviourGraph IBehaviourState.graph
		{
			get
			{
				return this.graph as IBehaviourGraph;
			}
		}

		IProcessedTransition[] IBehaviourState.ProcessedTransitions
		{
			get
			{
				return this.ProcessedTransitions;
			}
			set
			{
				this.ProcessedTransitions = (value as ProcessedTransition<T>[]);
			}
		}

		IEventTransition[] IBehaviourState.EventTransitions
		{
			get
			{
				return this.EventTransitions;
			}
			set
			{
				this.EventTransitions = (value as EventTransition<T>[]);
			}
		}

		string IBehaviourState.name
		{
			get
			{
				return base.name;
			}
		}

		bool IBehaviourState.HasPort(string portName)
		{
			return base.HasPort(portName);
		}

		void IBehaviourState.RemoveInstancePort(string portName)
		{
			base.RemoveInstancePort(portName);
		}

		NodePort IBehaviourState.AddInstanceOutput(Node.ConnectionType connectionType, string fieldName)
		{
			return base.AddInstanceOutput(base.GetType(), connectionType, fieldName);
		}

		IEventTransition IBehaviourState.CreateEventTransition()
		{
			if (this.EventTransitions == null)
			{
				this.EventTransitions = new EventTransition<T>[0];
			}
			EventTransition<T> eventTransition = new EventTransition<T>();
			this.EventTransitions = new List<EventTransition<T>>(this.EventTransitions)
			{
				eventTransition
			}.ToArray();
			return eventTransition;
		}

		IProcessedTransition IBehaviourState.CreateProcessedTransition()
		{
			if (this.ProcessedTransitions == null)
			{
				this.ProcessedTransitions = new ProcessedTransition<T>[0];
			}
			ProcessedTransition<T> processedTransition = new ProcessedTransition<T>();
			this.ProcessedTransitions = new List<ProcessedTransition<T>>(this.ProcessedTransitions)
			{
				processedTransition
			}.ToArray();
			return processedTransition;
		}

		void IBehaviourState.RemoveEventTransitionAt(int index)
		{
			EventTransition<T> eventTransition = this.EventTransitions[index];
			List<EventTransition<T>> list = new List<EventTransition<T>>(this.EventTransitions);
			list.RemoveAt(index);
			if (base.HasPort(eventTransition.m_PortName))
			{
				base.RemoveInstancePort(eventTransition.m_PortName);
			}
			this.EventTransitions = list.ToArray();
		}

		void IBehaviourState.RemoveProcessedTransitionAt(int index)
		{
			ProcessedTransition<T> processedTransition = this.ProcessedTransitions[index];
			List<ProcessedTransition<T>> list = new List<ProcessedTransition<T>>(this.ProcessedTransitions);
			list.RemoveAt(index);
			if (base.HasPort(processedTransition.TruePortName))
			{
				base.RemoveInstancePort(processedTransition.TruePortName);
			}
			if (base.HasPort(processedTransition.FalsePortName))
			{
				base.RemoveInstancePort(processedTransition.FalsePortName);
			}
			this.ProcessedTransitions = list.ToArray();
		}

		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			base.OnCreateConnection(from, to);
			this.SetupStates();
		}

		public override void OnRemoveConnection(NodePort port)
		{
			base.OnRemoveConnection(port);
			this.SetupStates();
		}

		public override object GetValue(NodePort port)
		{
			if (port.IsConnected)
			{
				NodePort connection = port.GetConnection(0);
				if (connection != null)
				{
					return connection.node as BehaviourState<T>;
				}
			}
			return null;
		}

		internal void OnStateEnter(T entity)
		{
			foreach (StateAction<T> stateAction in this.Actions)
			{
				if (stateAction != null)
				{
					stateAction.OnStateEnter(entity);
				}
			}
		}

		internal void OnStateExit(T entity)
		{
			foreach (StateAction<T> stateAction in this.Actions)
			{
				if (stateAction != null)
				{
					stateAction.OnStateExit(entity);
				}
			}
		}

		internal void OnStateUpdate(T entity)
		{
			foreach (StateAction<T> stateAction in this.Actions)
			{
				if (stateAction != null)
				{
					stateAction.OnStateUpdate(entity);
				}
			}
		}

		internal void ProcessTransitions(T entity)
		{
			foreach (ProcessedTransition<T> processedTransition in this.ProcessedTransitions)
			{
				if (processedTransition != null)
				{
					processedTransition.CheckCondition(entity);
				}
			}
		}

		internal void SubscribeTransitions(T entity, TransitionHandler<T> transitionActivate)
		{
			foreach (EventTransition<T> eventTransition in this.EventTransitions)
			{
				if (eventTransition != null)
				{
					eventTransition.SubscribeTransitions(entity, transitionActivate);
				}
			}
		}

		internal void UnsubscribeTransitions(T entity, TransitionHandler<T> transitionActivate)
		{
			foreach (EventTransition<T> eventTransition in this.EventTransitions)
			{
				if (eventTransition != null)
				{
					eventTransition.UnsubscribeTransitions(entity, transitionActivate);
				}
			}
		}

		private void InitActions()
		{
			if (this.m_Actions != null && this.m_Actions.Length > 0)
			{
				List<StateAction<T>> list = new List<StateAction<T>>();
				foreach (StateAction stateAction in this.m_Actions)
				{
					if (stateAction is StateAction<T>)
					{
						list.Add(stateAction as StateAction<T>);
					}
				}
				this.generic_Actions = list.ToArray();
			}
			else
			{
				this.generic_Actions = new StateAction<T>[0];
			}
		}

		private void InitProcessedTransitions()
		{
			if (this.m_ProcessedTransitions != null && this.m_ProcessedTransitions.Length > 0)
			{
				List<ProcessedTransition<T>> list = new List<ProcessedTransition<T>>();
				foreach (ProcessedTransition processedTransition in this.m_ProcessedTransitions)
				{
					if (processedTransition != null && processedTransition is ProcessedTransition<T>)
					{
						list.Add(processedTransition as ProcessedTransition<T>);
					}
					else
					{
						list.Add(new ProcessedTransition<T>(processedTransition));
					}
				}
				this.generic_ProcessedTransitions = list.ToArray();
			}
			else
			{
				this.generic_ProcessedTransitions = new ProcessedTransition<T>[0];
			}
		}

		private void InitEventTransitions()
		{
			if (this.m_EventTransitions != null && this.m_EventTransitions.Length > 0)
			{
				List<EventTransition<T>> list = new List<EventTransition<T>>();
				foreach (EventTransition eventTransition in this.m_EventTransitions)
				{
					if (eventTransition != null && eventTransition is EventTransition<T>)
					{
						list.Add(eventTransition as EventTransition<T>);
					}
					else
					{
						list.Add(new EventTransition<T>(eventTransition));
					}
				}
				this.generic_EventTransitions = list.ToArray();
			}
			else
			{
				this.generic_EventTransitions = new EventTransition<T>[0];
			}
		}

		private void SetupStates()
		{
			foreach (ProcessedTransition<T> processedTransition in this.ProcessedTransitions)
			{
				NodePort outputPort = base.GetOutputPort(processedTransition.TruePortName);
				if (outputPort != null)
				{
					if (outputPort.IsConnected)
					{
						BehaviourState<T> behaviourState = outputPort.GetOutputValue() as BehaviourState<T>;
						processedTransition.TrueState = ((!(behaviourState != null)) ? null : behaviourState);
					}
					else
					{
						processedTransition.TrueState = null;
					}
					NodePort outputPort2 = base.GetOutputPort(processedTransition.FalsePortName);
					if (outputPort2 != null)
					{
						if (outputPort2.IsConnected)
						{
							BehaviourState<T> behaviourState2 = outputPort2.GetOutputValue() as BehaviourState<T>;
							processedTransition.FalseState = ((!(behaviourState2 != null)) ? null : behaviourState2);
						}
						else
						{
							processedTransition.FalseState = null;
						}
					}
				}
			}
			this.m_ProcessedTransitions = this.generic_ProcessedTransitions;
			foreach (EventTransition<T> eventTransition in this.EventTransitions)
			{
				NodePort outputPort3 = base.GetOutputPort(eventTransition.m_PortName);
				if (outputPort3 != null)
				{
					if (outputPort3.IsConnected)
					{
						BehaviourState<T> behaviourState3 = outputPort3.GetOutputValue() as BehaviourState<T>;
						eventTransition.m_DestinationState = ((!(behaviourState3 != null)) ? null : behaviourState3);
					}
					else
					{
						eventTransition.m_DestinationState = null;
					}
				}
			}
			this.m_EventTransitions = this.generic_EventTransitions;
		}

		private StateAction<T>[] generic_Actions;

		private ProcessedTransition<T>[] generic_ProcessedTransitions;

		private EventTransition<T>[] generic_EventTransitions;
	}
}
