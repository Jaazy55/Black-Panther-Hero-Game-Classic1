using System;
using System.Collections.Generic;
using Game.Managers;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class PointSoundManager : MonoBehaviour
	{
		public static PointSoundManager Instance
		{
			get
			{
				if (PointSoundManager.instance == null)
				{
					UnityEngine.Debug.LogError("PointSoundManager is no initialized");
				}
				return PointSoundManager.instance;
			}
		}

		private void Awake()
		{
			if (PointSoundManager.instance == null)
			{
				PointSoundManager.instance = this;
			}
			this.currentVolume = SoundManager.instance.GetSoundValue() * this.Volume;
			SoundManager.ValueChanged b = delegate(float value)
			{
				this.currentVolume = SoundManager.instance.GetSoundValue() * this.Volume;
				this.currentVolume *= value;
			};
			SoundManager soundManager = SoundManager.instance;
			soundManager.GameSoundChanged = (SoundManager.ValueChanged)Delegate.Combine(soundManager.GameSoundChanged, b);
		}

		public void PlaySoundAtPoint(Vector3 position, TypeOfSound type)
		{
			PointSoundManager.AudioClipConfig[] array = new PointSoundManager.AudioClipConfig[this.AudioClips.Count];
			int num = 0;
			foreach (PointSoundManager.AudioClipConfig audioClipConfig in this.AudioClips)
			{
				if (audioClipConfig.type == type)
				{
					array[num] = audioClipConfig;
					num++;
				}
			}
			if (array.Length > 0)
			{
				int num2 = UnityEngine.Random.Range(0, num);
				float volume = this.currentVolume * array[num2].volume;
				GameObject specificPointSound = array[num2].specificPointSound;
				this.PlaySound(array[num2].clip, position, volume, specificPointSound);
			}
			else
			{
				UnityEngine.Debug.LogError("Wrong type");
			}
		}

		public void PlaySoundAtPoint(Vector3 position, AudioClip clip, float soundVolume = 1f)
		{
			soundVolume *= this.currentVolume;
			this.PlaySound(clip, position, soundVolume, null);
		}

		public void PlaySoundAtPoint(Vector3 position, string soundName)
		{
			AudioClip audioClip = null;
			float volume = 0f;
			GameObject specificPointSound = null;
			foreach (PointSoundManager.AudioClipConfig audioClipConfig in this.AudioClips)
			{
				if (audioClipConfig.name == soundName)
				{
					volume = this.currentVolume * audioClipConfig.volume;
					audioClip = audioClipConfig.clip;
					specificPointSound = audioClipConfig.specificPointSound;
				}
			}
			if (audioClip != null)
			{
				this.PlaySound(audioClip, position, volume, specificPointSound);
			}
			else
			{
				UnityEngine.Debug.LogError("Wrong sound name");
			}
		}

		public void PlayCustomClipAtPoint(Vector3 position, AudioClip clip, GameObject specificPointSound = null)
		{
			this.PlaySound(clip, position, this.currentVolume, specificPointSound);
		}

		public void Play3DSoundOnPoint(Vector3 position, AudioClip clip)
		{
			this.PlaySound(clip, position, this.currentVolume, this.SamplePointSound3D);
		}

		private void PlaySound(AudioClip clip, Vector3 position, float volume, GameObject specificPointSound)
		{
			if (clip == null)
			{
				return;
			}
			GameObject fromPool = PoolManager.Instance.GetFromPool((!specificPointSound) ? this.pointSound : specificPointSound);
			fromPool.transform.position = position;
			AudioSource component = fromPool.GetComponent<AudioSource>();
			component.clip = clip;
			component.volume = volume;
			component.Play();
			PoolManager.Instance.ReturnToPoolWithDelay(fromPool, clip.length);
		}

		private static PointSoundManager instance;

		public GameObject pointSound;

		public GameObject SamplePointSound3D;

		[Range(0f, 1f)]
		public float Volume;

		public List<PointSoundManager.AudioClipConfig> AudioClips = new List<PointSoundManager.AudioClipConfig>();

		private float currentVolume;

		private AudioSource[] playingClips;

		[Serializable]
		public class AudioClipConfig
		{
			public string name;

			public GameObject specificPointSound;

			public AudioClip clip;

			[Range(0f, 1f)]
			public float volume;

			public TypeOfSound type;
		}
	}
}
