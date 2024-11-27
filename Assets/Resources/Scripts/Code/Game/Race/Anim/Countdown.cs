using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Game.Race.Anim
{
	public class Countdown : MonoBehaviour
	{
		public void SetAudio(AudioClip audioClip)
		{
			this.audioSource.clip = audioClip;
			this.audioSource.Play();
		}

		public void SetText(string text)
		{
			this.text.text = text;
		}

		[SerializeField]
		private AudioSource audioSource;

		[SerializeField]
		private Text text;
	}
}
