

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneLoade : MonoBehaviour
{
    public GameObject LoadingPanel, Splashpanel;
    public Text LoadingText;
    public string NextSceneName;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            Splashpanel.SetActive(true);
            LoadingPanel.SetActive(false);
            PlayerPrefs.SetInt("FirstTime", 1);
        }
        else
        {
            StartCoroutine(LoadScene());

        }
    }

    //    this.RequestBanner();
    //}

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Menu");
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            float ff = asyncOperation.progress * 100;
            LoadingText.text = ff.ToString("00") + "%";

            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    public void Accept
        ()
    {
        StartCoroutine(LoadScene());
        LoadingPanel.SetActive(true);
        Splashpanel.SetActive(false);
    }
    public void Privacy()
    {
		Application.OpenURL("");
    }
}
