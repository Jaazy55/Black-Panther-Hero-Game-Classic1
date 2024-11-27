using System;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.Managers.Shop.Prefabs
{
	public class StatViewVehicles : StatView
	{
		public void SetMaxAmount(float maxValue)
		{
			this.fillImageMaxValue = maxValue;
		}

		public override void SetValue(float value)
		{
			this.fillImage.fillAmount = value / this.fillImageMaxValue;
			this.value.text = string.Empty + value;
		}

		[SerializeField]
		private Image fillImage;

		private float fillImageMaxValue;
	}
}
