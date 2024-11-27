using System;
using UnityEngine;

namespace Game.Managers
{
	public class SoundManager
	{
		private SoundManager()
		{
			this._soundValue = BaseProfile.SoundVolume;
			this._musicValue = BaseProfile.MusicVolume;
		}

		public static SoundManager instance
		{
			get
			{
				if (SoundManager._instance == null)
				{
					SoundManager._instance = new SoundManager();
				}
				return SoundManager._instance;
			}
		}

		public static void PlaySoundAtPosition(AudioClip clip, Vector3 position, SoundManager.SoundType soundType)
		{
			AudioSource.PlayClipAtPoint(clip, position, SoundManager.instance.GetValueByType(soundType));
		}

		public SoundManager.ValueChanged AddValueChangeByType(SoundManager.SoundType type, SoundManager.ValueChanged valueChanged)
		{
			if (type == SoundManager.SoundType.Sound)
			{
				this.SoundChanged = (SoundManager.ValueChanged)Delegate.Combine(this.SoundChanged, valueChanged);
			}
			if (type == SoundManager.SoundType.Music)
			{
				this.MusicChanged = (SoundManager.ValueChanged)Delegate.Combine(this.MusicChanged, valueChanged);
			}
			if (type == SoundManager.SoundType.GameSound)
			{
				this.GameSoundChanged = (SoundManager.ValueChanged)Delegate.Combine(this.GameSoundChanged, valueChanged);
			}
			return this.SoundChanged;
		}

		public void SetSoundValue(float value)
		{
			if (this.SoundChanged != null)
			{
				this.SoundChanged((!this._soundMuted) ? value : 0f);
			}
			if (this.GameSoundChanged != null)
			{
				this.GameSoundChanged((!this._gameSoundMuted) ? value : 0f);
			}
			this._soundValue = value;
			BaseProfile.SoundVolume = value;
		}

		public void SetMusicValue(float value)
		{
			if (this.MusicChanged != null)
			{
				this.MusicChanged((!this._musicMuted) ? value : 0f);
			}
			this._musicValue = value;
			BaseProfile.MusicVolume = value;
		}

		public float GetSoundValue()
		{
			return this._soundValue;
		}

		public float GetMusicValue()
		{
			return this._musicValue;
		}

		public float GetValueByType(SoundManager.SoundType type)
		{
			switch (type)
			{
			case SoundManager.SoundType.Sound:
				return this.GetSoundValue();
			case SoundManager.SoundType.Music:
				return this.GetMusicValue();
			case SoundManager.SoundType.GameSound:
				return this.GetSoundValue();
			default:
				return this.GetSoundValue();
			}
		}

		public void SetValueByType(float value, SoundManager.SoundType type)
		{
			switch (type)
			{
			case SoundManager.SoundType.Sound:
				this.SetSoundValue(value);
				break;
			case SoundManager.SoundType.Music:
				this.SetMusicValue(value);
				break;
			case SoundManager.SoundType.GameSound:
				this.SetSoundValue(value);
				break;
			default:
				this.SetSoundValue(value);
				break;
			}
		}

		public bool SoundMuted
		{
			get
			{
				return this._soundMuted;
			}
			set
			{
				this._soundMuted = value;
				SoundManager.instance.SetSoundValue(this._soundValue);
			}
		}

		public bool GameSoundMuted
		{
			get
			{
				return this._gameSoundMuted;
			}
			set
			{
				this._gameSoundMuted = value;
				SoundManager.instance.SetSoundValue(this._soundValue);
			}
		}

		public bool MusicMuted
		{
			get
			{
				return this._musicMuted;
			}
			set
			{
				this._musicMuted = value;
				SoundManager.instance.SetMusicValue(this._musicValue);
			}
		}

		private static SoundManager _instance;

		public SoundManager.ValueChanged SoundChanged;

		public SoundManager.ValueChanged GameSoundChanged;

		public SoundManager.ValueChanged MusicChanged;

		private bool _soundMuted;

		private bool _gameSoundMuted;

		private bool _musicMuted;

		private float _soundValue = -1f;

		private float _musicValue = -1f;

		public enum SoundType
		{
			Sound,
			Music,
			GameSound
		}

		public delegate void ValueChanged(float newValue);
	}
}
