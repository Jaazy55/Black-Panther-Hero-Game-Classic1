using System;
using UnityEngine;

namespace Naxeex.AI.StateSystem
{
	public class BehaviourController<T>
	{
		public BehaviourController(T entity, BehaviourGraph<T> behaviour)
		{
			this.m_entity = entity;
			this.m_behaviour = behaviour;
			if (behaviour != null)
			{
				this.CurrentState = behaviour.StartState;
			}
			else
			{
				this.m_currentState = null;
			}
			if (this.m_currentState == null)
			{
				UnityEngine.Debug.LogError("Start state of " + behaviour + " is null ");
			}
		}

		~BehaviourController()
		{
			if (this.m_currentState != null)
			{
				this.m_currentState.OnStateExit(this.m_entity);
				this.m_currentState.UnsubscribeTransitions(this.m_entity, new TransitionHandler<T>(this.EventTransitionActivate));
			}
		}

		public T Entity
		{
			get
			{
				return this.m_entity;
			}
		}

		public BehaviourGraph<T> Behaviour
		{
			get
			{
				return this.m_behaviour;
			}
		}

		public BehaviourState<T> CurrentState
		{
			get
			{
				return this.m_currentState;
			}
			private set
			{
				this.TransitionToState(value);
			}
		}

		public void SetBehaviourGraph(BehaviourGraph<T> newBehaviourGraph)
		{
			this.m_behaviour = newBehaviourGraph;
			this.CurrentState = this.m_behaviour.StartState;
			this.needFirstUpdate = true;
		}

		public void ProcessUpdate()
		{
			if (this.m_entity != null && this.m_behaviour != null && this.m_currentState != null)
			{
				this.ProcessActions();
				this.ProcessTransitions();
			}
		}

		public void ProcessActions()
		{
			this.m_currentState.OnStateUpdate(this.m_entity);
		}

		public void ProcessTransitions()
		{
			for (int i = 0; i < this.m_currentState.ProcessedTransitions.Length; i++)
			{
				ProcessedTransition<T> processedTransition = this.m_currentState.ProcessedTransitions[i];
				if (processedTransition.CheckCondition(this.m_entity))
				{
					if (processedTransition.TrueState != null && this.TransitionToState(processedTransition.TrueState))
					{
						return;
					}
				}
				else if (processedTransition.FalseState != null && this.TransitionToState(processedTransition.FalseState))
				{
					return;
				}
			}
		}

		private bool TransitionToState(BehaviourState<T> nextState)
		{
			if (nextState != this.m_currentState)
			{
				if (this.m_currentState != null)
				{
					this.m_currentState.OnStateExit(this.m_entity);
					this.m_currentState.UnsubscribeTransitions(this.m_entity, new TransitionHandler<T>(this.EventTransitionActivate));
				}
				this.m_currentState = nextState;
				if (this.m_currentState != null)
				{
					this.m_currentState.OnStateEnter(this.m_entity);
					this.m_currentState.SubscribeTransitions(this.m_entity, new TransitionHandler<T>(this.EventTransitionActivate));
				}
				return true;
			}
			return false;
		}

		private void EventTransitionActivate(EventTransition<T> transition)
		{
			this.TransitionToState(transition.DestinationState);
		}

		private T m_entity;

		private BehaviourGraph<T> m_behaviour;

		private BehaviourState<T> m_currentState;

		private bool needFirstUpdate = true;
	}
}
