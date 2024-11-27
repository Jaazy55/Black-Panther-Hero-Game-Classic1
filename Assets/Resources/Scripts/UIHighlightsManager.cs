using System;
using System.Collections.Generic;
using Game.Items;
using Game.Shop;
using UnityEngine;

public class UIHighlightsManager : MonoBehaviour
{
	public static UIHighlightsManager Instance
	{
		get
		{
			return UIHighlightsManager.instance;
		}
	}

	public static bool InstanceExist
	{
		get
		{
			return !(UIHighlightsManager.instance == null);
		}
	}

	public void SetTargetItem(GameItem gameItem)
	{
		if (gameItem != null)
		{
			if (ShopManager.Instance != null)
			{
				ShopManager.Instance.GetShopItem(gameItem.ID, out this.m_TargetShopItem, out this.m_TargetShopCategory);
				foreach (ShopCategory shopCategory in ShopManager.Instance.GetShopCategores())
				{
					if (shopCategory.GameItem.Type == ItemsTypes.Money || shopCategory.GameItem.Type == ItemsTypes.Bonus || shopCategory == this.m_TargetShopCategory)
					{
						shopCategory.gameObject.SetActive(true);
						shopCategory.Container.SetActive(false);
					}
					else
					{
						shopCategory.gameObject.SetActive(false);
						shopCategory.Container.SetActive(false);
					}
				}
			}
		}
		else
		{
			if (ShopManager.Instance != null)
			{
				foreach (ShopCategory shopCategory2 in ShopManager.Instance.GetShopCategores())
				{
					shopCategory2.gameObject.SetActive(true);
				}
			}
			this.m_TargetShopCategory = null;
			this.m_TargetShopItem = null;
		}
	}

	public void ActivateShopButtonsHighlights(bool value)
	{
		foreach (GameObject gameObject in this.m_ShopButtons)
		{
			gameObject.SetActive(value);
		}
	}

	public void ActivateExitShopButtonsHighlights(bool value)
	{
		this.m_ExitShopButton.SetActive(value);
	}

	public void ActivateBuyShopButtonsHighlights(bool value)
	{
		this.m_BuyShopButton.SetActive(value);
	}

	private void ActivateDinamicHighLight(bool value)
	{
		this.m_DinamicHighLights.SetActive(value);
	}

	private void SetPositionDinamicHighLight(Vector3 pos)
	{
		this.m_DinamicHighLights.transform.position = pos;
	}

	private void Awake()
	{
		UIHighlightsManager.instance = this;
	}

	private void OnDestroy()
	{
		if (UIHighlightsManager.InstanceExist && UIHighlightsManager.instance == this)
		{
			UIHighlightsManager.instance = null;
		}
	}

	private void Update()
	{
		if (this.m_TargetShopItem != null && ShopManager.Instance != null && ShopManager.Instance.currentItem != null && ShopManager.Instance.Links.Categories.activeInHierarchy)
		{
			if (ShopManager.Instance.currentItem.GameItem.ID == this.m_TargetShopItem.GameItem.ID)
			{
				this.ActivateDinamicHighLight(false);
				this.ActivateBuyShopButtonsHighlights(true);
			}
			else
			{
				if (ShopManager.Instance.activeCategory != this.m_TargetShopCategory)
				{
					this.ActivateDinamicHighLight(true);
					this.SetPositionDinamicHighLight(this.m_TargetShopCategory.transform.position);
				}
				else if (ShopManager.Instance.currentItem != this.m_TargetShopItem)
				{
					this.ActivateDinamicHighLight(true);
					this.SetPositionDinamicHighLight(this.m_TargetShopItem.transform.position);
				}
				else
				{
					this.ActivateDinamicHighLight(false);
				}
				this.ActivateBuyShopButtonsHighlights(false);
			}
		}
		else
		{
			this.ActivateDinamicHighLight(false);
			this.ActivateBuyShopButtonsHighlights(false);
		}
	}

	private static UIHighlightsManager instance;

	[Separator("Highlights arrays")]
	[SerializeField]
	private List<GameObject> m_ShopButtons;

	[SerializeField]
	private GameObject m_ExitShopButton;

	[SerializeField]
	private GameObject m_BuyShopButton;

	[SerializeField]
	private GameObject m_DinamicHighLights;

	private ShopCategory m_TargetShopCategory;

	private ShopItem m_TargetShopItem;
}
