using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Training
{
	public class TrainingManager : MonoBehaviour
	{
		public static TrainingManager Instance
		{
			get
			{
				return TrainingManager.instance;
			}
		}

		public static int CompleteTrainingsCount
		{
			get
			{
				return TrainingManager.completedTrainings.Count;
			}
		}

		public static void ClearCompletedTrainingsInfo()
		{
			BaseProfile.ClearArray<string>("CompletedTrainingsArray");
		}

		public void InitOffSceneTraining(TrainingBase training)
		{
			this.WatchedAtObject(training.ObjectForActive.gameObject, training);
		}

		public void ClearLocalCompletedTrainingsInfo()
		{
			TrainingManager.completedTrainings.Clear();
		}

		public void SkipTraining()
		{
			this.trainingSkipped = true;
			BaseProfile.SkipTraining = true;
			this.currentTraining.EndTraining();
			this.trainingsQueue.Clear();
			TrainingManager.SkipTrainingEvent(TrainingManager.CompleteTrainingsCount);
		}

		public void CompleteThisTraining()
		{
			this.currentTraining.EndTraining();
		}

		public void SetTrainingStatus(bool activate)
		{
			this.trainingSkipped = !activate;
		}

		public void TrackedObjectActivated(TrainingObjectTracker tracker)
		{
			if (this.trainingSkipped)
			{
				return;
			}
			if (this.trackedObjects.ContainsKey(tracker.gameObject))
			{
				this.lastTrainingEndTime = (float)DateTime.Now.Second;
				this.AddTrainingInQueue(this.trackedObjects[tracker.gameObject]);
			}
		}

		public void TrackedObjectDestroyed(TrainingObjectTracker tracker)
		{
			if (this.trackedObjects.ContainsKey(tracker.gameObject))
			{
				TrainingBase item = this.trackedObjects[tracker.gameObject];
				if (this.trainingsQueue.Contains(item))
				{
					this.trainingsQueue.Remove(item);
				}
				this.trackedObjects.Remove(tracker.gameObject);
			}
		}

		public void TrainingEnd()
		{
			BaseProfile.StoreLastElementOfArray<string>(this.currentTraining.TrainingName, "CompletedTrainingsArray");
			TrainingManager.completedTrainings.Add(this.currentTraining.TrainingName);
			this.trainingsQueue.Remove(this.currentTraining);
			this.currentTraining.gameObject.SetActive(false);
			this.currentTraining = null;
			this.TrainingPanel.SetActive(false);
			this.lastTrainingEndTime = (float)DateTime.Now.Second;
			BackButton.ChangeBackButtonsStatus(true);
		}

		public void ClickOnBlockerPanel()
		{
			this.currentTraining.ClickOnPanel();
		}

		private void AddTrainingInQueue(TrainingBase training)
		{
			if (!this.trainingsQueue.Contains(training))
			{
				this.trainingsQueue.Add(training);
			}
		}

		private void StartTraining(TrainingBase training)
		{
			if (this.trainingSkipped)
			{
				return;
			}
			if (!training.ObjectForActive.gameObject.activeInHierarchy || TrainingManager.completedTrainings.Contains(training.TrainingName))
			{
				this.trainingsQueue.Remove(training);
				return;
			}
			this.currentTraining = training;
			this.TrainingPanel.SetActive(true);
			this.currentTraining.gameObject.SetActive(true);
			this.MessageText.text = this.currentTraining.ObjectDescription;
			this.ToContinueMessageText.text = this.currentTraining.GetContinueMessage();
			this.ConfigureHelper(training.ObjectForActive);
			foreach (RectTransform rectTransform in this.Pointers)
			{
				this.EmptyHelper.SetParent(rectTransform.parent, true);
				rectTransform.anchorMin = this.EmptyHelper.anchorMin;
				rectTransform.anchorMax = this.EmptyHelper.anchorMax;
				rectTransform.anchoredPosition = this.EmptyHelper.anchoredPosition;
				rectTransform.sizeDelta = this.EmptyHelper.sizeDelta * (1f + this.currentTraining.AdditionalPointerScalling);
				rectTransform.pivot = this.EmptyHelper.pivot;
			}
			Image component = this.currentTraining.ObjectForActive.GetComponent<Image>();
			this.pointerImage.sprite = ((!component) ? null : component.sprite);
			this.pointerImage.preserveAspect = (!component || component.preserveAspect);
			this.pointerImage.type = ((!component) ? Image.Type.Simple : component.type);
			this.currentTraining.StartTraining();
			BackButton.ChangeBackButtonsStatus(false);
		}

		private void Awake()
		{
			TrainingManager.instance = this;
			this.pointerImage = this.Pointers[0].GetComponent<Image>();
			this.trainingSkipped = BaseProfile.SkipTraining;
			string[] array = BaseProfile.ResolveArray<string>("CompletedTrainingsArray");
			if (array != null)
			{
				TrainingManager.completedTrainings = array.ToList<string>();
			}
		}

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(this.TimeToStartTrainings);
			foreach (TrainingBase trainingBase in this.StartTrainings)
			{
				this.AddTrainingInQueue(trainingBase);
				this.WatchedAtObject(trainingBase.ObjectForActive.gameObject, trainingBase);
			}
			foreach (TrainingBase trainingBase2 in this.OnActiveTrainings)
			{
				this.WatchedAtObject(trainingBase2.ObjectForActive.gameObject, trainingBase2);
			}
			yield break;
		}

		private void Update()
		{
			if (this.trainingSkipped || this.trainingsQueue.Count == 0 || this.currentTraining != null || Mathf.Abs((float)DateTime.Now.Second - this.lastTrainingEndTime) < this.TimeBetweenTrainings)
			{
				return;
			}
			this.StartTraining(this.trainingsQueue[0]);
		}

		private void WatchedAtObject(GameObject obj, TrainingBase training)
		{
			if (this.trackedObjects.ContainsKey(obj))
			{
				return;
			}
			obj.AddComponent<TrainingObjectTracker>();
			this.trackedObjects.Add(obj, training);
		}

		private void ConfigureHelper(RectTransform element)
		{
			this.EmptyHelper.SetParent(element.parent, true);
			RectTransform rectTransform = (RectTransform)element.transform;
			this.EmptyHelper.localScale = Vector3.one;
			this.EmptyHelper.anchorMin = rectTransform.anchorMin;
			this.EmptyHelper.anchorMax = rectTransform.anchorMax;
			this.EmptyHelper.anchoredPosition = rectTransform.anchoredPosition;
			this.EmptyHelper.sizeDelta = rectTransform.sizeDelta;
			this.EmptyHelper.pivot = rectTransform.pivot;
		}

		public const string CompletedTrainingsArrayName = "CompletedTrainingsArray";

		public static TrainingManager.SkipTrainingDelegate SkipTrainingEvent;

		private static TrainingManager instance;

		public TrainingBase[] StartTrainings;

		public TrainingBase[] OnActiveTrainings;

		public float TimeToStartTrainings;

		public float TimeBetweenTrainings;

		[Separator("UI Links")]
		public GameObject TrainingPanel;

		public Text MessageText;

		public Text ToContinueMessageText;

		public RectTransform[] Pointers;

		public Canvas RootCanvas;

		public RectTransform EmptyHelper;

		private static List<string> completedTrainings = new List<string>();

		private readonly List<TrainingBase> trainingsQueue = new List<TrainingBase>();

		private readonly Dictionary<GameObject, TrainingBase> trackedObjects = new Dictionary<GameObject, TrainingBase>();

		private TrainingBase currentTraining;

		private Image pointerImage;

		private float lastTrainingEndTime;

		private bool trainingSkipped;

		public delegate void SkipTrainingDelegate(int completeTrainingCount);
	}
}
