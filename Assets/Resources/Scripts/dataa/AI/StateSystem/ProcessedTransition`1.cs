using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.AI.StateSystem
{
	[Serializable]
	public class ProcessedTransition<T> : ProcessedTransition, IProcessedTransition, ISerializationCallbackReceiver
	{
		public ProcessedTransition()
		{
		}

		public ProcessedTransition(ProcessedTransition processed)
		{
			this.m_Decisions = processed.m_Decisions;
			this.m_TrueState = processed.m_TrueState;
			this.m_FalseState = processed.m_FalseState;
			this.TruePortName = processed.TruePortName;
			this.FalsePortName = processed.FalsePortName;
		}

		public ProcessedDecision<T>[] Decisions
		{
			get
			{
				if (this.generic_Decisions == null)
				{
					this.InitDecisions();
				}
				return this.generic_Decisions;
			}
			set
			{
				this.generic_Decisions = value;
			}
		}

		public BehaviourState<T> TrueState
		{
			get
			{
				if (!this.m_initTrue)
				{
					this.InitTrueState();
				}
				return this.generic_TrueState;
			}
			set
			{
				this.generic_TrueState = value;
				this.m_TrueState = value;
			}
		}

		public BehaviourState<T> FalseState
		{
			get
			{
				if (!this.m_initFalse)
				{
					this.InitFalseState();
				}
				return this.generic_FalseState;
			}
			set
			{
				this.generic_FalseState = value;
				this.m_FalseState = value;
			}
		}

		public bool CheckCondition(T entity)
		{
			bool result = true;
			for (int i = 0; i < this.Decisions.Length; i++)
			{
				ProcessedDecision<T> processedDecision = this.Decisions[i];
				if (!processedDecision.GetDecision(entity))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_Decisions = this.generic_Decisions;
			this.m_TrueState = this.generic_TrueState;
			this.m_FalseState = this.generic_FalseState;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.InitDecisions();
			this.InitTrueState();
			this.InitFalseState();
		}

		string IProcessedTransition.TruePortName
		{
			get
			{
				return this.TruePortName;
			}
			set
			{
				this.TruePortName = value;
			}
		}

		string IProcessedTransition.FalsePortName
		{
			get
			{
				return this.FalsePortName;
			}
			set
			{
				this.FalsePortName = value;
			}
		}

		IBehaviourState IProcessedTransition.TrueState
		{
			get
			{
				return this.TrueState;
			}
			set
			{
				this.TrueState = (value as BehaviourState<T>);
			}
		}

		IBehaviourState IProcessedTransition.FalseState
		{
			get
			{
				return this.FalseState;
			}
			set
			{
				this.FalseState = (value as BehaviourState<T>);
			}
		}

		private void InitDecisions()
		{
			if (this.m_Decisions != null)
			{
				List<ProcessedDecision<T>> list = new List<ProcessedDecision<T>>();
				foreach (ProcessedDecision processedDecision in this.m_Decisions)
				{
					if (processedDecision is ProcessedDecision<T>)
					{
						list.Add(processedDecision as ProcessedDecision<T>);
					}
				}
				this.generic_Decisions = list.ToArray();
			}
			else
			{
				this.generic_Decisions = new ProcessedDecision<T>[0];
			}
		}

		private void InitTrueState()
		{
			if (this.m_TrueState != null)
			{
				this.generic_TrueState = (this.m_TrueState as BehaviourState<T>);
			}
			else
			{
				this.generic_TrueState = null;
			}
			this.m_initTrue = true;
		}

		private void InitFalseState()
		{
			if (this.m_FalseState != null)
			{
				this.generic_FalseState = (this.m_FalseState as BehaviourState<T>);
			}
			else
			{
				this.generic_FalseState = null;
			}
			this.m_initFalse = true;
		}

		private ProcessedDecision<T>[] generic_Decisions;

		private BehaviourState<T> generic_TrueState;

		private bool m_initTrue;

		private BehaviourState<T> generic_FalseState;

		private bool m_initFalse;
	}
}
