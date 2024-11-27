using System;
using UnityEngine;

public class MenuPanelManager : MonoBehaviour
{
	public void Start()
	{
		if (this.FirstOpen && this._currentPanelAnimator == null)
		{
			this.OpenPanel(this.FirstOpen);
		}
		else
		{
			this.OpenPanel(this._currentPanelAnimator);
		}
		// Firebase.Analytics.FirebaseAnalytics.LogEvent("strat");//Hussain

	}

	public void CloseCurrentPanel()
	{
		if (this._currentPanelAnimator != null)
		{
			if (this._currentPanelAnimator.isInitialized)
			{
				this._currentPanelAnimator.SetBool("Open", false);
			}
			this._currentPanelAnimator.gameObject.SetActive(false);
		}
		this._currentPanelAnimator = null;
	}

	public void OpenPanel(Animator panelAnimator)
	{
		if (this._currentPanelAnimator == panelAnimator)
		{
			return;
		}
		if (this._currentPanelAnimator != null)
		{
			if (this._currentPanelAnimator.isInitialized)
			{
				this._currentPanelAnimator.SetBool("Open", false);
			}
			this._currentPanelAnimator.gameObject.SetActive(false);
		}
		if (panelAnimator != null)
		{
			panelAnimator.gameObject.SetActive(true);
			if (panelAnimator.isInitialized)
			{
				panelAnimator.SetBool("Open", true);
			}
		}
		this._currentPanelAnimator = panelAnimator;
		AudioSource component = base.GetComponent<AudioSource>();
		if (component != null && component.isActiveAndEnabled)
		{
			component.Play();
		}
	}

	public void SwitchPanel(Animator switchPanel)
	{
		if (this._currentPanelAnimator == switchPanel)
		{
			if (this._currentPanelAnimator.isInitialized)
			{
				this._currentPanelAnimator.SetBool("Open", false);
			}
			this._currentPanelAnimator.gameObject.SetActive(false);
			this.FirstOpen.gameObject.SetActive(true);
			if (this.FirstOpen.isInitialized)
			{
				this.FirstOpen.SetBool("Open", false);
			}
			this._currentPanelAnimator = this.FirstOpen;
		}
		else if (this._currentPanelAnimator != switchPanel)
		{
			if (this._currentPanelAnimator.isInitialized)
			{
				this._currentPanelAnimator.SetBool("Open", false);
			}
			this._currentPanelAnimator.gameObject.SetActive(false);
			if (switchPanel.isInitialized)
			{
				switchPanel.SetBool("Open", false);
			}
			switchPanel.gameObject.SetActive(true);
			this._currentPanelAnimator = switchPanel;
		}
		AudioSource component = base.GetComponent<AudioSource>();
		if (component != null && component.isActiveAndEnabled)
		{
			component.Play();
		}
	}

	public Animator GetCurrentPanel()
	{
		return this._currentPanelAnimator;
	}

	public void ResetSaves()
	{
		BaseProfile.ClearBaseProfileWithoutSystemSettings();
	}

	public void ExitApplication()
	{
		Application.Quit();
	}


    public void MoreApps()
    {
		Application.OpenURL("");
	//	 Firebase.Analytics.FirebaseAnalytics.LogEvent("more");//

	}
	public void RateApps()
    {
		Application.OpenURL("");
	//	 Firebase.Analytics.FirebaseAnalytics.LogEvent("rate");//

	}
	public void CarrierStat()
    {
		
	//	 Firebase.Analytics.FirebaseAnalytics.LogEvent("Carrier_mode");//

	}
	public void ThrillMode()
    {
		
	//	 Firebase.Analytics.FirebaseAnalytics.LogEvent("Thrill_Mode");//

	}
	public void Privacypolicy()
    {
		Application.OpenURL("");


	}

	public Animator FirstOpen;

	private Animator _currentPanelAnimator;

	private const string AnimKey = "Open";
}
