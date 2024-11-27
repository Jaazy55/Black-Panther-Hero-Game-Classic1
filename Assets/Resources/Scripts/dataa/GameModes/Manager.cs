using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Events;

namespace Naxeex.GameModes
{
	public static class Manager
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event UnityAction OnActivate;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event UnityAction OnDeactivate;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event UnityAction OnRestart;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event UnityAction OnFinal;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event UnityAction<GameMode> OnChangeGameMode;

		public static void Include(IGameModeManager manager)
		{
			bool flag = true;
			if (Manager.m_currentManager != null)
			{
				flag = false;
				IGameModeManager currentManager = Manager.m_currentManager;
				if (Manager._003C_003Ef__mg_0024cache0 == null)
				{
					Manager._003C_003Ef__mg_0024cache0 = new UnityAction(Manager.OnDeactivateInvoke);
				}
				currentManager.OnDeactivate -= Manager._003C_003Ef__mg_0024cache0;
				IGameModeManager currentManager2 = Manager.m_currentManager;
				if (Manager._003C_003Ef__mg_0024cache1 == null)
				{
					Manager._003C_003Ef__mg_0024cache1 = new UnityAction(Manager.OnActivateInvoke);
				}
				currentManager2.OnActivate -= Manager._003C_003Ef__mg_0024cache1;
				IGameModeManager currentManager3 = Manager.m_currentManager;
				if (Manager._003C_003Ef__mg_0024cache2 == null)
				{
					Manager._003C_003Ef__mg_0024cache2 = new UnityAction(Manager.OnRestartInvoke);
				}
				currentManager3.OnRestart -= Manager._003C_003Ef__mg_0024cache2;
				IGameModeManager currentManager4 = Manager.m_currentManager;
				if (Manager._003C_003Ef__mg_0024cache3 == null)
				{
					Manager._003C_003Ef__mg_0024cache3 = new UnityAction(Manager.OnFinalInvoke);
				}
				currentManager4.OnFinal -= Manager._003C_003Ef__mg_0024cache3;
				Manager.m_currentManager.OnChangeGameMode -= Manager.OnChangeGameMode;
				Manager.m_activated = Manager.m_currentManager.IsActivated;
				Manager.m_currentGameMode = Manager.m_currentManager.CurrentMod;
			}
			Manager.m_currentManager = manager;
			if (Manager.m_currentManager != null)
			{
				if (flag)
				{
					IGameModeManager currentManager5 = Manager.m_currentManager;
					if (Manager._003C_003Ef__mg_0024cache4 == null)
					{
						Manager._003C_003Ef__mg_0024cache4 = new UnityAction(Manager.OnActivateInvoke);
					}
					currentManager5.OnActivate += Manager._003C_003Ef__mg_0024cache4;
				}
				if (Manager.m_activated)
				{
					Manager.m_currentManager.Activate();
				}
				if (!flag)
				{
					IGameModeManager currentManager6 = Manager.m_currentManager;
					if (Manager._003C_003Ef__mg_0024cache5 == null)
					{
						Manager._003C_003Ef__mg_0024cache5 = new UnityAction(Manager.OnActivateInvoke);
					}
					currentManager6.OnActivate += Manager._003C_003Ef__mg_0024cache5;
				}
				if (Manager.m_currentManager != null)
				{
					IGameModeManager currentManager7 = Manager.m_currentManager;
					if (Manager._003C_003Ef__mg_0024cache6 == null)
					{
						Manager._003C_003Ef__mg_0024cache6 = new UnityAction<GameMode>(Manager.OnChangeGameModeInvoke);
					}
					currentManager7.OnChangeGameMode += Manager._003C_003Ef__mg_0024cache6;
					IGameModeManager currentManager8 = Manager.m_currentManager;
					if (Manager._003C_003Ef__mg_0024cache7 == null)
					{
						Manager._003C_003Ef__mg_0024cache7 = new UnityAction(Manager.OnDeactivateInvoke);
					}
					currentManager8.OnDeactivate += Manager._003C_003Ef__mg_0024cache7;
					IGameModeManager currentManager9 = Manager.m_currentManager;
					if (Manager._003C_003Ef__mg_0024cache8 == null)
					{
						Manager._003C_003Ef__mg_0024cache8 = new UnityAction(Manager.OnRestartInvoke);
					}
					currentManager9.OnRestart += Manager._003C_003Ef__mg_0024cache8;
					IGameModeManager currentManager10 = Manager.m_currentManager;
					if (Manager._003C_003Ef__mg_0024cache9 == null)
					{
						Manager._003C_003Ef__mg_0024cache9 = new UnityAction(Manager.OnFinalInvoke);
					}
					currentManager10.OnFinal += Manager._003C_003Ef__mg_0024cache9;
					if (Manager.m_currentGameMode != Manager.m_currentManager.CurrentMod)
					{
						Manager.OnChangeGameModeInvoke(Manager.m_currentManager.CurrentMod);
					}
				}
			}
		}

		public static void Exclude(IGameModeManager manager)
		{
			if (manager != Manager.m_currentManager || manager == null)
			{
				return;
			}
			IGameModeManager currentManager = Manager.m_currentManager;
			if (Manager._003C_003Ef__mg_0024cacheA == null)
			{
				Manager._003C_003Ef__mg_0024cacheA = new UnityAction(Manager.OnDeactivateInvoke);
			}
			currentManager.OnDeactivate -= Manager._003C_003Ef__mg_0024cacheA;
			IGameModeManager currentManager2 = Manager.m_currentManager;
			if (Manager._003C_003Ef__mg_0024cacheB == null)
			{
				Manager._003C_003Ef__mg_0024cacheB = new UnityAction(Manager.OnActivateInvoke);
			}
			currentManager2.OnActivate -= Manager._003C_003Ef__mg_0024cacheB;
			IGameModeManager currentManager3 = Manager.m_currentManager;
			if (Manager._003C_003Ef__mg_0024cacheC == null)
			{
				Manager._003C_003Ef__mg_0024cacheC = new UnityAction(Manager.OnRestartInvoke);
			}
			currentManager3.OnRestart -= Manager._003C_003Ef__mg_0024cacheC;
			IGameModeManager currentManager4 = Manager.m_currentManager;
			if (Manager._003C_003Ef__mg_0024cacheD == null)
			{
				Manager._003C_003Ef__mg_0024cacheD = new UnityAction(Manager.OnFinalInvoke);
			}
			currentManager4.OnFinal -= Manager._003C_003Ef__mg_0024cacheD;
			Manager.m_currentManager.OnChangeGameMode -= Manager.OnChangeGameMode;
			Manager.m_currentManager = null;
		}

		public static GameMode CurrentMod
		{
			get
			{
				return (Manager.m_currentManager == null) ? null : Manager.m_currentManager.CurrentMod;
			}
			set
			{
				Manager.m_currentGameMode = value;
				if (Manager.m_currentManager != null)
				{
					Manager.m_currentManager.CurrentMod = value;
				}
			}
		}

		public static bool IsActivated
		{
			get
			{
				return (Manager.m_currentManager == null) ? Manager.m_activated : Manager.m_currentManager.IsActivated;
			}
		}

		public static void Activate()
		{
			Manager.m_activated = true;
			if (Manager.m_currentManager != null && !Manager.m_currentManager.IsActivated)
			{
				Manager.m_currentManager.Activate();
			}
		}

		public static void Deactivate()
		{
			Manager.m_activated = false;
			if (Manager.m_currentManager != null && Manager.m_currentManager.IsActivated)
			{
				Manager.m_currentManager.Deactivate();
			}
		}

		public static void Restart()
		{
			if (Manager.m_currentManager != null && Manager.m_currentManager.IsActivated)
			{
				Manager.m_currentManager.Restart();
			}
		}

		public static void Final()
		{
			if (Manager.m_currentManager != null && Manager.m_currentManager.IsActivated)
			{
				Manager.m_currentManager.Final();
			}
		}

		private static void OnActivateInvoke()
		{
			Manager.m_activated = true;
			if (Manager.OnActivate != null)
			{
				Manager.OnActivate();
			}
		}

		private static void OnDeactivateInvoke()
		{
			Manager.m_activated = false;
			if (Manager.OnDeactivate != null)
			{
				Manager.OnDeactivate();
			}
		}

		private static void OnRestartInvoke()
		{
			if (Manager.OnRestart != null)
			{
				Manager.OnRestart();
			}
		}

		private static void OnFinalInvoke()
		{
			if (Manager.OnFinal != null)
			{
				Manager.OnFinal();
			}
		}

		private static void OnChangeGameModeInvoke(GameMode gameMode)
		{
			Manager.m_currentGameMode = gameMode;
			if (Manager.OnChangeGameMode != null)
			{
				Manager.OnChangeGameMode(gameMode);
			}
		}

		private static IGameModeManager m_currentManager;

		private static bool m_activated;

		private static GameMode m_currentGameMode;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache3;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache4;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache5;

		[CompilerGenerated]
		private static UnityAction<GameMode> _003C_003Ef__mg_0024cache6;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache7;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache8;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache9;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cacheA;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cacheB;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cacheC;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cacheD;
	}
}
