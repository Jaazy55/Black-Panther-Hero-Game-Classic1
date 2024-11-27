using System;
using System.Collections.Generic;
using Game.Character;
using Game.GlobalComponent;
using Game.Shop;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonusesManager : MonoBehaviour
{
	public static DailyBonusesManager Instance
	{
		get
		{
			DailyBonusesManager result;
			if ((result = DailyBonusesManager.instance) == null)
			{
				result = (DailyBonusesManager.instance = UnityEngine.Object.FindObjectOfType<DailyBonusesManager>());
			}
			return result;
		}
	}

	public bool SomeBonusAvailable
	{
		get
		{
			return TimeManager.AnotherDay(this.bonusReceivingTime);
		}
	}

	private void Awake()
	{
		this.Init();
	}

	private void Start()
	{
		if (!this.inited)
		{
			return;
		}
		if (TimeManager.AnotherDay(this.bonusReceivingTime))
		{
			this.ShowBonusesPanel();
		}
	}

	public void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.currentBonusIndex = BaseProfile.ResolveValue<int>("BonusIndex", 0);
		this.todayBonusIndex = this.currentBonusIndex;
		this.bonusReceivingTime = BaseProfile.ResolveValue<long>("BonusReceiving", 0L);
		this.inited = true;
	}

	private void ShowBonusInfo(int bonusID)
	{
		Dictionary<string, Sprite> dictionary = new Dictionary<string, Sprite>();
		if (this.Bonuses[bonusID].Gems != 0)
		{
			dictionary.Add("Gems: " + this.Bonuses[bonusID].Gems, ShopManager.Instance.ShopIcons.Gems);
		}
		if (this.Bonuses[bonusID].Money != 0)
		{
			dictionary.Add("Money: " + this.Bonuses[bonusID].Money, ShopManager.Instance.ShopIcons.Money);
		}
		if (this.Bonuses[bonusID].Item != null)
		{
			dictionary.Add(this.Bonuses[bonusID].Item.ShopVariables.Name, this.Bonuses[bonusID].Item.ShopVariables.ItemIcon);
		}
		this.ShowBonusReward(dictionary, bonusID == this.currentBonusIndex);
	}

	public DailyBonus GetTodayBonus()
	{
		return this.Bonuses[this.todayBonusIndex];
	}

	public void ProceedCurrentBonus()
	{
		this.ProceedBonus(this.GetTodayBonus());
		this.currentBonusIndex = this.todayBonusIndex + 1;
		if (this.currentBonusIndex >= this.Bonuses.Length)
		{
			this.currentBonusIndex = 0;
		}
		this.bonusReceivingTime = DateTime.Today.ToFileTime();
		BaseProfile.StoreValue<int>(this.currentBonusIndex, "BonusIndex");
		BaseProfile.StoreValue<long>(this.bonusReceivingTime, "BonusReceiving");
		this.HideBonusesPanel();
	}

	private void ProceedBonus(DailyBonus bonus)
	{
		PlayerInfoManager.Money += bonus.Money;
		PlayerInfoManager.Gems += bonus.Gems;
		if (bonus.Item != null)
		{
			ShopManager.Instance.Give(bonus.Item, false);
		}
	}

	private void ShowBonusesPanel()
	{
		this.m_Panel.SetActive(true);
		this.GenerateBonusList();
	}

	private void HideBonusesPanel()
	{
		this.m_Panel.SetActive(false);
	}

	private void GenerateBonusList()
	{
		for (int i = 0; i < this.Bonuses.Length; i++)
		{
			DailyBonusUIButton component = UnityEngine.Object.Instantiate<GameObject>(this.EmptyBonus, this.BonusListRect.content.transform).GetComponent<DailyBonusUIButton>();
			component.transform.localScale = Vector3.one;
			component.BonusID = i;
			if (i >= this.currentBonusIndex)
			{
				component.Image.sprite = this.Bonuses[i].Icon;
			}
			else
			{
				component.Image.sprite = this.Bonuses[i].ReceivedIcon;
			}
		}
		this.SelectBonusUIButton(this.BonusButtonByIndex(this.currentBonusIndex));
	}

	public void SelectBonusUIButton(DailyBonusUIButton bonusButton)
	{
		if (this.currentSelected != null)
		{
			this.currentSelected.transform.localScale = Vector3.one;
		}
		this.currentSelected = bonusButton;
		this.currentSelected.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
		this.ShowBonusInfo(bonusButton.BonusID);
	}

	private DailyBonusUIButton BonusButtonByIndex(int index)
	{
		return this.BonusListRect.content.GetChild(index).GetComponent<DailyBonusUIButton>();
	}

	private void ShowBonusReward(Dictionary<string, Sprite> output, bool isAvailable)
	{
		this.ClearRewardContainer();
		foreach (KeyValuePair<string, Sprite> keyValuePair in output)
		{
			GameObject fromPool = PoolManager.Instance.GetFromPool(this.InfoPrimitivePrefab);
			fromPool.transform.parent = this.ContentContainer.transform;
			BonusInfoPrimitive component = fromPool.GetComponent<BonusInfoPrimitive>();
			component.transform.localScale = Vector3.one;
			component.InfoText.text = keyValuePair.Key;
			component.IconImage.sprite = keyValuePair.Value;
		}
	}

	private void ClearRewardContainer()
	{
		int childCount = this.ContentContainer.transform.childCount;
		GameObject[] array = new GameObject[childCount];
		for (int i = 0; i < childCount; i++)
		{
			array[i] = this.ContentContainer.transform.GetChild(i).gameObject;
		}
		foreach (GameObject o in array)
		{
			PoolManager.Instance.ReturnToPool(o);
		}
	}

	private const string bonusReceivingTimeKey = "BonusReceiving";

	private const string currentBonusIndexKey = "BonusIndex";

	private static DailyBonusesManager instance;

	public DailyBonus[] Bonuses;

	[Space(10f)]
	[SerializeField]
	private GameObject m_Panel;

	public ScrollRect BonusListRect;

	public GameObject EmptyBonus;

	public GameObject InfoPrimitivePrefab;

	public GameObject GetBonusButton;

	public GameObject ContentContainer;

	private int currentBonusIndex;

	private int todayBonusIndex;

	private long bonusReceivingTime;

	private DailyBonusUIButton currentSelected;

	private bool inited;
}
