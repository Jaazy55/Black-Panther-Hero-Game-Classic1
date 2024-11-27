using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonController : MonoBehaviour
{
	public void Init(LevelButtonController.LevelSelected levelSelected, LevelInfo levelInfo, int level)
	{
		this._level = level;
		this._levelSelected = levelSelected;
		this._levelInfo = levelInfo;
		if (this._levelInfo.IsAvailable)
		{
			int num = this._levelInfo.StarsCount;
			foreach (GameObject gameObject in this.Stars)
			{
				if (--num < 0)
				{
					break;
				}
				gameObject.SetActive(true);
			}
		}
		else
		{
			this.Button.interactable = false;
		}
		this.LevelText.text = level.ToString();
	}

	public void SelectLevel()
	{
		if (this._levelSelected != null)
		{
			this._levelSelected(this._level);
		}
	}

	public GameObject[] Stars;

	public Text LevelText;

	public Button Button;

	private LevelButtonController.LevelSelected _levelSelected;

	private int _level;

	private LevelInfo _levelInfo;

	public delegate void LevelSelected(int level);
}
