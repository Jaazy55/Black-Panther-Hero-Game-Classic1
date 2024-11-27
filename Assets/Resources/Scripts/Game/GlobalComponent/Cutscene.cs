using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	public abstract class Cutscene : MonoBehaviour
	{
		public virtual void Init(CutsceneManager manager)
		{
			this.mainManager = manager;
		}

		public virtual void StartScene()
		{
			this.IsPlaying = true;
		}

		public virtual void EndScene(bool isCheck = true)
		{
			this.IsPlaying = false;
			if (isCheck)
			{
				this.mainManager.CheckFrame(this);
			}
		}

		public string Name;

		public bool IsMainAction;

		public bool IsPlaying;

		protected CutsceneManager mainManager;
	}
}
