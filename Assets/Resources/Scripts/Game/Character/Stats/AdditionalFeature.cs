using System;
using UnityEngine;

namespace Game.Character.Stats
{
	[Serializable]
	public class AdditionalFeature
	{
		public AdditionalFeature(Sprite sprite, string description)
		{
			this.sprite = sprite;
			this.description = description;
		}

		public Sprite GetSprite()
		{
			return this.sprite;
		}

		public string GetDescription()
		{
			return this.description;
		}

		[SerializeField]
		private Sprite sprite;

		[SerializeField]
		private string description;
	}
}
