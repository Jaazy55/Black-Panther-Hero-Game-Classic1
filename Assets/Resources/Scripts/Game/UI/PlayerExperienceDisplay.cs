using System;
using Game.Character;
using Game.Character.Stats;
using UnityEngine.UI;

namespace Game.UI
{
	public class PlayerExperienceDisplay : PlayerInfoDisplayBase
	{
		protected override PlayerInfoType GetInfoType()
		{
			return PlayerInfoType.Experience;
		}

		protected override void Display()
		{
			if (this.TextLink)
			{
				this.TextLink.text = this.InfoValue + "/" + LevelManager.Instance.ExpForNextLevel.ToString("#");
			}
			if (this.SliderLink)
			{
				this.SliderLink.maxValue = (float)LevelManager.Instance.ExpForNextLevel;
				this.SliderLink.value = (float)this.InfoValue;
			}
		}

		public Slider SliderLink;

		public Text TextLink;
	}
}
