using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class CreditsManager : MonoBehaviour
	{
		private void Awake()
		{
			this.AddMusicName();
			float num = this.ScrollSpeed / Time.deltaTime;
			this.textLifeTime = Vector3.Distance(this.StartTextPosition.transform.position, this.EndTextPosition.transform.position) / num;
		}

		private void AddMusicName()
		{
			this.CreditsLines.Add(" ");
			this.CreditsLines.Add("Used music:");
			BackgroundMusic backgroundMusic = UnityEngine.Object.FindObjectOfType<BackgroundMusic>();
			string name = backgroundMusic.GetComponent<AudioSource>().clip.name;
			this.CreditsLines.Add(name);
			foreach (AudioClip audioClip in backgroundMusic.Clips)
			{
				if (!audioClip.name.Equals(name))
				{
					this.CreditsLines.Add(audioClip.name);
				}
			}
			if (this.RadioManagerPrefub)
			{
				foreach (RadioManager.RadioStation radioStation in this.RadioManagerPrefub.Stations)
				{
					foreach (AudioClip audioClip2 in radioStation.AudioClips)
					{
						if (!audioClip2.name.Equals(name))
						{
							this.CreditsLines.Add(audioClip2.name);
						}
					}
				}
			}
			this.CreditsLines.Add(" ");
		}

		private void Update()
		{
			if (this.waitTimer > 0f)
			{
				this.waitTimer -= Time.deltaTime;
			}
			else if (Vector3.Distance(this.lastInitedText.anchoredPosition, this.StartTextPosition.anchoredPosition) > this.TextSample.rect.height)
			{
				this.CreateNewStringLine(this.CreditsLines[this.currentLineIndex]);
				this.currentLineIndex++;
				if (this.currentLineIndex > this.CreditsLines.Count - 1)
				{
					this.currentLineIndex = 0;
					this.waitTimer = 3f;
				}
			}
			foreach (KeyValuePair<Text, float> item in this.initedTexts)
			{
				item.Key.gameObject.transform.Translate(Vector3.up * this.ScrollSpeed);
				Color color = item.Key.color;
				float num = 1f - (Time.time - item.Value) / this.textLifeTime;
				item.Key.color = new Color(color.r, color.g, color.b, num);
				if (num < 0.1f)
				{
					UnityEngine.Object.Destroy(item.Key.gameObject);
					this.initedTexts.Remove(item);
					break;
				}
			}
		}

		private void OnEnable()
		{
			this.currentLineIndex = 0;
			this.lastInitedText = null;
			this.waitTimer = 0f;
			this.CreateNewStringLine(this.CreditsLines[this.currentLineIndex]);
			this.currentLineIndex++;
		}

		private void OnDisable()
		{
			foreach (KeyValuePair<Text, float> keyValuePair in this.initedTexts)
			{
				UnityEngine.Object.Destroy(keyValuePair.Key.gameObject);
			}
			this.initedTexts.Clear();
		}

		private void CreateNewStringLine(string newString)
		{
			this.lastInitedText = UnityEngine.Object.Instantiate<RectTransform>(this.TextSample);
			RectTransform rectTransform = this.lastInitedText.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(this.CreditsPanel.transform, false);
			}
			else
			{
				this.lastInitedText.transform.parent = this.CreditsPanel.transform;
			}
			this.lastInitedText.transform.localScale = Vector3.one;
			this.lastInitedText.anchoredPosition = this.StartTextPosition.anchoredPosition;
			Text component = this.lastInitedText.GetComponent<Text>();
			component.text = newString;
			this.initedTexts.Add(component, Time.time);
		}

		private const float WaitTimeAfterFinishedLines = 3f;

		public List<string> CreditsLines = new List<string>();

		public float ScrollSpeed;

		public RectTransform TextSample;

		public RectTransform StartTextPosition;

		public RectTransform EndTextPosition;

		public GameObject CreditsPanel;

		public RadioManager RadioManagerPrefub;

		private float textLifeTime;

		private RectTransform lastInitedText;

		private int currentLineIndex;

		private float waitTimer;

		private readonly IDictionary<Text, float> initedTexts = new Dictionary<Text, float>();
	}
}
