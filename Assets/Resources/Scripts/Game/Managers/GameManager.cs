using System;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Managers
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance
		{
			get
			{
				if (GameManager.instance == null)
				{
					GameManager.instance = UnityEngine.Object.FindObjectOfType<GameManager>();
				}
				return GameManager.instance;
			}
		}

		private void Awake()
		{
			
			if (GameManager.instance == null)
			{
				GameManager.instance = this;
			}

		}

		public static bool ShowDebugs;

		public bool IsTransformersGame;

		public ControlsType[] TransformationTypes;

		private static GameManager instance;

		void Start()
		{
			
		}
	}
}
