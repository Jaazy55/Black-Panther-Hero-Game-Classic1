using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PanelGoes : MonoBehaviour {
    public Text LoadingText;
    public string NextSceneName;
    // Use this for initialization
    void Start () {
        StartCoroutine(LoadScene());

    }

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
}
