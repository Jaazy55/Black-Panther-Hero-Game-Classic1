using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopItem : ShopElement
	{
		private void Awake()
		{
			this.CheckSale();
		}

		public override void OnClick()
		{
			ShopManager.Instance.SelectItem(this);
		}

		private void CheckSale()
		{
			//Sprite sprite;
			//float num = (float)SalesManager.Instance.GetSale(this.GameItem.ID, out sprite);
			//if (num > 0f)
			//{
			//	this.SaleImage.sprite = sprite;
			//	this.SaleImage.gameObject.SetActive(true);
			//}
			//else
			//{
			//	this.SaleImage.gameObject.SetActive(false);
			//}
		}

		public Image SaleImage;
	}
}
