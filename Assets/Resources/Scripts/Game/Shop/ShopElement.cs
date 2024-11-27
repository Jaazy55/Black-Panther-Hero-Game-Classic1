using System;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopElement : MonoBehaviour
	{
		public virtual void OnClick()
		{
		}

		public virtual void SetUP()
		{
			this.Icon.sprite = this.GameItem.ShopVariables.ItemIcon;
		}

		public GameItem GameItem;

		public Image Back;

		public Image Icon;
	}
}
