using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

public class Cooldown : MonoBehaviour {

	public Image cooldown;
	public bool coolingDown;
	public float waitTime = 1.25f;
	public GameObject PanelIsLoading,InstersitialAdcall;
	void OnEnable()
	{
		Time.timeScale = 1;
		cooldown.fillAmount = 0;
		coolingDown = true;
	
	}

	// Update is called once per frame
	void Update () 
	{
		

		if (coolingDown == true)
		{
			//Reduce fill amount over 30 seconds
			cooldown.fillAmount += 0.25f/waitTime * Time.deltaTime;
			if (cooldown.fillAmount == 1) 
			{
				PanelIsLoading.SetActive (false);
				InstersitialAdcall.SetActive (true);
				coolingDown = false;
				cooldown.fillAmount = 0;

			}
		}

	}
	void PanelOfIntersitialCall()
	{
		

	}
}