using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	[Serializable]
	public class ShopLinks
	{
		public GameObject Categories;

		public GameObject Elements;

		public ShopInfoPanelManager InfoPanelManager;

		public Image PriceIcon;

		public Text Price;

		public GameObject BuyPanel;

		public GameObject ExchangePanel;

		public GameObject EquipButton;

		public GameObject BuyButton;

		public GameObject OpenExchangePanelButton;

		public Transform DialogPanelPlaceholder;

		public ShopDialogPanel[] DialogPanelPrefabs;

		public Image Background;
	}
}
