using System;
using Game.GlobalComponent.Quality;
using Game.UI;
using Naxeex.Attribute;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Load Scene Interpeter", menuName = "Game Modes/Interpeters/Load Scene Interpeter")]
	public class LoadSceneInterpreter : RuleInterpreter
	{
		public Scene LoadableScene
		{
			get
			{
				if (!this.m_initLoadScene)
				{
					this.m_loadableScene = SceneManager.GetSceneByName(this.m_LoadableScene);
					this.m_initLoadScene = true;
				}
				return this.m_loadableScene;
			}
		}

		public override void RuleBegin()
		{
			base.RuleBegin();
			if (this.NeedLoadScene())
			{
				this.asyncLoadSceneOperation = SceneManager.LoadSceneAsync(this.m_LoadableScene, LoadSceneMode.Additive);
				WaitingPanelController.Instance.StartWaiting(new Action(this.OnLoaded), new Func<bool>(this.SceneIsLoaded));
			}
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
			if (this.SceneIsLoaded() && SceneManager.GetSceneByName(this.m_LoadableScene).IsValid())
			{
				this.asyncUnloadSceneOperation = SceneManager.UnloadSceneAsync(this.m_LoadableScene);
			}
		}

		protected virtual void OnEnable()
		{
			this.m_loaded = SceneManager.GetSceneByName(this.m_LoadableScene).isLoaded;
			SceneManager.sceneLoaded += this.SceneLoadedHandler;
			SceneManager.sceneUnloaded += this.SceneUnloadedHandler;
		}

		protected virtual void OnDisable()
		{
			SceneManager.sceneLoaded -= this.SceneLoadedHandler;
			SceneManager.sceneUnloaded -= this.SceneUnloadedHandler;
		}

		protected void ContinueGame()
		{
			if (!WaitingPanelController.Instance.IsActivate)
			{
				UIGame.Instance.PanelManager.OpenPanel(UIGame.Instance.GamePanel);
			}
		}

		protected virtual void OnLoaded()
		{
			GameplayUtils.ResumeGame();
			this.ContinueGame();
		}

		private bool NeedLoadScene()
		{
			this.m_loaded = SceneManager.GetSceneByName(this.m_LoadableScene).isLoaded;
			if (this.asyncLoadSceneOperation != null && this.asyncLoadSceneOperation.isDone && !this.m_loaded)
			{
				this.asyncLoadSceneOperation = null;
			}
			return this.asyncLoadSceneOperation == null;
		}

		private bool SceneIsLoaded()
		{
			return this.m_loaded;
		}

		private void SceneLoadedHandler(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == this.m_LoadableScene)
			{
				this.m_loaded = true;
				this.OnLoaded();
			}
		}

		private void SceneUnloadedHandler(Scene scene)
		{
			if (scene.name == this.m_LoadableScene)
			{
				this.m_loaded = false;
			}
		}

		[SerializeField]
		[Scene]
		private string m_LoadableScene;

		private AsyncOperation asyncLoadSceneOperation;

		private AsyncOperation asyncUnloadSceneOperation;

		private bool m_loaded;

		private bool m_initLoadScene;

		private Scene m_loadableScene;
	}
}
