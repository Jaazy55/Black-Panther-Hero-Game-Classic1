using System;
using Game.Character;
using Game.Shop;
using UnityEngine;
using UnityEngine.UI;

public class SyncStoredData : MonoBehaviour
{
	private void Awake()
	{
		this.WeaponsNames = Enum.GetNames(typeof(WeaponNameList));
	}

	private void Start()
	{
		if (!this.work)
		{
			return;
		}
		if (this.UserHaventActualVersionKey() && this.HaveDataForSave())
		{
			this.ClearAllPrefs();
			this.UpdateData();
		}
		base.Invoke("DisablePanel", 5f);
	}

	private void DisablePanel()
	{
		this.Panel.SetActive(false);
	}

	private bool HaveDataForSave()
	{
		this.currentMoney = PlayerInfoManager.Money;
		this.currentExp = PlayerInfoManager.Experience;
		this.currentSP = PlayerInfoManager.UpgradePoints;
		foreach (string weaponName in this.WeaponsNames)
		{
			if (PlayerStoreProfile.Instance.GetOldWeapon(weaponName))
			{
				this.currentMoney += 3000;
			}
		}
		return this.currentMoney != 0 || this.currentExp != 0;
	}

	private void UpdateData()
	{
		this.ConvertStats();
		this.AddVersionKey();
		this.ShowMessage();
	}

	private void ConvertStats()
	{
		this.newMoney = (int)((float)this.currentMoney * this.MoneyExchangeRate);
		this.newExp = (int)((float)this.currentExp * this.ExpExchangeRate);
		PlayerInfoManager.Money = this.newMoney;
		PlayerInfoManager.Experience = this.newExp;
		PlayerInfoManager.UpgradePoints = this.currentSP;
	}

	private bool UserHaventActualVersionKey()
	{
		if (!PlayerPrefs.HasKey("VersionKey"))
		{
			return true;
		}
		string @string = PlayerPrefs.GetString("VersionKey");
		if (@string == this.VersionKey)
		{
			this.debug.text = "User already have version actual key";
			return false;
		}
		return true;
	}

	private void ClearAllPrefs()
	{
		BaseProfile.ClearBaseProfileWithoutSystemSettings();
	}

	private void AddVersionKey()
	{
		PlayerPrefs.SetString("VersionKey", this.VersionKey);
	}

	private void ShowMessage()
	{
		this.debug.text = string.Format("old money {0}, new money {1} \n old exp {2}, new exp {3}", new object[]
		{
			this.currentMoney,
			this.newMoney,
			this.currentExp,
			this.newExp
		});
	}

	private const string VersionKeyHesh = "VersionKey";

	public bool work;

	public string VersionKey;

	public float MoneyExchangeRate;

	public float ExpExchangeRate;

	public Text debug;

	public GameObject Panel;

	private int currentMoney;

	private int currentSP;

	private int currentExp;

	private int newMoney;

	private int newExp;

	private string[] WeaponsNames;
}
