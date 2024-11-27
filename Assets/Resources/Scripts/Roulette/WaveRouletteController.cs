using System;
using UnityEngine;

namespace Roulette
{
	public class WaveRouletteController : RouletteController
	{
		public static bool HasInstance
		{
			get
			{
				return WaveRouletteController.instance != null;
			}
		}

		public static WaveRouletteController Instance
		{
			get
			{
				return WaveRouletteController.instance;
			}
		}

		public static void Clear()
		{
			if (PlayerPrefs.HasKey("ChipsWave"))
			{
				PlayerPrefs.DeleteKey("ChipsWave");
			}
			if (PlayerPrefs.HasKey("ChipsCheetWave"))
			{
				PlayerPrefs.DeleteKey("ChipsCheetWave");
			}
		}

		protected override string ChipsKey
		{
			get
			{
				return "ChipsWave";
			}
		}

		protected override string CheetKey
		{
			get
			{
				return "ChipsCheetWave";
			}
		}

		protected override void Awake()
		{
			if (WaveRouletteController.instance == null)
			{
				WaveRouletteController.instance = this;
				base.Awake();
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
		}

		protected override void OnDestroy()
		{
			if (WaveRouletteController.instance == this)
			{
				base.OnDestroy();
				WaveRouletteController.instance = null;
			}
		}

		private const string m_ChipsKey = "ChipsWave";

		private const string m_CheetKey = "ChipsCheetWave";

		private static WaveRouletteController instance;
	}
}
