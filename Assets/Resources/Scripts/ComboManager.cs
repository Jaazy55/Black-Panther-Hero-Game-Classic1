using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.GlobalComponent.Quality;
using Game.Shop;
using Game.Vehicle;
using Game.Weapons;
using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
	public static ComboManager Instance
	{
		get
		{
			if (ComboManager.instance == null)
			{
				ComboManager.instance = UnityEngine.Object.FindObjectOfType<ComboManager>();
			}
			return ComboManager.instance;
		}
	}

	public ComboManager.ComboInfo WeaponComboInfo
	{
		get
		{
			return this.weaponComboInfo;
		}
	}

	public ComboManager.ComboInfo OverallComboInfo
	{
		get
		{
			return this.overallComboInfo;
		}
	}

	private void Start()
	{
		if (this.DebugLog)
		{
			UnityEngine.Debug.Log("Combo manager started");
		}
		if (EntityManager.Instance != null)
		{
			EntityManager entityManager = EntityManager.Instance;
			entityManager.PlayerKillEvent = (EntityManager.PlayerKill)Delegate.Combine(entityManager.PlayerKillEvent, new EntityManager.PlayerKill(this.ComboCount));
		}
		if (PlayerDieManager.Instance != null)
		{
			PlayerDieManager playerDieManager = PlayerDieManager.Instance;
			playerDieManager.PlayerDiedEvent = (PlayerDieManager.PlayerDied)Delegate.Combine(playerDieManager.PlayerDiedEvent, new PlayerDieManager.PlayerDied(this.OnPlayerDie));
		}
		this.player = PlayerInteractionsManager.Instance.Player;
		this.weaponController = this.player.GetComponent<WeaponController>();
		if (this.AutoСalculateMaxTime)
		{
			this.maxTimeBetweenKills = (float)(90 / QualityManager.CountPedestrians) * this.AutoCalculateMult;
			this.MaxTimeBetweenKills = this.maxTimeBetweenKills;
		}
		else
		{
			this.maxTimeBetweenKills = this.MaxTimeBetweenKills;
		}
		if (this.ComboPanel != null && this.ComboMeterText != null && this.ComboMeterSlider != null)
		{
			this.ComboPanel.gameObject.SetActive(false);
			this.ComboMeterText.gameObject.SetActive(false);
			this.ComboMeterSlider.gameObject.SetActive(false);
		}
		else
		{
			UnityEngine.Debug.Log("Some components of ComboManager was NOT be seted. It's may cause some problems.");
		}
	}

	private void OnPlayerDie(float deathTime)
	{
		this.ResetAllCombos();
	}

	private void Update()
	{
		if (this.KillingDellay() > this.maxTimeBetweenKills)
		{
			this.ResetAllCombos();
		}
		this.ManageComboMeter();
	}

	public float KillingDellay()
	{
		return Time.time - this.previousKillTime;
	}

	private float TimeLeft()
	{
		float num = this.previousKillTime + this.maxTimeBetweenKills - Time.time;
		return (num <= 0f) ? 0f : num;
	}

	private void ComboCount(HitEntity enemy)
	{
		if (!(enemy is VehicleStatus))
		{
			WeaponNameList weaponNameList = WeaponNameList.None;
			if (this.KillingDellay() <= this.maxTimeBetweenKills || this.previousWeapon == WeaponNameList.None)
			{
				if (enemy.KilledByAbillity == WeaponNameList.None)
				{
					weaponNameList = this.weaponController.CurrentWeapon.WeaponNameInList;
				}
				else
				{
					weaponNameList = enemy.KilledByAbillity;
				}
				this.overallComboInfo.ComboMultiplier++;
				this.overallComboInfo.WeaponNameInList = weaponNameList;
				this.overallComboInfo.LastVictim = enemy;
				if (this.previousWeapon == weaponNameList)
				{
					this.weaponComboInfo.ComboMultiplier++;
				}
				else
				{
					this.weaponComboInfo.ComboMultiplier = 1;
				}
				this.weaponComboInfo.WeaponNameInList = weaponNameList;
				this.weaponComboInfo.LastVictim = enemy;
			}
			this.previousKillTime = Time.time;
			this.previousWeapon = weaponNameList;
			if (this.OverallComboEvent != null)
			{
				this.OverallComboEvent(this.OverallComboInfo);
			}
			if (this.WeaponComboEvent != null && !PlayerInteractionsManager.Instance.inVehicle)
			{
				this.WeaponComboEvent(this.WeaponComboInfo);
			}
			if (this.DebugLog)
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					enemy.name,
					" killed. X",
					this.OverallComboInfo.ComboMultiplier,
					" overallCOMBO by ",
					this.OverallComboInfo.WeaponNameInList
				}));
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					enemy.name,
					" killed. X",
					this.WeaponComboInfo.ComboMultiplier,
					" weaponCOMBO by ",
					this.WeaponComboInfo.WeaponNameInList
				}));
			}
		}
	}

	public void ResetWeaponCombo()
	{
		this.weaponComboInfo.WeaponNameInList = (this.previousWeapon = WeaponNameList.None);
		this.weaponComboInfo.ComboMultiplier = 0;
	}

	public void ResetOverallCombo()
	{
		this.OverallComboInfo.WeaponNameInList = (this.previousWeapon = WeaponNameList.None);
		this.OverallComboInfo.ComboMultiplier = (this.overallComboMultiplier = 0);
	}

	public void ResetAllCombos()
	{
		this.UpdateComboMeter(0, string.Empty, true);
		this.ResetWeaponCombo();
		this.ResetOverallCombo();
	}

	private void ManageComboMeter()
	{
		float num = this.TimeLeft();
		if (num <= 0.001f && this.previousKillTime != 0f)
		{
			this.UpdateComboMeter(0, string.Empty, true);
			this.previousKillTime = 0f;
		}
		if (this.ComboMeterSlider != null && this.ComboMeterSlider.gameObject.activeInHierarchy)
		{
			this.ComboMeterSlider.value = num / this.maxTimeBetweenKills;
		}
	}

	public void UpdateComboMeter(int comboMult, string weaponName, bool turnOff = false)
	{
		if (this.ComboPanel != null && this.ComboMeterText != null && this.ComboMeterSlider != null)
		{
			if (this.DebugLog)
			{
				UnityEngine.Debug.Log("Обновляю стату на экране.");
			}
			if (turnOff)
			{
				this.ComboPanel.gameObject.SetActive(false);
				this.ComboMeterText.gameObject.SetActive(false);
				this.ComboMeterSlider.gameObject.SetActive(false);
				this.ComboMeterText.text = string.Empty;
				this.previousComboMeterUpdateTime = Time.time;
			}
			else if (Time.time - this.previousComboMeterUpdateTime >= this.ComboMeterUpdateCooldown)
			{
				this.ComboPanel.gameObject.SetActive(true);
				this.ComboMeterText.gameObject.SetActive(true);
				this.ComboMeterSlider.gameObject.SetActive(true);
				this.ComboMeterText.text = string.Concat(new object[]
				{
					"X",
					comboMult,
					" COMBO with ",
					weaponName
				});
				this.previousComboMeterUpdateTime = Time.time;
			}
		}
	}

	private static ComboManager instance;

	public bool DebugLog;

	[Space(10f)]
	public float MaxTimeBetweenKills = 3f;

	public bool AutoСalculateMaxTime = true;

	public float AutoCalculateMult = 1f;

	[Space(10f)]
	public Transform ComboPanel;

	public Text ComboMeterText;

	public Slider ComboMeterSlider;

	public float ComboMeterUpdateCooldown = 0.01f;

	public ComboManager.ComboDelegate OverallComboEvent;

	public ComboManager.ComboDelegate WeaponComboEvent;

	private Player player;

	private WeaponController weaponController;

	private WeaponNameList previousWeapon = WeaponNameList.None;

	private WeaponNameList neededWeapon = WeaponNameList.None;

	private float maxTimeBetweenKills;

	private float previousKillTime;

	private float previousComboMeterUpdateTime;

	private int overallComboMultiplier;

	private ComboManager.ComboInfo weaponComboInfo = new ComboManager.ComboInfo();

	private ComboManager.ComboInfo overallComboInfo = new ComboManager.ComboInfo();

	public class ComboInfo
	{
		public WeaponNameList WeaponNameInList = WeaponNameList.None;

		public int ComboMultiplier;

		public HitEntity LastVictim;
	}

	public class BaseComboEffect : BaseBuff
	{
		public virtual void StartEffect(ComboManager.ComboInfo comboInfo)
		{
			if (this.MultAsStackCount)
			{
				this.currStacks = comboInfo.ComboMultiplier;
			}
			this.StartEffect();
		}

		public virtual void StopEffect(ComboManager.ComboInfo comboInfo)
		{
			this.StopEffect();
		}

		public override void AddStackEffect()
		{
		}

		public override void ClearStacksEffects()
		{
		}

		[Separator("Combo specific parameters")]
		[Tooltip("Use combo multiplier as start stack count?")]
		public bool MultAsStackCount;

		[Tooltip("Finisher will stop other combo-effects and disallow new one untill it's own effet ends")]
		public bool IsFinisher;
	}

	public delegate void ComboDelegate(ComboManager.ComboInfo curCombo);
}
