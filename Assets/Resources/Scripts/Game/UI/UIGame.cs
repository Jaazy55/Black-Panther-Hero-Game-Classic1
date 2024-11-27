using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    
	public class UIGame : MonoBehaviour
	{
        
		public static bool IsCompleted
		{
			get
			{
				return UIGame._isCompleted;
			}
		}

		private void Awake()
		{
			GameplayUtils.ResumeGame();
			UIGame._isCompleted = false;
			UIGame.Instance = this;
           
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(this.ScreenshotKey))
			{
				GameplayUtils.PerformScreenshot((!this.HDScreenshots) ? 1 : 2);
			}
			if (UnityEngine.Input.GetKeyDown(this.PauseKey))
			{
				UnityEngine.Debug.Break();
			}
		}

		public void SetProgress(float value)
		{
			if (this.ProgeressBar != null)
			{
				this.ProgeressBar.value = value;
			}
		}

		public void SetHP(float value)
		{
			if (this.HpBar != null)
			{
				this.HpBar.value = value;
			}
		}

		public void SetTime(DateTime dateTime)
		{
			if (this.Timer != null)
			{
				this.Timer.text = dateTime.ToString("mm:ss.f");
			}
		}

		public void SetScore(int score)
		{
			if (this.Score != null)
			{
				this.Score.text = score.ToString();
			}
		}

		public void Complete(bool won, int starsCount)
		{
			if (UIGame._isCompleted)
			{
				return;
			}
			UIGame._isCompleted = true;
			GameplayUtils.PauseGame();
			if (won)
			{
				this.PanelManager.OpenPanel(this.WinPanel);
				AudioSource component = this.WinPanel.gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.Play();
				}
				WinnerStarsController component2 = this.WinPanel.gameObject.GetComponent<WinnerStarsController>();
				if (component2 != null)
				{
					component2.SetStarts(starsCount);
				}
				LevelInfo levelInfo = LevelsProfile.GetLevelInfo(MApplication.CurrentLevel);
				if (levelInfo.StarsCount < starsCount)
				{
					levelInfo.StarsCount = starsCount;
					LevelsProfile.SetLevelInfo(MApplication.CurrentLevel, levelInfo);
				}
				LevelInfo levelInfo2 = LevelsProfile.GetLevelInfo(MApplication.CurrentLevel + 1);
				if (!levelInfo2.IsAvailable)
				{
					LevelsProfile.SetLevelInfo(MApplication.CurrentLevel + 1, new LevelInfo
					{
						IsAvailable = true
					});
				}
			}
			else
			{
				this.PanelManager.OpenPanel(this.LoosePanel);
				AudioSource component3 = this.LoosePanel.gameObject.GetComponent<AudioSource>();
				if (component3 != null)
				{
					component3.Play();
				}
			}
		}

		public void Pause()
		{
			if (this.OnPausePanelOpen != null)
			{
				this.OnPausePanelOpen();
                
               

			}
			GameplayUtils.PauseGame();
		}

		public void Resume()
		{
			GameplayUtils.ResumeGame();
		}

		public void Loose()
		{
			this.Complete(false, 0);
		}

		public void Win()
		{
			this.Complete(true, 2);
		}

		public void ExitInMenu()
		{
			UniversalYesNoPanel.Instance.DisplayOffer("Exit game", "Are you sure?", delegate()
			{
				if (this.OnExitInMenu != null)
				{
					this.OnExitInMenu();
				}
				Thread.Sleep(500);
				SceneManager.LoadScene(Constants.Scenes.Menu.ToString());
			}, null, false);
		}

		public KeyCode ScreenshotKey = KeyCode.Keypad0;

		public KeyCode PauseKey = KeyCode.KeypadPeriod;

		public MenuPanelManager PanelManager;

		public GameObject achievmentUI;

		public Animator GamePanel;

		public Animator PausePanel;

		public Animator LoosePanel;

		public Animator WinPanel;

		public Text Timer;

		public Text Score;

		public Slider ProgeressBar;

		public Slider HpBar;

		public bool HDScreenshots;

		public Action OnPausePanelOpen;

		public Action OnExitInMenu;

		private static bool _isCompleted;

		public static bool DrawOnGUI = true;

		public static UIGame Instance;
	}
}
