using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class RadioManager : MonoBehaviour
	{
		private void Awake()
		{
			RadioManager.Instance = this;
		}

		public void EnableRadio()
		{
			if (this.BlockRadio)
			{
				return;
			}
			if (this.RadioPanel)
			{
				this.RadioPanel.SetActive(true);
			}
			this.isPlay = true;
			this.audioSource = BackgroundMusic.instance.GetAudioSource();
			this.currentRadioIndex = UnityEngine.Random.Range(0, this.Stations.Count);
			this.EnableStation();
		}

		public void DisableRadio()
		{
			if (!this.isPlay)
			{
				return;
			}
			if (this.RadioPanel)
			{
				this.RadioPanel.SetActive(false);
			}
			this.isPlay = false;
			if (this.audioSource.isPlaying)
			{
				this.audioSource.Stop();
			}
		}

		public void NextStation()
		{
			this.DisableStation();
			this.currentRadioIndex++;
			if (this.currentRadioIndex == this.Stations.Count)
			{
				this.currentRadioIndex = 0;
			}
			this.EnableStation();
		}

		public void PrevStation()
		{
			this.DisableStation();
			this.currentRadioIndex--;
			if (this.currentRadioIndex < 0)
			{
				this.currentRadioIndex = this.Stations.Count - 1;
			}
			this.EnableStation();
		}

		private void NextClip()
		{
			if (this.audioSource.isPlaying || this.Stations[this.currentRadioIndex].AudioClips.Length == 0)
			{
				return;
			}
			this.Stations[this.currentRadioIndex].CurrentClipIndex = this.GetRandomIndex(0, this.Stations[this.currentRadioIndex].AudioClips.Length, this.Stations[this.currentRadioIndex].CurrentClipIndex);
			this.audioSource.clip = this.Stations[this.currentRadioIndex].AudioClips[this.Stations[this.currentRadioIndex].CurrentClipIndex];
			this.audioSource.time = 0f;
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.Play();
			}
		}

		private void TurnClipFromPlace(float timeOffset = 0f, bool isStop = false)
		{
			if (isStop)
			{
				if (this.audioSource.isPlaying)
				{
					this.audioSource.Stop();
				}
				return;
			}
			RadioManager.RadioStation radioStation = this.Stations[this.currentRadioIndex];
			if (timeOffset == 0f)
			{
				timeOffset = UnityEngine.Random.Range(0f, radioStation.AudioClips[radioStation.CurrentClipIndex].length - 1.5f);
			}
			else
			{
				timeOffset = Mathf.Clamp(timeOffset, 0f, radioStation.AudioClips[radioStation.CurrentClipIndex].length - 1.5f);
			}
			this.audioSource.clip = radioStation.AudioClips[radioStation.CurrentClipIndex];
			this.audioSource.time = timeOffset;
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.Play();
			}
		}

		private void EnableStation()
		{
			RadioManager.RadioStation radioStation = this.Stations[this.currentRadioIndex];
			if (this.RadioLogoImage)
			{
				this.RadioLogoImage.sprite = radioStation.LogoSprite;
			}
			if (this.RadioNameText)
			{
				this.RadioNameText.text = radioStation.Name;
			}
			InGameLogManager.Instance.RegisterNewMessage(MessageType.RadioName, radioStation.Name);
			if (radioStation.AudioClips.Length == 0)
			{
				this.TurnClipFromPlace(0f, true);
				return;
			}
			float num = Time.fixedTime - radioStation.LastTime;
			bool flag = num + 1.5f < radioStation.AudioClips[radioStation.CurrentClipIndex].length - radioStation.LastClipTime;
			if (flag)
			{
				num += radioStation.LastClipTime;
			}
			else
			{
				num = 0f;
			}
			if (!radioStation.StationEnabled || !flag)
			{
				radioStation.CurrentClipIndex = this.GetRandomIndex(0, radioStation.AudioClips.Length, radioStation.CurrentClipIndex);
			}
			this.TurnClipFromPlace(num, false);
			radioStation.StationEnabled = true;
		}

		private void DisableStation()
		{
			this.Stations[this.currentRadioIndex].LastTime = Time.fixedTime;
			this.Stations[this.currentRadioIndex].LastClipTime = this.audioSource.time;
		}

		private void FixedUpdate()
		{
			if (!this.isPlay)
			{
				return;
			}
			this.NextClip();
		}

		private void Update()
		{
			if (!this.isPlay)
			{
				return;
			}
			float axis = Controls.GetAxis("Radio");
			if (axis == 0f)
			{
				if (this.prevRadioInput > 0f)
				{
					this.NextStation();
				}
				if (this.prevRadioInput < 0f)
				{
					this.PrevStation();
				}
			}
			this.prevRadioInput = axis;
		}

		private int GetRandomIndex(int start, int end, int exclusion)
		{
			if (start + 1 == end || start == end)
			{
				return 0;
			}
			int num;
			for (num = UnityEngine.Random.Range(start, end); num == exclusion; num = UnityEngine.Random.Range(start, end))
			{
			}
			return num;
		}

		public static RadioManager Instance;

		private const float ClipOffset = 1.5f;

		public GameObject RadioPanel;

		public Image RadioLogoImage;

		public Text RadioNameText;

		public List<RadioManager.RadioStation> Stations;

		[HideInInspector]
		public bool BlockRadio;

		private int currentRadioIndex;

		private AudioSource audioSource;

		private bool isPlay;

		private float prevRadioInput;

		[Serializable]
		public class RadioStation
		{
			public string Name;

			public Sprite LogoSprite;

			public AudioClip[] AudioClips;

			[HideInInspector]
			public int CurrentClipIndex;

			[HideInInspector]
			public bool StationEnabled;

			[HideInInspector]
			public float LastTime;

			[HideInInspector]
			public float LastClipTime;
		}
	}
}
