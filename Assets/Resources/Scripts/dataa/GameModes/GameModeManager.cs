using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Naxeex.GameModes
{
	public class GameModeManager : MonoBehaviour, IGameModeManager
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction OnActivate;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction OnDeactivate;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction OnRestart;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction OnFinal;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UnityAction<GameMode> OnChangeGameMode;

		public GameMode CurrentMod
		{
			get
			{
				return this.m_CurrentGameMode;
			}
			set
			{
				if (this.m_CurrentGameMode != value)
				{
					this.UpdateRuleInterpreters(this.m_CurrentGameMode, value);
					this.m_CurrentGameMode = value;
					if (this.OnChangeGameMode != null)
					{
						this.OnChangeGameMode(value);
					}
				}
			}
		}

		public bool IsActivated
		{
			get
			{
				return this.m_activated;
			}
		}

		public void Activate()
		{
			if (!this.m_activated)
			{
				this.m_activated = true;
				this.BeginGame();
				if (this.OnActivate != null)
				{
					this.OnActivate();
				}
			}
		}

		public void Deactivate()
		{
			if (this.m_activated)
			{
				this.m_activated = false;
				this.EndGame();
				if (this.OnDeactivate != null)
				{
					this.OnDeactivate();
				}
			}
		}

		public void Restart()
		{
			if (!this.m_activated)
			{
				this.Activate();
			}
			else
			{
				this.RestartGame();
			}
			if (this.OnRestart != null)
			{
				this.OnRestart();
			}
		}

		public void Final()
		{
			if (this.m_activated && this.OnFinal != null)
			{
				this.OnFinal();
			}
		}

		protected void BeginGame()
		{
			foreach (RuleInterpreter ruleInterpreter in this.ValidateRuleInterpreters)
			{
				ruleInterpreter.RuleBegin();
			}
		}

		protected void EndGame()
		{
			foreach (RuleInterpreter ruleInterpreter in this.ValidateRuleInterpreters)
			{
				ruleInterpreter.RuleEnd();
			}
		}

		protected void RestartGame()
		{
			foreach (RuleInterpreter ruleInterpreter in this.ValidateRuleInterpreters)
			{
				ruleInterpreter.RuleRestart();
			}
		}

		protected void ProcessGame()
		{
			foreach (RuleInterpreter ruleInterpreter in this.ValidateRuleInterpreters)
			{
				ruleInterpreter.RuleProcess();
			}
		}

		protected void UpdateRuleInterpreters(GameMode old, GameMode current)
		{
			List<RuleInterpreter> list = new List<RuleInterpreter>();
			if (this.m_activated)
			{
				list.AddRange(this.ValidateRuleInterpreters);
			}
			this.ValidateRuleInterpreters.Clear();
			if (current != null)
			{
				Rule[] rules = current.Rules;
				if (rules != null)
				{
					foreach (Rule rule in rules)
					{
						RuleInterpreter[] ruleInterpreters = rule.RuleInterpreters;
						if (ruleInterpreters != null)
						{
							foreach (RuleInterpreter item in ruleInterpreters)
							{
								if (!this.ValidateRuleInterpreters.Contains(item))
								{
									this.ValidateRuleInterpreters.Add(item);
								}
							}
						}
					}
				}
			}
			if (this.m_activated)
			{
				foreach (RuleInterpreter ruleInterpreter in this.ValidateRuleInterpreters)
				{
					if (!list.Contains(ruleInterpreter))
					{
						ruleInterpreter.RuleBegin();
					}
				}
				foreach (RuleInterpreter ruleInterpreter2 in list)
				{
					if (!this.ValidateRuleInterpreters.Contains(ruleInterpreter2))
					{
						ruleInterpreter2.RuleEnd();
					}
				}
			}
		}

		protected void Awake()
		{
			Manager.Include(this);
			if (this.m_VariableGameMode != null)
			{
				this.m_VariableGameMode.ChangeGameMode += this.ChangeGameMOde;
				this.CurrentMod = this.m_VariableGameMode.GetGameMode();
			}
		}

		protected void OnDestroy()
		{
			if (this.m_activated)
			{
				this.Deactivate();
			}
			Manager.Exclude(this);
		}

		protected void Update()
		{
			if (this.m_activated && this.CurrentMod != null)
			{
				this.ProcessGame();
			}
		}

		private void ChangeGameMOde(GameMode gameMode)
		{
			this.CurrentMod = gameMode;
		}

		[SerializeField]
		private VariableGameModeGetter m_VariableGameMode;

		private List<RuleInterpreter> ValidateRuleInterpreters = new List<RuleInterpreter>();

		private bool m_activated;

		private GameMode m_CurrentGameMode;
	}
}
