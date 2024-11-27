using System;
using System.Collections.Generic;
using System.Linq;
//using Common.Analytics;
using Game.GlobalComponent.Quality;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class PerformanceDetecting : MonoBehaviour
	{
		public void PlayBonusGame()
		{
			this.ResultPanel.SetActive(false);
		//	AdsManager.ShowFullscreenAd();
			this.BonusGameLoader.Load();
		}

		private void Start()
		{
			this.Init();
		}

		private bool MinimumRequirements()
		{
			return (float)SystemInfo.systemMemorySize < this.MinRam;
		}

		private void Init()
		{
			this.startTesting = true;
			this.Test.text = SystemInfo.systemMemorySize.ToString();
			if (this.MinimumRequirements())
			{
				this.Сonclude(0f);
				return;
			}
			this.index = 0;
			this.StartTestGrop();
			this.detectingTime = 0f;
			for (int i = 0; i < this.TestGroups.Length; i++)
			{
				float num = this.TestGroups[i].PerformanceTests[0].DetectingTime;
				for (int j = 0; j < this.TestGroups[i].PerformanceTests.Length; j++)
				{
					if (this.TestGroups[i].PerformanceTests[j].DetectingTime > num)
					{
						num = this.TestGroups[i].PerformanceTests[j].DetectingTime;
					}
				}
				this.detectingTime += num;
			}
			this.ProgressBar.maxValue = this.detectingTime;
		}

		private void FixedUpdate()
		{
			if (this.startTesting)
			{
				this.ShowProgress();
			}
		}

		private void ShowProgress()
		{
//			Debug.Log("Showing Progress!");
			this.detectingTime -= Time.deltaTime;
			if (this.detectingTime <= 0f)
			{
				this.ProgressText.text = "100%";
				this.ProgressBar.value = this.ProgressBar.maxValue;
				this.ProgressBar.gameObject.SetActive(false);
				return;
			}
			this.ProgressBar.value = (1f - this.detectingTime / this.ProgressBar.maxValue) * this.ProgressBar.maxValue;
			this.ProgressText.text = ((int)((1f - this.detectingTime / this.ProgressBar.maxValue) * 100f)).ToString() + "%";
		}

		private void StartTestGrop()
		{
		//	if (this.index >= this.TestGroups.Length || this.CalcResult() < this.ResultToMidQuality)
			{
				this.Сonclude(this.CalcResult());
				return;
			}
			foreach (PerformanceTest performanceTest in this.TestGroups[this.index].PerformanceTests)
			{
				performanceTest.Init();
				performanceTest.EndTestingEvent += this.EndTest;
			}
		}

		private void EndTest(float result, PerformanceTest test)
		{
			if (this.testEnd)
			{
				return;
			}
			if (!test.IsNotReturnResults)
			{
				if (this.testResults.Count > 0 || !this.testResults.ContainsKey(test))
				{
					this.testResults.Add(test, result);
				}
				else
				{
					this.testResults[test] = result;
				}
			}
			bool flag = this.TestGroups[this.index].PerformanceTests.All((PerformanceTest t) => t.IsEnd) || this.CalcResult() < this.ResultToMidQuality;
			if (flag)
			{
				this.index++;
				this.StartTestGrop();
			}
		}

		private float CalcResult()
		{
			float num = 0f;
			ICollection<float> values = this.testResults.Values;
			foreach (float num2 in values)
			{
				float num3 = num2;
				num += num3;
			}
			return num / (float)values.Count;
		}

		private void Сonclude(float res)
		{
			this.testEnd = true;
			this.detectingTime = 0f;
			bool flag = false;
			Debug.Log("-- Running the game in High QUality by defauly as performance detector wasn't working...");
			/*if (res > this.ResultToUltraQuality)
			{
				QualityManager.ChangeQuality(9, false);
				BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.P, "QUltra", 4);
			}
			else if (res > this.ResultToHighQuality)*/
			{
				QualityManager.ChangeQuality(8, false);
//				BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.P, "QHigh", 3);
			}
			/*else if (res > this.ResultToMidQuality)
			{
				QualityManager.ChangeQuality(7, false);
				BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.P, "QMid", 2);
			}
			else if (res > this.ResultToLowQuality)
			{
				QualityManager.ChangeQuality(6, false);
				BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.P, "QLow", 1);
			}
			else
			{
				this.ResultPanel.SetActive(true);
				QualityManager.ChangeQuality(6, false);
				BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.P, "QToSlow", 0);
				flag = true;
			}*/
			if (!flag)
			{
				BaseProfile.StoreValue<bool>(true, SystemSettingsList.PerformanceDetected.ToString());
				this.ResultPanel.SetActive(false);
				this.MainMenuLoader.Load();
			}
		}

		public void PlayAnyway()
		{
			this.ResultPanel.SetActive(false);
			BaseProfile.StoreValue<bool>(true, SystemSettingsList.PerformanceDetected.ToString());
//			AdsManager.ShowFullscreenAd();
			this.MainMenuLoader.Load();
		}

		private void OnGUI()
		{
			if (!this.IsDebug)
			{
				return;
			}
			GUIStyle guistyle = new GUIStyle();
			GUIStyle guistyle2 = new GUIStyle();
			guistyle.fontSize = 60;
			guistyle2.fontSize = 62;
			guistyle.normal.textColor = Color.white;
			guistyle2.normal.textColor = Color.black;
			GUI.backgroundColor = Color.gray;
			GUI.Box(new Rect(15f, 60f, 500f, 200f), string.Empty);
			GUI.Label(new Rect(22f, 60f, 100f, 100f), "Result: " + this.CalcResult(), guistyle2);
			GUI.Label(new Rect(20f, 60f, 100f, 100f), "Result: " + this.CalcResult(), guistyle);
			GUI.Label(new Rect(22f, 120f, 100f, 100f), "Test ended: " + this.testResults.Count, guistyle2);
			GUI.Label(new Rect(20f, 120f, 100f, 100f), "Test ended: " + this.testResults.Count, guistyle);
			GUI.Label(new Rect(20f, 180f, 100f, 100f), "RAM: " + this.Test.text, guistyle2);
			GUI.Label(new Rect(22f, 180f, 100f, 100f), "RAM: " + this.Test.text, guistyle);
		}

		[Separator("Test result")]
		public float ResultToUltraQuality = 95f;

		public float ResultToHighQuality = 70f;

		public float ResultToMidQuality = 50f;

		public float ResultToLowQuality = 20f;

		[Separator("Minimum Requirements")]
		public float MinRam = 600f;

		[Separator("Test Group")]
		public PerformanceDetecting.TestGroup[] TestGroups;

		[Separator("All setup")]
		public GameObject ResultPanel;

		public LoadSceneController MainMenuLoader;

		public LoadSceneController BonusGameLoader;

		public Slider ProgressBar;

		public Text ProgressText;

		public Text Test;

		public bool IsDebug;

		private float detectingTime;

		private int index;

		private Dictionary<PerformanceTest, float> testResults = new Dictionary<PerformanceTest, float>();

		private bool testEnd;

		private bool startTesting;

		[Serializable]
		public class TestGroup
		{
			public string Name;

			public PerformanceTest[] PerformanceTests;
		}
	}
}
