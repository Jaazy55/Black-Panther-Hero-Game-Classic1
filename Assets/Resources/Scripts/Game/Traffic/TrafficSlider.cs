using System;
using Game.GlobalComponent.Quality;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Traffic
{
	public class TrafficSlider : MonoBehaviour
	{
		public static void UpdteValue()
		{
			if (TrafficSlider.DensitySlider)
			{
				TrafficSlider.DensitySlider.value = (float)QualityManager.CountPedestrians;
			}
		}

		private void Awake()
		{
			TrafficSlider.DensitySlider = base.GetComponent<Slider>();
		}

		private void Start()
		{
			TrafficSlider.UpdteValue();
		}

		public void ChangeDensity(float density)
		{
			QualityManager.SetCountPedestrians((int)density);
			QualityManager.SetCountVehicles((int)density);
		}

		public static Slider DensitySlider;
	}
}
