using System;
using UnityEngine;

namespace Roulette
{
	public class SurvivalRouletteController : RouletteController
	{
		public static bool HasInstance
		{
			get
			{
				return SurvivalRouletteController.instance != null;
			}
		}

		public static SurvivalRouletteController Instance
		{
			get
			{
				return SurvivalRouletteController.instance;
			}
		}

		public static void Clear()
		{
			if (PlayerPrefs.HasKey("ChipsSurvival"))
			{
				PlayerPrefs.DeleteKey("ChipsSurvival");
			}
			if (PlayerPrefs.HasKey("ChipsCheetSurvival"))
			{
				PlayerPrefs.DeleteKey("ChipsCheetSurvival");
			}
		}

		protected override string ChipsKey
		{
			get
			{
				return "ChipsSurvival";
			}
		}

		protected override string CheetKey
		{
			get
			{
				return "ChipsCheetSurvival";
			}
		}

		protected override void Awake()
		{
			if (SurvivalRouletteController.instance == null)
			{
				SurvivalRouletteController.instance = this;
				base.Awake();
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
		}

		protected override void OnDestroy()
		{
			if (SurvivalRouletteController.instance == this)
			{
				base.OnDestroy();
				SurvivalRouletteController.instance = null;
			}
		}

		private const string m_ChipsKey = "ChipsSurvival";

		private const string m_CheetKey = "ChipsCheetSurvival";

		private static SurvivalRouletteController instance;
	}
}
