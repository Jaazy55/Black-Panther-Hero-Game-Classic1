using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.AI.StateSystem
{
	[Serializable]
	public class EventTransition<T> : EventTransition, IEventTransition, ISerializationCallbackReceiver
	{
		public EventTransition()
		{
		}

		public EventTransition(EventTransition eventTransition)
		{
			this.m_Decisions = eventTransition.m_Decisions;
			this.m_PortName = eventTransition.m_PortName;
			this.m_DestinationState = eventTransition.m_DestinationState;
		}

		public EventDecision<T>[] Decisions
		{
			get
			{
				if (this.generic_Decisions == null)
				{
					this.InitDecisions();
				}
				return this.generic_Decisions;
			}
		}

		public BehaviourState<T> DestinationState
		{
			get
			{
				if (!this.s_initDestinationState)
				{
					this.InitDestinationState();
				}
				return this.generic_DestinationState;
			}
			private set
			{
				this.generic_DestinationState = value;
				this.m_DestinationState = value;
			}
		}

		string IEventTransition.PortName
		{
			get
			{
				return this.m_PortName;
			}
			set
			{
				this.m_PortName = value;
			}
		}

		IBehaviourState IEventTransition.DestinationState
		{
			get
			{
				return this.DestinationState;
			}
			set
			{
				this.DestinationState = (value as BehaviourState<T>);
			}
		}

		public void SubscribeTransitions(T entity, TransitionHandler<T> transitionActivate)
		{
			if (!this.TransitionActivateEvents.ContainsKey(entity))
			{
				this.TransitionActivateEvents.Add(entity, transitionActivate);
			}
			else
			{
				Dictionary<T, TransitionHandler<T>> transitionActivateEvents= this.TransitionActivateEvents;
				(transitionActivateEvents )[entity] = (TransitionHandler<T>)Delegate.Combine(transitionActivateEvents[entity], transitionActivate);
			}
			foreach (EventDecision<T> eventDecision in this.Decisions)
			{
				if (eventDecision != null)
				{
					eventDecision.Subscribe(entity, new EntityHandler<T>(this.EventInvoke));
				}
			}
		}

		public void UnsubscribeTransitions(T entity, TransitionHandler<T> transitionActivate)
		{
			if (this.TransitionActivateEvents.ContainsKey(entity))
			{
				Dictionary<T, TransitionHandler<T>> transitionActivateEvents= this.TransitionActivateEvents;
				(transitionActivateEvents )[entity] = (TransitionHandler<T>)Delegate.Remove(transitionActivateEvents[entity], transitionActivate);
			}
			foreach (EventDecision<T> eventDecision in this.Decisions)
			{
				if (eventDecision != null)
				{
					eventDecision.Unsubscribe(entity, new EntityHandler<T>(this.EventInvoke));
				}
			}
		}

		private void InitDecisions()
		{
			if (this.m_Decisions != null)
			{
				List<EventDecision<T>> list = new List<EventDecision<T>>();
				foreach (EventDecision eventDecision in this.m_Decisions)
				{
					if (eventDecision is EventDecision<T>)
					{
						list.Add(eventDecision as EventDecision<T>);
					}
				}
				this.generic_Decisions = list.ToArray();
			}
			else
			{
				this.generic_Decisions = new EventDecision<T>[0];
			}
		}

		private void InitDestinationState()
		{
			if (!this.s_initDestinationState)
			{
				if (this.m_DestinationState != null && this.m_DestinationState is BehaviourState<T>)
				{
					this.generic_DestinationState = (this.m_DestinationState as BehaviourState<T>);
				}
				else
				{
					this.generic_DestinationState = null;
				}
				this.s_initDestinationState = true;
			}
		}

		private void EventInvoke(T entity)
		{
			if (this.TransitionActivateEvents.ContainsKey(entity) && this.TransitionActivateEvents[entity] != null)
			{
				this.TransitionActivateEvents[entity](this);
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_Decisions = this.generic_Decisions;
			this.m_DestinationState = this.generic_DestinationState;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.InitDecisions();
			this.InitDestinationState();
		}

		private EventDecision<T>[] generic_Decisions;

		private bool s_initDestinationState;

		private BehaviourState<T> generic_DestinationState;

		private Dictionary<T, TransitionHandler<T>> TransitionActivateEvents = new Dictionary<T, TransitionHandler<T>>();
	}
}
