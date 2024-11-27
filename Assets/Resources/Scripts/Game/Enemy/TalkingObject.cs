using System;
using UnityEngine;

namespace Game.Enemy
{
	[RequireComponent(typeof(AudioSource), typeof(BaseSoundController))]
	public class TalkingObject : MonoBehaviour
	{
		public bool IsGreetingAvaible
		{
			get
			{
				if (this.parentNPC == null)
				{
					this.parentNPC = base.gameObject.GetComponentInParent<BaseNPC>();
				}
				return this.parentNPC.CurrentControllerType == BaseNPC.NPCControllerType.Simple;
			}
		}

		public void TalkPhraseOfType(TalkingObject.PhraseType type)
		{
			if (Time.time < this.nextAvaibleTalkTime)
			{
				return;
			}
			TalkingObject.Phrase phrase = null;
			foreach (TalkingObject.Phrase phrase2 in this.Phrases)
			{
				if (phrase2.Type == type)
				{
					phrase = phrase2;
					break;
				}
			}
			if (phrase == null || phrase.Clips.Length == 0)
			{
				return;
			}
			AudioClip audioClip = phrase.Clips[UnityEngine.Random.Range(0, phrase.Clips.Length)];
			if (audioClip == null)
			{
				return;
			}
			float num = audioClip.length;
			if (UnityEngine.Random.value <= phrase.ChanceToProc)
			{
				num *= 2f;
				this.audioSource.PlayOneShot(audioClip);
			}
			this.nextAvaibleTalkTime = Time.time + num;
		}

		private void Awake()
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}

		private const int SuccessfulPhraseCooldownCounter = 2;

		public TalkingObject.Phrase[] Phrases;

		private float nextAvaibleTalkTime;

		private AudioSource audioSource;

		private BaseNPC parentNPC;

		public enum PhraseType
		{
			Greeting,
			Alarm,
			RunOut,
			Attack
		}

		[Serializable]
		public class Phrase
		{
			public TalkingObject.PhraseType Type;

			public AudioClip[] Clips;

			[Range(0f, 1f)]
			public float ChanceToProc;
		}
	}
}
