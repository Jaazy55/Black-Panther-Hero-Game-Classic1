using System;
using Game.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
	private void Awake()
	{
		LevelInfo levelInfo = LevelsProfile.GetLevelInfo(1);
		if (!levelInfo.IsAvailable)
		{
			levelInfo.IsAvailable = true;
			LevelsProfile.SetLevelInfo(1, levelInfo);
		}
		if (this.LevelButtonPrefab != null)
		{
			for (int i = 0; i < this.LevelsCount; i++)
			{
				LevelButtonController levelButtonController = UnityEngine.Object.Instantiate<LevelButtonController>(this.LevelButtonPrefab);
				levelButtonController.transform.SetParent(base.transform);
				levelButtonController.transform.localScale = new Vector3(1f, 1f, 1f);
				LevelInfo levelInfo2 = LevelsProfile.GetLevelInfo(i + 1);
				levelButtonController.Init(new LevelButtonController.LevelSelected(this.SelectLevel), levelInfo2, i + 1);
				levelButtonController.gameObject.SetActive(true);
			}
		}
	}

	public void SelectLevel(int level)
	{
		if (GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log("Selected level = " + level);
		}
		MApplication.CurrentLevel = level;
		this.MenuPanelManager.OpenPanel(this.LoadingPanel);
//		AdsManager.ShowFullscreenAd();
		base.Invoke("LoadLevel", 0.5f);
	}

	private void LoadLevel()
	{
		SceneManager.LoadScene(Constants.Scenes.Game.ToString());
	}

	public MenuPanelManager MenuPanelManager;

	public Animator LoadingPanel;

	public LevelButtonController LevelButtonPrefab;

	public int LevelsCount;
}
