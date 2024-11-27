using System;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.Managers.Shop.Prefabs
{
	public class AdditionalFeatureVehicles : MonoBehaviour
	{
		public void SetImage(Sprite sprite)
		{
			this.image.sprite = sprite;
		}

		public void SetDescription(string description)
		{
			this.description.text = string.Empty + description;
		}

		[SerializeField]
		private Image image;

		[SerializeField]
		private Text description;
	}
}
