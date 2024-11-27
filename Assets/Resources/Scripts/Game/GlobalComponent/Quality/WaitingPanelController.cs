using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent.Quality
{
	public class WaitingPanelController : MonoBehaviour
	{
		public static WaitingPanelController Instance
		{
			get
			{
				WaitingPanelController result;
				if ((result = WaitingPanelController.instance) == null)
				{
					result = (WaitingPanelController.instance = UnityEngine.Object.FindObjectOfType<WaitingPanelController>());
				}
				return result;
			}
		}

		public bool IsActivate
		{
			get
			{
				return this.PanelWaiting.gameObject.activeSelf;
			}
		}

		public void StartWaiting(Action callback, int frameCount = 30)
		{
			this.PanelWaiting.SetActive(true);
			int num = WaitingPanelController.GenerateCoroutineID();
			Coroutine value = base.StartCoroutine(this.ShowFunction(num, callback, frameCount));
			this.ShowCoroutines.Add(num, value);
		}

		public void StartWaiting(Action callback, Func<bool> checkFunction)
		{
			this.PanelWaiting.SetActive(true);
			this.m_checkFunction = checkFunction;
			int num = WaitingPanelController.GenerateCoroutineID();
			Coroutine value = base.StartCoroutine(this.ShowFunction(num, callback, checkFunction));
			this.ShowCoroutines.Add(num, value);
		}

		private static int GenerateCoroutineID()
		{
			WaitingPanelController.lastID++;
			int result = WaitingPanelController.lastID;
			if (WaitingPanelController.lastID == 2147483647)
			{
				WaitingPanelController.lastID = int.MinValue;
			}
			return result;
		}

		private void Awake()
		{
			WaitingPanelController.instance = this;
		}

		private void RemoveIDCoroutine(int id)
		{
			if (!this.ShowCoroutines.ContainsKey(id))
			{
				return;
			}
			this.ShowCoroutines.Remove(id);
			Dictionary<int, Coroutine>.KeyCollection keys = this.ShowCoroutines.Keys;
			foreach (int key in keys)
			{
				if (this.ShowCoroutines[key] == null)
				{
					this.ShowCoroutines.Remove(key);
				}
			}
			if (this.ShowCoroutines.Count == 0)
			{
				this.PanelWaiting.SetActive(false);
			}
		}

		private IEnumerator ShowFunction(int id, Action callback, int frameCount)
		{
			int timer = Time.frameCount + frameCount;
			yield return new WaitForEndOfFrame();
			while (Time.frameCount < timer && timer != 0)
			{
				yield return new WaitForEndOfFrame();
			}
			timer = 0;
			this.RemoveIDCoroutine(id);
			if (callback != null)
			{
				callback();
			}
			yield break;
		}

		private IEnumerator ShowFunction(int id, Action callback, Func<bool> checkFunction)
		{
			yield return new WaitForEndOfFrame();
			if (checkFunction != null)
			{
				while (!checkFunction())
				{
					yield return new WaitForEndOfFrame();
				}
			}
			this.RemoveIDCoroutine(id);
			if (callback != null)
			{
				callback();
			}
			yield break;
		}

		private static WaitingPanelController instance;

		private static int lastID = int.MinValue;

		public GameObject PanelWaiting;

		private Func<bool> m_checkFunction;

		private Dictionary<int, Coroutine> ShowCoroutines = new Dictionary<int, Coroutine>();
	}
}
