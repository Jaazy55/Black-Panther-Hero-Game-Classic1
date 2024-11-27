using System;
using Game.Managers;
using UnityEngine;

public class BaseSoundController : MonoBehaviour
{
	private void Awake()
	{
		AudioSource audioSource = base.GetComponent<AudioSource>();
		if (audioSource != null)
		{
			audioSource.volume = SoundManager.instance.GetSoundValue() * this.VolumeEffect;
			if (this.IsMusicPlayer)
			{
				audioSource.volume *= SoundManager.instance.GetMusicValue();
			}
			SoundManager.ValueChanged b = delegate(float value)
			{
				if (audioSource != null)
				{
					audioSource.volume = value * this.VolumeEffect;
					if (this.IsMusicPlayer)
					{
						audioSource.volume *= SoundManager.instance.GetMusicValue();
					}
				}
			};
			if (this.IsInGameSound)
			{
				SoundManager instance = SoundManager.instance;
				instance.GameSoundChanged = (SoundManager.ValueChanged)Delegate.Combine(instance.GameSoundChanged, b);
			}
			else
			{
				SoundManager instance2 = SoundManager.instance;
				instance2.SoundChanged = (SoundManager.ValueChanged)Delegate.Combine(instance2.SoundChanged, b);
			}
		}
	}

	public float VolumeEffect = 1f;

	public bool IsInGameSound;

	public bool IsMusicPlayer;
}
