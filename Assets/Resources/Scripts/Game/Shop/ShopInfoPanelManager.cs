using System;
using Game.Character;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopInfoPanelManager : MonoBehaviour
	{
		public static ShopInfoPanelManager Instance
		{
			get
			{
				if (ShopInfoPanelManager.instance == null)
				{
					ShopInfoPanelManager.instance = UnityEngine.Object.FindObjectOfType<ShopInfoPanelManager>();
				}
				return ShopInfoPanelManager.instance;
			}
		}

		public void Init()
		{
			if (this.inited)
			{
				return;
			}
			ShopInfoPanelManager.instance = this;
			this.currInfoPanel = this.DefaultInfoPanelPrefab;
			this.currPanelGO = this.currInfoPanel.gameObject;
			this.currPanelGO.SetActive(true);
			this.inited = true;
		}

		public void ShowItemInfo(GameItem item, bool isAvailable)
		{
			ItemsTypes type = item.Type;
			if (type != this.currInfoPanel.Type)
			{
				this.currPanelGO.SetActive(false);
				this.currInfoPanel = this.GetPanelByType(type);
				this.currPanelGO = this.currInfoPanel.gameObject;
				this.currPanelGO.SetActive(true);
			}
			this.currInfoPanel.ShowInfo(item);
			this.ShowBlockedItemPanel(item, isAvailable);
		}

		public void JumpToVIP()
		{
			ShopManager.Instance.ChangeCategory(this.currentItemCategory);
			ShopManager.Instance.SelectItem(this.currentShopItem);
		}

		private void ShowBlockedItemPanel(GameItem item, bool isAvailable)
		{
			bool flag = item.ShopVariables.Hider != null && item.ShopVariables.Hider.IsHide;
			if (isAvailable && !flag)
			{
				this.BlockedItemPanel.SetActive(false);
				return;
			}
			bool active = false;
			if (flag)
			{
				active = true;
				this.VIPImage.gameObject.SetActive(false);
				this.LvlGO.SetActive(false);
			}
			else
			{
				if (item.ShopVariables.VipLvl > PlayerInfoManager.VipLevel)
				{
					ShopManager.Instance.GetShopItemByType<GameItemBonus>(ItemsTypes.Bonus, new object[]
					{
						BonusTypes.VIP,
						item.ShopVariables.VipLvl
					}, out this.currentItemCategory, out this.currentShopItem);
					this.VIPImage.sprite = this.currentShopItem.Icon.sprite;
					this.VIPImage.gameObject.SetActive(true);
					active = true;
				}
				else
				{
					this.VIPImage.gameObject.SetActive(false);
				}
				if (item.ShopVariables.playerLvl > PlayerInfoManager.Level)
				{
					active = true;
				}
				this.LvlGO.SetActive(true);
				this.LvlText.text = item.ShopVariables.playerLvl.ToString();
			}
			this.BlockedItemPanel.SetActive(active);
		}

		private ShopInfoPanel GetPanelByType(ItemsTypes neededType)
		{
			foreach (ShopInfoPanel shopInfoPanel in this.InfoPanelsPrefabs)
			{
				if (shopInfoPanel.Type == neededType)
				{
					return shopInfoPanel;
				}
			}
			return this.DefaultInfoPanelPrefab;
		}

		private void FixPanelRect()
		{
			RectTransform component = this.currPanelGO.GetComponent<RectTransform>();
			component.localScale = Vector3.one;
			component.pivot = new Vector2(0.5f, 0.5f);
			RectTransform rectTransform = component;
			Vector2 zero = Vector2.zero;
			component.offsetMax = zero;
			rectTransform.offsetMin = zero;
		}

		public ShopInfoPanel DefaultInfoPanelPrefab;

		public ShopInfoPanel[] InfoPanelsPrefabs;

		[Space(10f)]
		public Color ReqSatisfiedColor = Color.white;

		public Color ReqNotSatisfiedColor = Color.red;

		public GameObject BlockedItemPanel;

		public Image VIPImage;

		public GameObject LvlGO;

		public Text LvlText;

		private static ShopInfoPanelManager instance;

		private GameObject currPanelGO;

		private ShopInfoPanel currInfoPanel;

		private ShopCategory currentItemCategory;

		private ShopItem currentShopItem;

		private bool inited;
	}
}
