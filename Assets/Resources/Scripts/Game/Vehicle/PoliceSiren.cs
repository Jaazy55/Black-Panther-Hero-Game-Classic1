using System;
using System.Collections;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class PoliceSiren : MonoBehaviour, IInitable
	{
		public void Init()
		{
			if (this.SirenSound)
			{
				this.SirenAudioSource.clip = this.SirenSound;
			}
			base.Invoke("InitDummyDriver", 0.2f);
		}

		public void DeInit()
		{
			if (this.SirenAudioSource)
			{
				this.SirenAudioSource.Stop();
				this.SirenAudioSource.clip = null;
			}
			this.BlueLight.SetActive(false);
			this.RedLight.SetActive(false);
			this.RedMark.SetActive(false);
			this.BlueMark.SetActive(false);
			this.currentDummyDriver = null;
			this.working = false;
			base.StopAllCoroutines();
		}

		private void Awake()
		{
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), this.ChangeLightTime);
			this.DeInit();
		}

		private void FixedUpdate()
		{
			if (!this.working)
			{
				return;
			}
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void InitDummyDriver()
		{
			this.currentDummyDriver = base.GetComponentInChildren<DummyDriver>();
			if (!this.currentDummyDriver)
			{
				return;
			}
			DummyDriver dummyDriver = this.currentDummyDriver;
			dummyDriver.DummyExitEvent = (DummyDriver.DummyEvent)Delegate.Combine(dummyDriver.DummyExitEvent, new DummyDriver.DummyEvent(this.DeInit));
			this.SirenAudioSource.Play();
			base.StartCoroutine(this.MiniMapMarkerChange());
			this.working = true;
		}

		private void SlowUpdate()
		{
			this.activeRedLight = !this.activeRedLight;
			this.RedLight.SetActive(this.activeRedLight);
			this.BlueLight.SetActive(!this.activeRedLight);
		}

		private IEnumerator MiniMapMarkerChange()
		{
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			for (;;)
			{
				this.activeRedMark = !this.activeRedMark;
				this.RedMark.SetActive(this.activeRedMark);
				this.BlueMark.SetActive(!this.activeRedMark);
				for (int i = 0; i < 15; i++)
				{
					yield return waitForEndOfFrame;
				}
			}
			yield break;
		}

		private const int ChangeMarkerWaitingFrame = 15;

		public GameObject BlueLight;

		public GameObject RedLight;

		public GameObject BlueMark;

		public GameObject RedMark;

		public float ChangeLightTime = 0.5f;

		public AudioSource SirenAudioSource;

		public AudioClip SirenSound;

		private bool working;

		private bool activeRedLight;

		private bool activeRedMark;

		private SlowUpdateProc slowUpdateProc;

		private DummyDriver currentDummyDriver;
	}
}
