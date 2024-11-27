using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.Stats
{
	public class StatText : CharacterStatDisplay
	{
		private Text Text
		{
			get
			{
				if (this.text == null)
				{
					this.text = base.GetComponent<Text>();
					if (this.text == null)
					{
						UnityEngine.Debug.LogError("Can't find Text");
						base.enabled = false;
					}
				}
				return this.text;
			}
		}

		protected override void UpdateDisplayValue()
		{
			if (this.Text != null)
			{
				this.Text.text = this.current.ToString("F0");
			}
		}

		public override void OnChanged(float amount)
		{
		}

		[SerializeField]
		private Text text;
	}
}
