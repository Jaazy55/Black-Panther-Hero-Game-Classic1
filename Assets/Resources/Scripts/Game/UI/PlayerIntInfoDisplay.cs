using System;
using Game.Character;
using UnityEngine.UI;

namespace Game.UI
{
	public class PlayerIntInfoDisplay : PlayerInfoDisplayBase
	{
		protected override PlayerInfoType GetInfoType()
		{
			return this.InfoType;
		}

		protected override void Display()
		{
			if (this.TextLink)
			{
				this.TextLink.text = this.BeforeText + this.InfoValue;
			}
		}

		public PlayerInfoType InfoType;

		public Text TextLink;

		public string BeforeText;
	}
}
