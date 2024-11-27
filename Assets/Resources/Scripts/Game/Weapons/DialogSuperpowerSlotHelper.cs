using System;
using UnityEngine;

namespace Game.Weapons
{
	public class DialogSuperpowerSlotHelper : DialogSlotHelper
	{
		public override void UpdateSlot(bool IsAvailable, Sprite sprite, bool highlighted)
		{
			this.CheckItem();
			base.UpdateSlot(IsAvailable, sprite, highlighted);
		}

		private void CheckItem()
		{
		}

		[Space(10f)]
		public bool Active;
	}
}
