using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Quality
{
	public class QualitySlider : MonoBehaviour
	{
		private void Awake()
		{
			this.slider = base.GetComponentInChildren<Slider>();
			if (this.slider)
			{
				this.slider.maxValue = 650f;
				this.slider.minValue = 100f;
			}
			QualityManager.updateQuality = (QualityManager.UpdateQuality)Delegate.Combine(QualityManager.updateQuality, new QualityManager.UpdateQuality(this.UpdateSlider));
		}

		private void OnDestroy()
		{
			QualityManager.updateQuality = (QualityManager.UpdateQuality)Delegate.Remove(QualityManager.updateQuality, new QualityManager.UpdateQuality(this.UpdateSlider));
		}

		private void OnEnable()
		{
			this.UpdateSlider();
		}

		public void ChangeValue(float value)
		{
			if (this.ApplyNow)
			{
				QualityManager.Instance.ChangeCameraCliping(value);
			}
			else
			{
				QualityManager.FarClipPlane = (int)value;
			}
		}

		public void UpdateSlider()
		{
			if (this.slider)
			{
				this.slider.value = (float)QualityManager.FarClipPlane;
			}
		}

		public bool ApplyNow;

		private Slider slider;
	}
}
