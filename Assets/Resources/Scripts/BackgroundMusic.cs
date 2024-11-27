using System;
using Game.Character;
using Game.GlobalComponent;
using Game.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
	public AudioSource GetAudioSource()
	{
		return (!this._thisAudio) ? base.GetComponent<AudioSource>() : this._thisAudio;
	}

	public void PlayTimeQuestClip()
	{
		RadioManager.Instance.DisableRadio();
		RadioManager.Instance.BlockRadio = true;
		this._thisAudio.clip = this.TimeQuestClip;
		this._thisAudio.loop = true;
		if (!this._thisAudio.isPlaying)
		{
			this._thisAudio.Play();
		}
	}

	public void StopTimeQuestClip()
	{
		RadioManager.Instance.BlockRadio = false;
		this._thisAudio.Stop();
		this._thisAudio.loop = false;
		if (PlayerInteractionsManager.Instance.inVehicle && PlayerInteractionsManager.Instance.LastDrivableVehicle.VehicleSpecificPrefab.HasRadio)
		{
			RadioManager.Instance.EnableRadio();
		}
	}

	private void PlayOther()
	{
		int num = UnityEngine.Random.Range(0, this.Clips.Length);
		if (this._thisAudio.clip.Equals(this.Clips[num]))
		{
			num = (num + 1) % this.Clips.Length;
		}
		this._thisAudio.clip = this.Clips[num];
		this._thisAudio.Play();
	}

	private void Awake()
	{
		this.HasPlayClips();
		if (BackgroundMusic.instance == null)
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			this._thisAudio = base.GetComponent<AudioSource>();
			if (this._thisAudio != null)
			{
			//	this._thisAudio.volume = SoundManager.instance.GetMusicValue();
			}
			SoundManager soundManager = SoundManager.instance;
			soundManager.MusicChanged = (SoundManager.ValueChanged)Delegate.Combine(soundManager.MusicChanged, new SoundManager.ValueChanged(delegate(float value)
			{
				if (this._thisAudio != null)
				{
					this._thisAudio.volume = value * this.MusicVolumeFactor;
				}
			}));
			BackgroundMusic.instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		this.HasPlayClips();
		if (this._thisAudio)
		{
			this._thisAudio.Stop();
		}
	}

	private bool HasPlayClips()
	{
		this.playClips = true;// (SceneManager.GetActiveScene().name.Equals(Constants.Scenes.Menu.ToString()) && BaseProfile.ResolveValue<bool>(SystemSettingsList.PerformanceDetected.ToString(), false));
		return this.playClips;
	}

	private void FixedUpdate()
	{
		if (!this.playClips)
		{
			//return;
		}
		if (!this._thisAudio.isPlaying)
		{
			this.PlayOther();
		}
	}

	public static BackgroundMusic instance;

	public AudioClip[] Clips;

	public AudioClip TimeQuestClip;

	public bool PlayBackgroundInGameScene;

	public float MusicVolumeFactor = 0.5f;

	private AudioSource _thisAudio;

	private bool playClips;
}
