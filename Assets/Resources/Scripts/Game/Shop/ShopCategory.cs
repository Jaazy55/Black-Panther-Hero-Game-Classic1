using System;
using UnityEngine;

namespace Game.Shop
{
	public class ShopCategory : ShopElement
	{
		public override void OnClick()
		{
			ShopManager.Instance.ChangeCategory(this);
		}

		public override void SetUP()
		{
			base.SetUP();
			this.Container.SetActive(false);
		}

		public GameObject Container;
	}
}
