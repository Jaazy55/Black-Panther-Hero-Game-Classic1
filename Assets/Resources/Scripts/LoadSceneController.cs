using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneController : MonoBehaviour
{
	public void Load()
	{
		base.Invoke("DelayLoad", this.Delay);
	}

	private void DelayLoad()
	{
		if (this.Loader)
		{
			this.Loader.LoadScene(this.SceneToLoad.ToString());
		}
		else
		{
			Thread.Sleep(500);
			if (this.MenuState != Constants.MenuState.None)
			{
				MApplication.MenuState = this.MenuState;
			}
			SceneManager.LoadScene(this.SceneToLoad.ToString());
		}
	}

	public Constants.Scenes SceneToLoad;

	public Constants.MenuState MenuState;

	public AsyncSceneLoader Loader;

	public float Delay;
}
