using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Game.Character.Superpowers;
using Game.GlobalComponent.Qwest;
using Game.Items;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopManager : MonoBehaviour
	{
		public static ShopManager Instance
		{
			get
			{
				ShopManager result;
				if ((result = ShopManager.instance) == null)
				{
					result = (ShopManager.instance = UnityEngine.Object.FindObjectOfType<ShopManager>());
				}
				return result;
			}
		}

		public static bool HasInstance
		{
			get
			{
				return ShopManager.instance != null;
			}
		}

		public static bool IsOpen
		{
			get
			{
				return ShopManager.instance.Links.Categories.activeInHierarchy;
			}
		}

		public static Dictionary<int, int> BoughtItemsCoins { get; private set; }

		public static Dictionary<int, int> BoughtItemsGems { get; private set; }

		private void Awake()
		{
			this.Init();
		}

		private void Init()
		{
			if (this.inited)
			{
				return;
			}
			ShopManager.instance = this;
			base.StartCoroutine("LoadPreviewRoom");
			ItemsManager.Instance.Init();
			StuffManager.Instance.Init();
			PlayerAbilityManager.LoadAbilities();
			PlayerAbilityManager.EnableEbilities();
			this.Links.InfoPanelManager.Init();
			this.LoadBI();
			SalesManager.Instance.Init();
			DailyBonusesManager.Instance.Init();
			this.inited = true;
			this.FillShopStuffDictionary();
		}

		public T GetShopItemByType<T>(ItemsTypes itemType, object[] parametrs) where T : GameItem
		{
			ShopCategory shopCategory;
			ShopItem shopItem;
			return this.GetShopItemByType<T>(itemType, parametrs, out shopCategory, out shopItem);
		}

		public T GetShopItemByType<T>(ItemsTypes itemType, object[] parametrs, out ShopCategory category, out ShopItem item) where T : GameItem
		{
			category = null;
			item = null;
			foreach (KeyValuePair<ShopCategory, List<ShopItem>> keyValuePair in this.ShopStuff[itemType])
			{
				foreach (ShopItem shopItem in keyValuePair.Value)
				{
					T t = shopItem.GameItem as T;
					if (t && shopItem.GameItem.SameParametrWithOther(parametrs))
					{
						category = keyValuePair.Key;
						item = shopItem;
						return t;
					}
				}
			}
			return (T)((object)null);
		}

		public List<ShopCategory> GetShopCategores()
		{
			List<ShopCategory> list = new List<ShopCategory>();
			foreach (KeyValuePair<ItemsTypes, Dictionary<ShopCategory, List<ShopItem>>> keyValuePair in this.ShopStuff)
			{
				foreach (KeyValuePair<ShopCategory, List<ShopItem>> keyValuePair2 in keyValuePair.Value)
				{
					list.Add(keyValuePair2.Key);
				}
			}
			return list;
		}

		public bool GetShopItem(int id, out ShopItem item, out ShopCategory category)
		{
			category = null;
			item = null;
			foreach (KeyValuePair<ItemsTypes, Dictionary<ShopCategory, List<ShopItem>>> keyValuePair in this.ShopStuff)
			{
				foreach (KeyValuePair<ShopCategory, List<ShopItem>> keyValuePair2 in keyValuePair.Value)
				{
					foreach (ShopItem shopItem in keyValuePair2.Value)
					{
						if (shopItem.GameItem.ID == id)
						{
							item = shopItem;
							category = keyValuePair2.Key;
							return true;
						}
					}
				}
			}
			return false;
		}

		public static void ClearBI(bool coinsOnly = true)
		{
			BaseProfile.ClearArray<int>("BoughtItemsKeys");
			if (coinsOnly)
			{
				return;
			}
			BaseProfile.ClearArray<int>("GemsBoughtItemsKeys");
		}

		public void Enable()
		{
			if (this.ShopOpeningEvent != null)
			{
				this.ShopOpeningEvent();
			}
			this.ChangeCategory(this.GetFirstCategory());
		}

		public void Disable()
		{
			if (this.ShopCloseningEvent != null)
			{
				this.ShopCloseningEvent();
			}
		}

		public bool BoughtAlredy(GameItem gameItem)
		{
			Dictionary<int, int> dictionary = (!gameItem.ShopVariables.gemPrice) ? ShopManager.BoughtItemsCoins : ShopManager.BoughtItemsGems;
			return dictionary.ContainsKey(gameItem.ID);
		}

		public bool BoughtAlredy(int gameItemID)
		{
			return this.BoughtAlredy(ItemsManager.Instance.GetItem(gameItemID));
		}

		public void ChangeCategory(ShopCategory category)
		{
			if (this.activeCategory != null)
			{
				this.activeCategory.Container.SetActive(false);
				this.activeCategory.Back.sprite = this.ShopIcons.ShopCategoryOff;
			}
			category.Container.SetActive(true);
			this.activeCategory = category;
			if (this.activeCategory != null)
			{
				this.activeCategory.Back.sprite = this.ShopIcons.ShopCategoryOn;
			}
			this.SelectItem(this.GetFirstItemInCategory(this.activeCategory));
		}

		public void SelectItem(ShopItem item)
		{
			bool flag = this.currentItem == item;
			if (this.currentItem != null)
			{
				this.currentItem.Back.sprite = this.ShopIcons.ShopButtonOff;
			}
			this.currentItem = item;
			if (this.currentItem != null)
			{
				this.currentItem.Back.sprite = this.ShopIcons.ShopItemOn;
				this.currentItem.GameItem.UpdateItem();
			}
			if (this.selected && flag && item.GameItem is GameItemSkin)
			{
				PreviewManager.Instance.ShowItem(item, true);
				this.selected = false;
			}
			else
			{
				PreviewManager.Instance.ShowItem(item, false);
				this.selected = true;
			}
			if (this.ShowDebug)
			{
				UnityEngine.Debug.LogFormat(this.currentItem, "item selected {0}", new object[]
				{
					this.currentItem.name
				});
			}
			if (this.currentItem.GameItem.ShopVariables.price == 0 && this.ItemAvailableForBuy(this.currentItem.GameItem))
			{
				this.Give(this.currentItem.GameItem, false);
				return;
			}
			this.UpdateInfo();
		}

		public void Buy()
		{
			this.Buy(null);
		}

		public void OpenExchangePanel()
		{
			if (this.currentItem.GameItem.ShopVariables.gemPrice)
			{
				this.OpenGemsPanel();
				return;
			}
			float num = SalesManager.GetSale(this.currentItem.GameItem.ID) / 100f;
			int num2 = Mathf.RoundToInt((float)this.currentItem.GameItem.ShopVariables.price - (float)this.currentItem.GameItem.ShopVariables.price * num);
			int num3 = num2 - PlayerInfoManager.Money;
			this.gems = Mathf.CeilToInt((float)num3 / 100f);
			this.money = this.gems * 100;
			if (PlayerInfoManager.Gems < this.gems)
			{
				this.OpenGemsPanel();
			}
			else
			{
				this.NeedMoneyText.text = "To buy this product you need " + num3 + " money more";
				this.MoneyText.text = this.money.ToString();
				this.GemText.text = this.gems.ToString();
				this.Links.ExchangePanel.SetActive(true);
			}
		}

		public void CloseExchangePanel()
		{
			this.Links.ExchangePanel.SetActive(false);
		}

		public void Exchange()
		{
			if (PlayerInfoManager.Gems < this.gems)
			{
				this.OpenGemsPanel();
			}
			else
			{
				this.ExchangeItem.ShopVariables.price = this.gems;
				this.ExchangeItem.BonusValue = this.money;
				this.Buy(this.ExchangeItem);
			}
		}

		public void Buy(GameItem item)
		{
			if (item == null)
			{
				item = this.currentItem.GameItem;
			}
			if (this.ShowDebug)
			{
				UnityEngine.Debug.LogFormat(item, "try buy item {0}", new object[]
				{
					item.name
				});
			}
			float num = SalesManager.GetSale(item.ID) / 100f;
			int num2 = Mathf.RoundToInt((float)item.ShopVariables.price - (float)item.ShopVariables.price * num);
			if (item.ShopVariables.gemPrice)
			{
				PlayerInfoManager.Gems -= num2;
			}
			else
			{
				PlayerInfoManager.Money -= num2;
				PlayerInfoManager.Instance.AddSpendMoney(-num2);
			}
			GameEventManager.Instance.Event.GetShopEvent();
			this.Give(item, true);
		}

		public void Give(GameItem item, bool onBuy = false)
		{
			Dictionary<int, int> dictionary;
			if (item.ShopVariables.gemPrice)
			{
				dictionary = ShopManager.BoughtItemsGems;
			}
			else
			{
				dictionary = ShopManager.BoughtItemsCoins;
			}
			if (dictionary.ContainsKey(item.ID))
			{
				if (item.ShopVariables.MaxAmount == 1)
				{
					return;
				}
				Dictionary<int, int> dictionary2= dictionary;
				int id= item.ID;
				(dictionary2 )[id ] = dictionary2[id] + item.ShopVariables.PerStackAmount;
			}
			else
			{
				dictionary.Add(item.ID, item.ShopVariables.PerStackAmount);
			}
			if (item.ShopVariables.InstantEquip || StuffManager.Instance.CanEquipInstantly(item, onBuy))
			{
				this.Equip(item, true);
			}
			item.OnBuy();
			this.SavePlayerInfo();
			this.UpdateInfo();
		}

		public void Equip()
		{
			this.Equip(null, false);
		}

		public void Equip(GameItem item, bool equipOnly = false)
		{
			if (item == null)
			{
				item = this.currentItem.GameItem;
			}
			StuffManager.Instance.EquipItem(item, equipOnly);
			this.UpdateInfo();
		}

		public void OpenDialogPanel(GameItem item)
		{
			GameObject gameObject = null;
			foreach (ShopDialogPanel shopDialogPanel in this.Links.DialogPanelPrefabs)
			{
				bool flag = false;
				foreach (ItemsTypes itemsTypes in shopDialogPanel.DialogPanelTypes)
				{
					if (itemsTypes == item.Type)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					gameObject = shopDialogPanel.gameObject;
					break;
				}
			}
			if (gameObject != null)
			{
				this.Links.DialogPanelPlaceholder.gameObject.SetActive(true);
				this.currDialogPanel = gameObject.GetComponent<ShopDialogPanel>();
				gameObject.SetActive(true);
				this.UpdateDialogPanel(item);
			}
		}

		public void CloseDialogPanel()
		{
			this.currDialogPanel.gameObject.SetActive(false);
			this.Links.DialogPanelPlaceholder.gameObject.SetActive(false);
		}

		public void UpdateDialogPanel(GameItem item = null)
		{
			if (this.currDialogPanel != null)
			{
				this.currDialogPanel.UpdatePanel(item);
			}
		}

		public void OpenGemsPanel()
		{
			this.Links.DialogPanelPlaceholder.gameObject.SetActive(true);
			this.currDialogPanel = this.BuyGemsPanel.GetComponent<ShopDialogPanel>();
			this.BuyGemsPanel.SetActive(true);
			this.UpdateDialogPanel(this.currentItem.GameItem);
		}

		public void CloseGemsPanel()
		{
			this.CloseDialogPanel();
		}

		private void FixPanelRect(Transform targetTransform)
		{
			RectTransform rectTransform = targetTransform as RectTransform;
			if (rectTransform == null)
			{
				return;
			}
			rectTransform.localScale = Vector3.one;
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			RectTransform rectTransform2 = rectTransform;
			Vector2 zero = Vector2.zero;
			rectTransform.offsetMax = zero;
			rectTransform2.offsetMin = zero;
		}

		public void DeleteFromBI(int ID, bool fromGems = false)
		{
			if (fromGems)
			{
				ShopManager.BoughtItemsGems.Remove(ID);
			}
			else
			{
				ShopManager.BoughtItemsCoins.Remove(ID);
			}
			this.SaveBI();
		}

		public int GetBIValue(int ID, bool inGems = false)
		{
			if (!this.inited)
			{
				this.LoadBI();
			}
			int result;
			if (inGems)
			{
				ShopManager.BoughtItemsGems.TryGetValue(ID, out result);
			}
			else
			{
				ShopManager.BoughtItemsCoins.TryGetValue(ID, out result);
			}
			return result;
		}

		public void SetBIValue(int ID, int value, bool inGems = false)
		{
			if (inGems)
			{
				if (ShopManager.BoughtItemsGems.ContainsKey(ID))
				{
					ShopManager.BoughtItemsGems[ID] = value;
				}
				else
				{
					ShopManager.BoughtItemsGems.Add(ID, value);
				}
			}
			else if (ShopManager.BoughtItemsCoins.ContainsKey(ID))
			{
				ShopManager.BoughtItemsCoins[ID] = value;
			}
			else
			{
				ShopManager.BoughtItemsCoins.Add(ID, value);
			}
			this.SaveBI();
		}

		public void GenerateUI()
		{
			this.ClearCategories();
			this.ClearElements();
			ItemsManager.Instance.AssembleGameitems();
			foreach (int key in ItemsManager.Instance.Items.Keys)
			{
				if (!ItemsManager.Instance.Items[key].ShopVariables.HideInShop)
				{
					if (ItemsManager.Instance.Items[key].ShopVariables.isDivision)
					{
						ShopCategory shopCategory = this.CreateShopCategory(ItemsManager.Instance.Items[key]);
						foreach (GameItem gameItem in ItemsManager.Instance.Items[key].GetComponentsInChildren<GameItem>())
						{
							if (!gameItem.ShopVariables.isDivision && !gameItem.ShopVariables.HideInShop)
							{
								this.CreateShopItem(gameItem, shopCategory.Container);
							}
						}
					}
				}
			}
			UnityEngine.Debug.Log("Shop UI is generated");
		}

		public void UpdateInfo()
		{
			if (this.currentItem == null)
			{
				return;
			}
			bool flag = this.ItemAvailableForBuy(this.currentItem.GameItem);
			bool alreadyEquiped = StuffManager.AlredyEquiped(this.currentItem.GameItem);
			bool flag2 = this.currentItem.GameItem.ShopVariables.Hider != null && this.currentItem.GameItem.ShopVariables.Hider.IsHide;
			this.ManageBuyPanel(!flag2 && flag, this.EnoughMoneyToBuyItem(this.currentItem.GameItem));
			this.ManageEquipButton(!flag2 && (!flag & this.ItemAvailableForEquip(this.currentItem.GameItem)), alreadyEquiped);
			this.Links.InfoPanelManager.ShowItemInfo(this.currentItem.GameItem, this.BoughtAlredy(this.currentItem.GameItem));
		}

		public bool ItemAvailableForBuy(GameItem item)
		{
			if (!this.BoughtAlredy(item))
			{
				return item.ShopVariables.playerLvl <= PlayerInfoManager.Level && item.ShopVariables.VipLvl <= PlayerInfoManager.VipLevel && item.CanBeBought;
			}
			return (item.ShopVariables.MaxAmount == 0 || (item.ShopVariables.MaxAmount > 1 & this.GetBIValue(item.ID, false) + item.ShopVariables.PerStackAmount < item.ShopVariables.MaxAmount)) && item.CanBeBought;
		}

		public bool EnoughMoneyToBuyItem(GameItem item)
		{
			float num = SalesManager.GetSale(item.ID) / 100f;
			int num2 = (int)((float)item.ShopVariables.price - (float)item.ShopVariables.price * num);
			if (item.ShopVariables.gemPrice)
			{
				if (PlayerInfoManager.Gems < num2)
				{
					return false;
				}
			}
			else if (PlayerInfoManager.Money < num2)
			{
				return false;
			}
			return true;
		}

		public bool ItemAvailableForEquip(GameItem item)
		{
			return (!item.ShopVariables.InstantEquip & this.BoughtAlredy(item)) && item.CanBeEquiped;
		}

		public void JumpToMoneyCategory()
		{
			ShopCategory category;
			ShopItem item;
			GameItemBonus shopItemByType = this.GetShopItemByType<GameItemBonus>(ItemsTypes.Bonus, new object[]
			{
				BonusTypes.Money
			}, out category, out item);
			this.ChangeCategory(category);
			this.SelectItem(item);
		}

		public void JumpToCategory<T>(ItemsTypes itemsTypes) where T : GameItem
		{
			ShopCategory category;
			ShopItem item;
			this.GetShopItemByType<T>(itemsTypes, new object[0], out category, out item);
			this.ChangeCategory(category);
			this.SelectItem(item);
		}

		private void SavePlayerInfo()
		{
			if (this.ShowDebug)
			{
				UnityEngine.Debug.Log("Saving player info");
			}
			this.SaveBI();
		}

		private void SaveBI()
		{
			BaseProfile.StoreArray<int>(ShopManager.BoughtItemsCoins.Keys.ToArray<int>(), "BoughtItemsKeys");
			BaseProfile.StoreArray<int>(ShopManager.BoughtItemsCoins.Values.ToArray<int>(), "BoughtItemsValues");
			BaseProfile.StoreArray<int>(ShopManager.BoughtItemsGems.Keys.ToArray<int>(), "GemsBoughtItemsKeys");
			BaseProfile.StoreArray<int>(ShopManager.BoughtItemsGems.Values.ToArray<int>(), "GemsBoughtItemsValues");
		}

		private void LoadBI()
		{
			int[] array = BaseProfile.ResolveArray<int>("BoughtItemsKeys");
			int[] array2 = BaseProfile.ResolveArray<int>("BoughtItemsValues");
			int[] array3 = BaseProfile.ResolveArray<int>("GemsBoughtItemsKeys");
			int[] array4 = BaseProfile.ResolveArray<int>("GemsBoughtItemsValues");
			ShopManager.BoughtItemsCoins = new Dictionary<int, int>();
			ShopManager.BoughtItemsGems = new Dictionary<int, int>();
			if (array != null && array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					ShopManager.BoughtItemsCoins.Add(array[i], array2[i]);
				}
			}
			if (array3 != null && array3.Length > 0)
			{
				for (int j = 0; j < array3.Length; j++)
				{
					ShopManager.BoughtItemsGems.Add(array3[j], array4[j]);
				}
			}
		}

		private IEnumerator LoadPreviewRoom()
		{
			AsyncOperation loading = SceneManager.LoadSceneAsync("ShopRoom", LoadSceneMode.Additive);
			yield return loading;
			yield break;
		}

		private ShopCategory GetFirstCategory()
		{
			return this.Links.Categories.GetComponentInChildren<ShopCategory>();
		}

		private void ClearPlaceholder(Transform placeholder)
		{
			if (placeholder == null)
			{
				return;
			}
			for (int i = 0; i < placeholder.childCount; i++)
			{
				UnityEngine.Object.Destroy(placeholder.GetChild(i).gameObject);
			}
		}

		private void ClearCategories()
		{
			int childCount = this.Links.Categories.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				UnityEngine.Object.Destroy(this.Links.Categories.transform.GetChild(i).gameObject);
			}
		}

		private void ClearElements()
		{
			int childCount = this.Links.Elements.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				UnityEngine.Object.Destroy(this.Links.Elements.transform.GetChild(i).gameObject);
			}
		}

		private ShopItem GetFirstItemInCategory(ShopCategory category)
		{
			return category.Container.GetComponentInChildren<ShopItem>();
		}

		private ShopCategory CreateShopCategory(GameItem item)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.BlankCategory, this.Links.Categories.transform);
			gameObject.name = item.name;
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.localScale = Vector3.one;
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.BlankContainer, this.Links.Elements.transform);
			RectTransform component2 = gameObject2.GetComponent<RectTransform>();
			RectTransform component3 = gameObject2.transform.parent.GetComponent<RectTransform>();
			gameObject2.name = item.name;
			component2.anchoredPosition = component3.anchoredPosition;
			component2.sizeDelta = component3.sizeDelta;
			component2.eulerAngles = component3.eulerAngles;
			component2.localScale = Vector3.one;
			ShopCategory component4 = gameObject.GetComponent<ShopCategory>();
			component4.GameItem = item;
			component4.Container = gameObject2;
			component4.SetUP();
			return component4;
		}

		private ShopItem CreateShopItem(GameItem item, GameObject container)
		{
			Transform parent = container.transform.Find("Viewport/Content");
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.BlankElement, parent);
			gameObject.name = item.name;
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.localScale = Vector3.one;
			ShopItem component2 = gameObject.GetComponent<ShopItem>();
			component2.GameItem = item;
			component2.SetUP();
			return component2;
		}

		private void ManageEquipButton(bool interactable, bool alreadyEquiped)
		{
			this.Links.EquipButton.GetComponent<Image>().sprite = ((!alreadyEquiped) ? this.ShopIcons.Equip : this.ShopIcons.Unequip);
			this.Links.EquipButton.SetActive(interactable);
		}

		private void ManageBuyPanel(bool itemIsAvailableForBuy, bool enoughtMoney)
		{
			this.Links.BuyPanel.SetActive(itemIsAvailableForBuy);
			this.Links.BuyButton.SetActive(enoughtMoney);
			this.Links.OpenExchangePanelButton.SetActive(!enoughtMoney);
			this.Links.Price.color = ((!enoughtMoney) ? this.ShopIcons.NotAvailablePriceColor : this.ShopIcons.AvailablePriceColor);
			float num = (float)this.currentItem.GameItem.ShopVariables.price * SalesManager.GetSale(this.currentItem.GameItem.ID) / 100f;
			this.Links.Price.text = ((int)((float)this.currentItem.GameItem.ShopVariables.price - num)).ToString();
			this.Links.PriceIcon.sprite = ((!this.currentItem.GameItem.ShopVariables.gemPrice) ? this.ShopIcons.Money : this.ShopIcons.Gems);
		}

		private void ApplyPrefab(GameObject go)
		{
		}

		private Transform getProtoParent(Transform incTransform)
		{
			while (incTransform.parent != null)
			{
				incTransform = incTransform.parent;
			}
			return incTransform;
		}

		private void FillShopStuffDictionary()
		{
			for (int i = 0; i < this.Links.Categories.transform.childCount; i++)
			{
				Transform child = this.Links.Categories.transform.GetChild(i);
				ShopCategory component = child.GetComponent<ShopCategory>();
				if (!this.ShopStuff.ContainsKey(component.GameItem.Type))
				{
					this.ShopStuff.Add(component.GameItem.Type, new Dictionary<ShopCategory, List<ShopItem>>());
				}
				if (!this.ShopStuff[component.GameItem.Type].ContainsKey(component))
				{
					this.ShopStuff[component.GameItem.Type].Add(component, new List<ShopItem>());
				}
				Transform child2 = component.Container.transform.GetChild(0).GetChild(0);
				for (int j = 0; j < child2.childCount; j++)
				{
					child = child2.GetChild(j);
					ShopItem component2 = child.GetComponent<ShopItem>();
					this.ShopStuff[component.GameItem.Type][component].Add(component2);
				}
			}
		}

		private const string BIKeysArrayName = "BoughtItemsKeys";

		private const string BIValuesArrayName = "BoughtItemsValues";

		private const string GemsPrefix = "Gems";

		private static ShopManager instance;

		public bool ShowDebug;

		[Space(10f)]
		public ShopLinks Links;

		[Space(10f)]
		public ShopIcons ShopIcons;

		[Space(10f)]
		public GameObject GameItemsHierarhy;

		[Space(10f)]
		public GameObject BlankElement;

		public GameObject BlankCategory;

		public GameObject BlankContainer;

		[Space(10f)]
		public GameObject BuyGemsPanel;

		[Space(10f)]
		public ShopCategory activeCategory;

		public ShopItem currentItem;

		public ShopManager.ShopOpened ShopOpeningEvent;

		public ShopManager.ShopClosed ShopCloseningEvent;

		[Space(10f)]
		public Text NeedMoneyText;

		public Text MoneyText;

		public Text GemText;

		public GameItemBonus ExchangeItem;

		private bool inited;

		private bool selected;

		private ShopDialogPanel currDialogPanel;

		private Dictionary<ItemsTypes, Dictionary<ShopCategory, List<ShopItem>>> ShopStuff = new Dictionary<ItemsTypes, Dictionary<ShopCategory, List<ShopItem>>>();

		private int gems;

		private int money;

		public delegate void ShopOpened();

		public delegate void ShopClosed();
	}
}
