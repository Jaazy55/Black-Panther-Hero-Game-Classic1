using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GamePlay : MonoBehaviour {
	public GameObject Splash;
	public GameObject Consent;
	// Use this for initialization
	void Start()
	{
		//PlayerPrefs.DeleteAll();
		//if (PlayerPrefs.GetInt("firsttime") == 1)
		//{
		//	Consent.SetActive(true);

		//}
		//else
		//{
		//	loadscene();
		//	Consent.SetActive(false);
		//}
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void loadscene()
    {
		Splash.SetActive(true);
		Invoke("Sceneloade", 8f);
    }
	public void Sceneloade()
	{

        SceneManager.LoadScene("Menu");
	}

}
