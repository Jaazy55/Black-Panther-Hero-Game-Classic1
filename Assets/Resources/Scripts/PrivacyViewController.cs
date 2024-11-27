using System;
using System.Collections;
//using Common.Analytics;
using Game.GlobalComponent;
using Game.Managers;
using UnityEngine;

public class PrivacyViewController : MonoBehaviour
{
	private void Awake()
	{
		this.ShowPanel(false);
	}

	private void OnDestroy()
	{
		base.StopAllCoroutines();
		if (SoundManager.instance.MusicMuted)
		{
			this.UnmuteMusic();
		}
	}

	private void Start()
	{
		bool flag = BaseProfile.ResolveValue<bool>(SystemSettingsList.PerformanceDetected.ToString(), false);
		bool flag2 = BaseProfile.ResolveValue<bool>("PPConfirmed", false);
		if (flag && !flag2)
		{
			this.MuteMusic();
			this.ShowPanel(true);
			BackButton.BackButtonsActive = false;
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private IEnumerator BehaviorOnAccept()
	{
		SoundManager.PlaySoundAtPosition(this.m_AcceptSound, base.transform.position, SoundManager.SoundType.Sound);
		yield return new WaitForSecondsRealtime(this.m_AcceptSound.length);
		BackButton.BackButtonsActive = true;
		this.UnmuteMusic();
		base.gameObject.SetActive(false);
		yield break;
	}

	private void MuteMusic()
	{
		SoundManager.instance.MusicMuted = true;
	}

	private void UnmuteMusic()
	{
		SoundManager.instance.MusicMuted = false;
	}

	public void OpenLink()
	{
		Application.OpenURL("");

    }

	public void Confirm()
	{
		BaseProfile.StoreValue<bool>(true, "PPConfirmed");
		this.ShowPanel(false);
		base.StartCoroutine(this.BehaviorOnAccept());
	//	BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.AM, "PPolicy", "Confirmed");
	}

	private void ShowPanel(bool value)
	{
		this.m_CanvasGroup.alpha = ((!value) ? 0f : 1f);
		this.m_CanvasGroup.interactable = value;
	}

	[SerializeField]
	private string m_Link;

	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private AudioClip m_AcceptSound;
}
