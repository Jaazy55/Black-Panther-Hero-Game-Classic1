using System;
using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class CutsceneManager : MonoBehaviour
	{
		public bool Inited
		{
			get
			{
				return this.init;
			}
		}

		public int CurrentIndex()
		{
			return this.sceneIndex;
		}

		public void Init(CutsceneManager.Callback cb, CutsceneManager.Callback forcedCallback)
		{
			this.player = PlayerInteractionsManager.Instance.Player;
			CutscenePanel.Instance.Open();
			this.sceneIndex = 0;
			this.callback = cb;
			this.fCallback = forcedCallback;
			this.init = true;
			this.StartMove();
			this.player.DiedEvent += this.StopAllFrame;
		}

		public void CheckFrame(Cutscene scene)
		{
			if (scene.IsMainAction && this.Scenes[this.sceneIndex].EndMainAction)
			{
				this.StopFrame();
			}
			bool flag = true;
			foreach (Cutscene cutscene in this.Scenes[this.sceneIndex].Scenes)
			{
				if (cutscene.IsPlaying)
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.StopFrame();
			}
		}

		public void StartMove()
		{
			foreach (Cutscene cutscene in this.Scenes[this.sceneIndex].Scenes)
			{
				cutscene.Init(this);
				cutscene.StartScene();
			}
			this.timer = this.Scenes[this.sceneIndex].Time;
		}

		private void StopScene()
		{
			if (this.init)
			{
				CutscenePanel.Instance.Close();
				if (this.callback != null)
				{
					this.callback();
				}
			}
			this.init = false;
		}

		private void StopAllFrame()
		{
			if (this.init)
			{
				CutscenePanel.Instance.Close();
			}
			this.init = false;
			if (this.fCallback != null)
			{
				this.fCallback();
			}
			for (int i = 0; i < this.Scenes.Length; i++)
			{
				foreach (Cutscene cutscene in this.Scenes[i].Scenes)
				{
					if (cutscene.IsPlaying)
					{
						cutscene.EndScene(false);
					}
				}
			}
		}

		private void Update()
		{
			if (!this.init)
			{
				return;
			}
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.StopAllFrame();
			}
		}

		private void StopFrame()
		{
			foreach (Cutscene cutscene in this.Scenes[this.sceneIndex].Scenes)
			{
				if (cutscene.IsPlaying)
				{
					cutscene.EndScene(false);
				}
			}
			this.sceneIndex++;
			if (this.sceneIndex >= this.Scenes.Length)
			{
				this.sceneIndex--;
				this.StopScene();
			}
			else
			{
				this.StartMove();
			}
		}

		public CutsceneManager.Frame[] Scenes;

		[HideInInspector]
		public bool ForcedStop;

		private CutsceneManager.Callback callback;

		private CutsceneManager.Callback fCallback;

		private int sceneIndex;

		private bool init;

		private Animator prevPanel;

		private Player player;

		private float timer;

		public delegate void Callback();

		[Serializable]
		public class Frame
		{
			public string Name;

			public Cutscene[] Scenes;

			public float Time = 10f;

			public bool EndMainAction;
		}
	}
}
