using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Roulette;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Naxeex.GameModes
{
	public class ArenaTutorial : MonoBehaviour
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ArenaTutorial.TutorialState> OnChangeState;

		public static ArenaTutorial.TutorialState State
		{
			get
			{
				return ArenaTutorial.m_State;
			}
			set
			{
				if (ArenaTutorial.m_State != value)
				{
					ArenaTutorial.m_State = value;
					if (ArenaTutorial.OnChangeState != null)
					{
						ArenaTutorial.OnChangeState(ArenaTutorial.m_State);
					}
					ArenaTutorial.StateHandler(ArenaTutorial.m_State);
				}
			}
		}

		public static bool IsCompleted
		{
			get
			{
				if (ArenaTutorial.m_ArenaIsCompited == null)
				{
					ArenaTutorial.m_ArenaIsCompited = new bool?(PlayerPrefs.HasKey("ArenaTutorial") && PlayerPrefs.GetInt("ArenaTutorial") > 0);
				}
				return ArenaTutorial.m_ArenaIsCompited.Value;
			}
			set
			{
				ArenaTutorial.m_ArenaIsCompited = new bool?(value);
				PlayerPrefs.SetInt("ArenaTutorial", (!value) ? 0 : 1);
			}
		}

		public static void ClearAll()
		{
			PlayerPrefs.DeleteKey("ArenaTutorial");
		}

		private static void StateHandler(ArenaTutorial.TutorialState state)
		{
			switch (state)
			{
			case ArenaTutorial.TutorialState.ClickTutorial:
				ArenaTutorial.instance.m_FingerToTutorial.gameObject.SetActive(true);
				ArenaTutorial.instance.m_EnterTutorialButton.gameObject.SetActive(true);
				ArenaTutorial.instance.m_EnterStartButton.interactable = false;
				ArenaTutorial.instance.m_EnterBackButton.interactable = false;
				return;
			case ArenaTutorial.TutorialState.WinArena:
				ArenaTutorial.instance.m_FingerToRestart.gameObject.SetActive(true);
				ArenaTutorial.instance.m_ResultBackButton.interactable = false;
				return;
			case ArenaTutorial.TutorialState.RoulettleClick:
			{
				SurvivalRouletteController.Instance.Spins++;
				ArenaTutorial.SpinWasGiven = true;
				ArenaTutorial.instance.m_FingerToTutorial.gameObject.SetActive(false);
				ArenaTutorial.instance.m_FingerToRestart.gameObject.SetActive(false);
				ArenaTutorial.instance.m_FingerToBackInMenu.gameObject.SetActive(true);
				ArenaTutorial.instance.m_FingerToBackInGame.gameObject.SetActive(true);
				ArenaTutorial.instance.m_FingerToRoulette.gameObject.SetActive(true);
				ArenaTutorial.instance.m_ResultRestartButton.interactable = false;
				ArenaTutorial.instance.m_ResultBackButton.interactable = true;
				ArenaTutorial.instance.m_EnterTutorialButton.interactable = false;
				ArenaTutorial.instance.m_EnterBackButton.interactable = true;
				ArenaTutorial.instance.m_RouletteBackButton.interactable = false;
				RoulettePanel roulettePanel = ArenaTutorial.instance.roulettePanel;
				if (ArenaTutorial._003C_003Ef__mg_0024cache0 == null)
				{
					ArenaTutorial._003C_003Ef__mg_0024cache0 = new Action(ArenaTutorial.SpinHandler);
				}
				roulettePanel.OnSpin += ArenaTutorial._003C_003Ef__mg_0024cache0;
				UnityEvent onClick = ArenaTutorial.instance.m_RouletteBackButton.onClick;
				if (ArenaTutorial._003C_003Ef__mg_0024cache1 == null)
				{
					ArenaTutorial._003C_003Ef__mg_0024cache1 = new UnityAction(ArenaTutorial.ClickRouletteBack);
				}
				onClick.AddListener(ArenaTutorial._003C_003Ef__mg_0024cache1);
				return;
			}
			case ArenaTutorial.TutorialState.End:
				ArenaTutorial.IsCompleted = true;
				ArenaTutorial.SpinWasGiven = false;
				ArenaTutorial.State = ArenaTutorial.TutorialState.None;
				return;
			}
			if (ArenaTutorial.SpinWasGiven && SurvivalRouletteController.HasInstance && SurvivalRouletteController.Instance.Spins > 0)
			{
				SurvivalRouletteController.Instance.Spins--;
				ArenaTutorial.SpinWasGiven = false;
			}
			ArenaTutorial.instance.m_EnterTutorialButton.gameObject.SetActive(false);
			ArenaTutorial.instance.m_FingerToTutorial.gameObject.SetActive(false);
			ArenaTutorial.instance.m_FingerToRestart.gameObject.SetActive(false);
			ArenaTutorial.instance.m_FingerToBackInMenu.gameObject.SetActive(false);
			ArenaTutorial.instance.m_FingerToBackInGame.gameObject.SetActive(false);
			ArenaTutorial.instance.m_FingerToRoulette.gameObject.SetActive(false);
			ArenaTutorial.instance.m_FingerToBackRoulette.gameObject.SetActive(false);
			ArenaTutorial.instance.m_EnterStartButton.interactable = true;
			ArenaTutorial.instance.m_EnterBackButton.interactable = true;
			ArenaTutorial.instance.m_EnterTutorialButton.interactable = true;
			ArenaTutorial.instance.m_ResultRestartButton.interactable = true;
			ArenaTutorial.instance.m_ResultBackButton.interactable = true;
			ArenaTutorial.instance.m_RouletteBackButton.interactable = true;
			RoulettePanel roulettePanel2 = ArenaTutorial.instance.roulettePanel;
			if (ArenaTutorial._003C_003Ef__mg_0024cache2 == null)
			{
				ArenaTutorial._003C_003Ef__mg_0024cache2 = new Action(ArenaTutorial.SpinHandler);
			}
			roulettePanel2.OnSpin -= ArenaTutorial._003C_003Ef__mg_0024cache2;
			UnityEvent onClick2 = ArenaTutorial.instance.m_RouletteBackButton.onClick;
			if (ArenaTutorial._003C_003Ef__mg_0024cache3 == null)
			{
				ArenaTutorial._003C_003Ef__mg_0024cache3 = new UnityAction(ArenaTutorial.ClickRouletteBack);
			}
			onClick2.RemoveListener(ArenaTutorial._003C_003Ef__mg_0024cache3);
			ArenaTutorial.RouletteWas = false;
		}

		private void Awake()
		{
			if (ArenaTutorial.instance == null)
			{
				ArenaTutorial.instance = this;
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			if (ArenaTutorial.instance == this)
			{
				ArenaTutorial.State = ArenaTutorial.TutorialState.None;
				ArenaTutorial.instance = null;
			}
		}

		private void Start()
		{
			if (ArenaTutorial.SpinWasGiven && SurvivalRouletteController.HasInstance && SurvivalRouletteController.Instance.Spins > 0)
			{
				SurvivalRouletteController.Instance.Spins--;
				ArenaTutorial.SpinWasGiven = false;
			}
		}

		private static void SpinHandler()
		{
			ArenaTutorial.instance.m_RouletteBackButton.interactable = true;
			RoulettePanel roulettePanel = ArenaTutorial.instance.roulettePanel;
			if (ArenaTutorial._003C_003Ef__mg_0024cache4 == null)
			{
				ArenaTutorial._003C_003Ef__mg_0024cache4 = new Action(ArenaTutorial.SpinHandler);
			}
			roulettePanel.OnSpin -= ArenaTutorial._003C_003Ef__mg_0024cache4;
			ArenaTutorial.instance.m_FingerToBackRoulette.gameObject.SetActive(true);
			ArenaTutorial.RouletteWas = true;
		}

		private static void ClickRouletteBack()
		{
			if (ArenaTutorial.RouletteWas)
			{
				ArenaTutorial.State = ArenaTutorial.TutorialState.End;
				UnityEvent onClick = ArenaTutorial.instance.m_RouletteBackButton.onClick;
				if (ArenaTutorial._003C_003Ef__mg_0024cache5 == null)
				{
					ArenaTutorial._003C_003Ef__mg_0024cache5 = new UnityAction(ArenaTutorial.ClickRouletteBack);
				}
				onClick.RemoveListener(ArenaTutorial._003C_003Ef__mg_0024cache5);
				ArenaTutorial.RouletteWas = false;
			}
		}

		private const string CompletedKey = "ArenaTutorial";

		private static ArenaTutorial instance;

		private static bool? m_ArenaIsCompited;

		private static bool? m_RouletteIsComplited;

		private static ArenaTutorial.TutorialState m_State;

		[SerializeField]
		private GameObject m_FingerToTutorial;

		[SerializeField]
		private GameObject m_FingerToRestart;

		[SerializeField]
		private GameObject m_FingerToBackInMenu;

		[SerializeField]
		private GameObject m_FingerToBackInGame;

		[SerializeField]
		private GameObject m_FingerToRoulette;

		[SerializeField]
		private GameObject m_FingerToBackRoulette;

		[SerializeField]
		private Button m_EnterStartButton;

		[SerializeField]
		private Button m_EnterBackButton;

		[SerializeField]
		private Button m_EnterTutorialButton;

		[SerializeField]
		private Button m_ResultBackButton;

		[SerializeField]
		private Button m_ResultRestartButton;

		[SerializeField]
		private Button m_RouletteBackButton;

		[SerializeField]
		private RoulettePanel roulettePanel;

		private static bool SpinWasGiven;

		private static bool RouletteWas;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache3;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache4;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache5;

		public enum TutorialState
		{
			None,
			ClickTutorial,
			WinArena,
			RoulettleClick,
			End
		}
	}
}
